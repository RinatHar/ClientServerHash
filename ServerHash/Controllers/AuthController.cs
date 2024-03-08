using Microsoft.AspNetCore.Mvc;
using ServerHash.Dto;
using ServerHash.Filters;
using ServerHash.Services;

namespace ServerHash.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(AuthService authService) : ControllerBase
    {
        private readonly AuthService _authService = authService;

        [HttpGet("login")]
        [TypeFilter(typeof(AuthFilter))]
        [ProducesResponseType(typeof(string[]), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login()
        {
            if (HttpContext.Items.TryGetValue("UserRights", out var userRights))
            {
                return Ok(userRights);
            }

            // Если права не найдены, возвращаем Unauthorized
            return Unauthorized("User not found!");
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
