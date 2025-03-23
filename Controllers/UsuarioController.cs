using eCommerce.API.Models;
using eCommerce.API.Repository.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace eCommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController(IUsuarioRepository usuarioRepository) : ControllerBase
    {
        private readonly IUsuarioRepository _usuarioRepository = usuarioRepository;

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_usuarioRepository.Get());
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var usuario = _usuarioRepository.GetById(id);
            if (usuario == null)
                return NotFound();

            return Ok(usuario);
        }

        [HttpPost]
        public IActionResult Insert([FromBody] Usuario usuario)
        {
            _usuarioRepository.Insert(usuario);
            return StatusCode(StatusCodes.Status201Created);
        }

        [HttpPut]
        public IActionResult Update([FromBody] Usuario usuario)
        {
            _usuarioRepository.Update(usuario);
            return Ok(usuario);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            if (_usuarioRepository.GetById(id) is null)
                return NotFound();

            _usuarioRepository.Delete(id);
            return Ok();
        }
    }
}
