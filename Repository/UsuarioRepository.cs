using eCommerce.API.Models;
using eCommerce.API.Repository.Interfaces;

namespace eCommerce.API.Repository
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private static List<Usuario> _usuariosDb =
        [
            new() { Id = 1, Nome = "John Doe", Email = "john.doe@example.com" },
            new() { Id = 2, Nome = "Jane Smith", Email = "jane.smith@example.com" },
            new() { Id = 3, Nome = "Alice Johnson", Email = "alice.johnson@example.com" },
            new() { Id = 4, Nome = "Bob Brown", Email = "bob.brown@example.com" },
            new() { Id = 5, Nome = "Charlie Davis", Email = "charlie.davis@example.com" }
        ];

        public List<Usuario> Get()
        {
            return _usuariosDb;
        }

        public Usuario? GetById(int id)
        {
            return _usuariosDb.FirstOrDefault(usuario => usuario.Id == id);
        }

        public void Insert(Usuario usuario)
        {
            usuario.Id = _usuariosDb.LastOrDefault()?.Id + 1 ?? 1;

            _usuariosDb.Add(usuario);
        }

        public void Update(Usuario usuario)
        {
            _usuariosDb[_usuariosDb.FindIndex(u => u.Id == usuario.Id)] = usuario;
        }

        public void Delete(int id)
        {
            _usuariosDb.Remove(_usuariosDb.FirstOrDefault(usuario => usuario.Id == id));
        }
    }
}
