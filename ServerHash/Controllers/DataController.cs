using ServerHash.Dto;
using ServerHash.Models;
using Microsoft.AspNetCore.Mvc;
using ServerHash.Filters;
using ServerHash.Services;

namespace Lab1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DataController(DataService dataService, AuthService authService) : ControllerBase
    {
        private readonly DataService _dataService = dataService;
        private readonly AuthService _authService = authService;

        [HttpGet("read")]
        [TypeFilter(typeof(AuthFilter))]
        [ProducesResponseType(typeof(Data), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Get()
        {
            try
            {
                List<DataDto> data = await _dataService.GetAllData();
                return new JsonResult(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPost("write")]
        [TypeFilter(typeof(AuthFilter))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Post(DataDto data)
        {
            try
            {
                // Проверка прав доступа
                if (!_authService.UserHasWriteAccess(HttpContext))
                {
                    return new ForbidResult();
                }

                await _dataService.AddData(data);

                return new JsonResult("Added successfully!");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        

    }
}
