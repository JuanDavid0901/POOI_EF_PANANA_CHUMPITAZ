using System.ComponentModel.DataAnnotations;

namespace POOI_EF_PANANA_CHUMPITAZ.Models
{
    public class Consumidor
    {
        [Required(ErrorMessage = "Ingrese el DNI.")]
        [RegularExpression(@"^[0-9]{8}$",
            ErrorMessage = "El DNI debe tener exactamente 8 digitos.")]
        [Display(Name = "DNI")]
        public string dnicont { get; set; }

        [Required(ErrorMessage = "Ingrese los nombres.")]
        [StringLength(40,
            ErrorMessage = "Los nombres deben tener como maximo 40 caracteres.")]
        [Display(Name = "Nombres")]
        public string nomcont { get; set; }

        [Required(ErrorMessage = "Ingrese los apellidos.")]
        [StringLength(40,
            ErrorMessage = "Los apellidos deben tener como maximo 40 caracteres.")]
        [Display(Name = "Apellidos")]
        public string apecont { get; set; }

        [EmailAddress(ErrorMessage = "Ingrese un correo valido.")]
        [StringLength(250,
            ErrorMessage = "El correo debe tener como maximo 250 caracteres.")]
        [Display(Name = "Correo electronico")]
        public string emailcont { get; set; }

        [Required(ErrorMessage = "Seleccione un pais.")]
        [RegularExpression(@"^[A-Z]{3}$",
            ErrorMessage = "Seleccione un pais valido.")]
        [Display(Name = "Pais")]
        public string idpais { get; set; }
    }
}