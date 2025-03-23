using eCommerce.API.Models;

namespace eCommerce.API.Repository.Interfaces
{
    public interface IUsuarioRepository
    {
        public List<Usuario> Get();

        public Usuario? GetById(int id);

        public void Insert(Usuario usuario);

        public void Update(Usuario usuario);

        public void Delete(int id);
    }
}
