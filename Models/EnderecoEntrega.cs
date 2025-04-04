﻿using eCommerce.API.Models.Base;

namespace eCommerce.API.Models
{
    public class EnderecoEntrega : BaseModel
    {
        public int? UsuarioId { get; set; }

        public string NomeEndereco { get; set; }

        public string CEP { get; set; }

        public string Estado { get; set; }

        public string Cidade { get; set; }

        public string Bairro { get; set; }

        public string Endereco { get; set; }

        public string Numero { get; set; }

        public string Complemento { get; set; }

        public Usuario? Usuario { get; set; }
    }
}
