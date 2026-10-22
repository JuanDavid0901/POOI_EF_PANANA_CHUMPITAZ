using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.Mvc;
using POOI_EF_PANANA_CHUMPITAZ.Models;

namespace POOI_EF_PANANA_CHUMPITAZ.Controllers
{
    public class ConsumidorController : Controller
    {
        private readonly string cadena =
            ConfigurationManager.ConnectionStrings["cn"].ConnectionString;

        private Consumidor LeerConsumidor(SqlDataReader dr)
        {
            return new Consumidor
            {
                dnicont = dr["dnicont"].ToString(),
                nomcont = dr["nomcont"].ToString(),
                apecont = dr["apecont"].ToString(),
                emailcont = dr["emailcont"].ToString(),
                idpais = dr["idpais"].ToString()
            };
        }

        private List<Consumidor> ListarConsumidores()
        {
            List<Consumidor> lista = new List<Consumidor>();

            using (SqlConnection cn = new SqlConnection(cadena))
            using (SqlCommand cmd =
                new SqlCommand("usp_listar_consumidor", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cn.Open();

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(LeerConsumidor(dr));
                    }
                }
            }

            return lista;
        }

        private Consumidor BuscarConsumidor(string dni)
        {
            using (SqlConnection cn = new SqlConnection(cadena))
            using (SqlCommand cmd =
                new SqlCommand("usp_buscar_consumidor", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@dnicont", SqlDbType.Char, 8).Value = dni;

                cn.Open();

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        return LeerConsumidor(dr);
                    }
                }
            }

            return null;
        }

        private void CargarPaises(string seleccionado = null)
        {
            List<Pais> lista = new List<Pais>();

            using (SqlConnection cn = new SqlConnection(cadena))
            using (SqlCommand cmd =
                new SqlCommand("usp_listar_paises", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cn.Open();

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new Pais
                        {
                            idpais = dr["idpais"].ToString(),
                            nompais = dr["nompais"].ToString()
                        });
                    }
                }
            }

            ViewBag.Paises = new SelectList(
                lista, "idpais", "nompais", seleccionado);
        }

        private string GuardarConsumidor(
            Consumidor consumidor, string procedimiento)
        {
            using (SqlConnection cn = new SqlConnection(cadena))
            using (SqlCommand cmd =
                new SqlCommand(procedimiento, cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@dnicont", SqlDbType.VarChar, 50)
                    .Value = consumidor.dnicont;

                cmd.Parameters.Add("@nomcont", SqlDbType.VarChar, 40)
                    .Value = consumidor.nomcont;

                cmd.Parameters.Add("@apecont", SqlDbType.VarChar, 40)
                    .Value = consumidor.apecont;

                cmd.Parameters.Add("@emailcont", SqlDbType.VarChar, 250)
                    .Value = (object)consumidor.emailcont ?? DBNull.Value;

                cmd.Parameters.Add("@idpais", SqlDbType.VarChar, 50)
                    .Value = consumidor.idpais;

                SqlParameter mensaje =
                    cmd.Parameters.Add("@mensaje", SqlDbType.VarChar, 250);

                mensaje.Direction = ParameterDirection.Output;

                cn.Open();
                cmd.ExecuteNonQuery();

                return mensaje.Value.ToString();
            }
        }

        public ActionResult Index()
        {
            return View(ListarConsumidores());
        }

        public ActionResult Create()
        {
            CargarPaises();
            return View(new Consumidor());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Consumidor consumidor)
        {
            if (ModelState.IsValid)
            {
                string mensaje = GuardarConsumidor(
                    consumidor, "usp_agrega_consumidor");

                if (mensaje == "Consumidor registrado correctamente.")
                {
                    TempData["Mensaje"] = mensaje;
                    return RedirectToAction("Index");
                }

                ModelState.AddModelError("", mensaje);
            }

            CargarPaises(consumidor.idpais);
            return View(consumidor);
        }

        public ActionResult Update(string id)
        {
            if (string.IsNullOrEmpty(id) || id.Length != 8)
            {
                return HttpNotFound();
            }

            Consumidor consumidor = BuscarConsumidor(id);

            if (consumidor == null)
            {
                return HttpNotFound();
            }

            CargarPaises(consumidor.idpais);
            return View(consumidor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(Consumidor consumidor)
        {
            if (ModelState.IsValid)
            {
                string mensaje = GuardarConsumidor(
                    consumidor, "usp_actualizar_consumidor");

                if (mensaje == "Consumidor actualizado correctamente.")
                {
                    TempData["Mensaje"] = mensaje;
                    return RedirectToAction("Index");
                }

                ModelState.AddModelError("", mensaje);
            }

            CargarPaises(consumidor.idpais);
            return View(consumidor);
        }

        public ActionResult Details(string id)
        {
            if (string.IsNullOrEmpty(id) || id.Length != 8)
            {
                return HttpNotFound();
            }

            Consumidor consumidor = BuscarConsumidor(id);

            if (consumidor == null)
            {
                return HttpNotFound();
            }

            return View(consumidor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(string id)
        {
            if (string.IsNullOrEmpty(id) || id.Length != 8)
            {
                TempData["Mensaje"] = "El DNI debe tener 8 caracteres.";
                return RedirectToAction("Index");
            }

            using (SqlConnection cn = new SqlConnection(cadena))
            using (SqlCommand cmd =
                new SqlCommand("usp_eliminar_consumidor", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@dnicont", SqlDbType.Char, 8)
                    .Value = id;

                SqlParameter mensaje =
                    cmd.Parameters.Add("@mensaje", SqlDbType.VarChar, 250);

                mensaje.Direction = ParameterDirection.Output;

                cn.Open();
                cmd.ExecuteNonQuery();

                TempData["Mensaje"] = mensaje.Value.ToString();
            }

            return RedirectToAction("Index");
        }
    }
}