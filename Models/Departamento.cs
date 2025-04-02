using eCommerce.API.Models.Base;

namespace eCommerce.API.Models
{
    public class Departamento : BaseModel
    {
        public string Nome { get; set; }

        public ICollection<Usuario>? Usuarios { get; set; }
    }
}
