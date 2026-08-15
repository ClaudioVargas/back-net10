using System.ComponentModel.DataAnnotations;

namespace back.Core.Entity
{
    public class Contacto
    {
        public int Id { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "El campo {0} es obligatorio.")]
        public string  Nombre { get; set; } = string.Empty;
        [Required(AllowEmptyStrings = false, ErrorMessage = "El campo {0} es obligatorio.")]
        public string? Telefono { get; set; }
    }
}
