using eCommerce.API.Models.Base;

namespace eCommerce.API.Models
{
    public class Contato : BaseModel
    {
        public int UsuarioId { get; set; }

        public string Telefone { get; set; }

        public string Celular { get; set; }

        public Usuario Usuario { get; set; }
    }
}
