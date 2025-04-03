using Dapper.Contrib.Extensions;
using eCommerce.API.Models.Base;

namespace eCommerce.API.Models
{
    public class Usuario : BaseModel
    {
        public string Nome { get; set; }

        public string Email { get; set; }

        public string Sexo { get; set; }

        public string RG { get; set; }

        public string CPF { get; set; }

        public string NomeMae { get; set; }

        public string SituacaoCadastro { get; set; }

        public DateTimeOffset DataCadastro { get; set; }

        [Write(false)]
        public Contato? Contato { get; set; }
        [Write(false)]
        public ICollection<EnderecoEntrega>? EnderecosEntrega { get; set; }
        [Write(false)]
        public ICollection<Departamento>? Departamentos { get; set; }
    }
}
