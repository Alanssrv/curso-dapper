using System.Data;
using Dapper.Contrib.Extensions;
using eCommerce.API.Models;
using eCommerce.API.Repository.Interfaces;
using Microsoft.Data.SqlClient;

namespace eCommerce.API.Repository
{
    public class UsuarioContribRepository : IUsuarioRepository
    {
        private IDbConnection _connection;

        public UsuarioContribRepository()
        {
            string? connectionString = Environment.GetEnvironmentVariable("eCommerceConnectionString");
            if (string.IsNullOrEmpty(connectionString))
                throw new Exception("Environment connection string not provide");

            _connection = new SqlConnection(connectionString);
        }

        public List<Usuario> Get()
        {
            return _connection.GetAll<Usuario>().ToList();
        }

        public Usuario? GetById(int id)
        {
            return _connection.Get<Usuario>(id);
        }

        public void Insert(Usuario usuario)
        {
            var usuarioId = _connection.Insert(usuario);
            usuario.Id = (int)usuarioId;
        }

        public void Update(Usuario usuario)
        {
            _connection.Update(usuario);
        }

        public void Delete(int id)
        {
            var usuario = GetById(id);
            _connection.Delete(usuario);
        }
    }
}
