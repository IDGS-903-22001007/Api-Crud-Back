using Microsoft.AspNetCore.Mvc;
using prjAPI3.Data;
using prjAPI3.Models.catEmpleados;

namespace apiEmpleados.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmpleadoController : ControllerBase
    {
        private readonly clsCatEmpleadosData _objEmpleadoData;

        public EmpleadoController(clsCatEmpleadosData empleadoData)
        {
            _objEmpleadoData = empleadoData;
        }

        [HttpGet]
        public async Task<IActionResult> listar()
        {
            List<clsCatEmpleados> Lista = await _objEmpleadoData.ListaEmpleados();
            return StatusCode(StatusCodes.Status200OK, Lista);
        }

        [HttpGet("{IdEmpleado}")]
        public async Task<IActionResult> obtener(int IdEmpleado)
        {
            clsCatEmpleados objEmpleado = await _objEmpleadoData.ObtenerEmpleado(IdEmpleado);

            // Devolver 404 si no se encuentra
            if (objEmpleado == null)
            {
                return NotFound(new { mensaje = $"Empleado con Id {IdEmpleado} no encontrado." });
            }

            return StatusCode(StatusCodes.Status200OK, objEmpleado);
        }

        [HttpPost]
        public async Task<IActionResult> crear([FromBody] clsCatEmpleados empleado)
        {
            bool respuesta = await _objEmpleadoData.CrearEmpleado(empleado);

            // Devolver 201 Created si es exitoso
            if (respuesta)
            {
                return StatusCode(StatusCodes.Status201Created, new { isSuccess = true });
            }

            // Devolver 400 Bad Request si falla
            return StatusCode(StatusCodes.Status400BadRequest, new { isSuccess = false, mensaje = "No se pudo crear el empleado." });
        }

        [HttpDelete("{IdEmpleado}")]
        public async Task<IActionResult> Eliminar(int IdEmpleado)
        {
            bool respuesta = await _objEmpleadoData.EliminarEmpleado(IdEmpleado);

            // Devolver 204 No Content si se eliminó correctamente
            if (respuesta)
            {
                return StatusCode(StatusCodes.Status204NoContent);
            }

            // Devolver 404 Not Found si el ID no existía
            return StatusCode(StatusCodes.Status404NotFound, new { isSuccess = false, mensaje = $"Empleado con Id {IdEmpleado} no encontrado para eliminar." });
        }
    }
}