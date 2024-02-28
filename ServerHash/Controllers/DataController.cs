using ServerHash.Dto;
using ServerHash.Models;
using Microsoft.AspNetCore.Mvc;
using ServerHash.Filters;
using ServerHash.Services;

namespace Lab1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DataController : ControllerBase
    {
        private readonly DataService _dataService;
        private readonly AuthService _authService;

        public DataController(DataService dataService, AuthService authService)
        {
            _dataService = dataService;
            _authService = authService;
        }

        [HttpGet("read")]
        [TypeFilter(typeof(AuthFilter))]
        [ProducesResponseType(typeof(Data), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Get()
        {
            try
            {
                List<Data> data = await _dataService.GetAllData();
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
                    return new StatusCodeResult(StatusCodes.Status403Forbidden);
                }

                var newData = new Data { Value = data.Value };
                await _dataService.AddData(newData);

                return new JsonResult("Added successfully!");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("login")]
        [TypeFilter(typeof(AuthFilter))]
        [ProducesResponseType(typeof(string[]), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult Login()
        {
            // Проверяем наличие прав в Items
            if (HttpContext.Items.TryGetValue("UserRights", out var userRights))
            {
                // Приводим объект к нужному типу (JsonResult)
                var userRightsJson = userRights as JsonResult;

                if (userRightsJson != null)
                {
                    return userRightsJson;
                }
            }

            // Если права не найдены, возвращаем Unauthorized
            return Unauthorized("Пользователь не найден");
        }


        [HttpPost("registration")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Registration(UserDto user)
        {
            try
            {
                await _authService.RegisterUser(user);
                return new JsonResult("Registration successful!");
            }

            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

    }
}
