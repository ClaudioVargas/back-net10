using back.Core.Entity;
using back.Core.Interfaces;
using back.Repositories;
using back.Services;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace back.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactoController : ControllerBase
    {
        private readonly IContactoService _contactoService;

        public ContactoController(IContactoService contactoService)
        {
            _contactoService = contactoService;
        }

        // GET: api/<ContactoController>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var contactos = await _contactoService.GetAll();
            return Ok(contactos);
        }

        // GET api/<ContactoController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var contacto = await _contactoService.GetById(id);
            if (contacto == null)
            {
                return NotFound();
            }
            return Ok(contacto);
        }

        // POST api/<ContactoController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Contacto contacto)
        {
            // codigo para probar el middleware
            //throw new ArgumentOutOfRangeException(nameof(contacto.Telefono), "El telefono ya existe");
            try
            {

                var contactoCreado = await _contactoService.Add(contacto);
                if (contactoCreado != null) { 
                    return new ObjectResult(contactoCreado) { StatusCode = StatusCodes.Status201Created };
                }
                return Conflict("El usuario ya existe en la base de datos.");
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        // PUT api/<ContactoController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Contacto contacto)
        {
            contacto.Id = id;
            var editado = await _contactoService.Update(contacto);
            if(editado)
            {
                return Ok();
            }
            return NotFound();
        }

        // DELETE api/<ContactoController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var eliminado = await _contactoService.Delete(id);
            if(eliminado)
            {
                return Ok();
            }
            return NotFound();

        }
    }
}
