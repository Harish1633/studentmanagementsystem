using Microsoft.AspNetCore.Mvc;
using StudentManagementAPI.DTOs;
using StudentManagementAPI.Services;

namespace StudentManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        /// <summary>
        /// Login and receive a JWT token.
        /// Demo credentials — admin / Admin@123 or user / User@123
        /// </summary>
        [HttpPost("login")]
        [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), 200)]
        [ProducesResponseType(401)]
        public IActionResult Login([FromBody] LoginDto dto)
        {
            _logger.LogInformation("Login request received for user: {Username}", dto.Username);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = _authService.Login(dto);
            if (result == null)
                return Unauthorized(ApiResponse<AuthResponseDto>.Fail("Invalid username or password."));

            return Ok(ApiResponse<AuthResponseDto>.Ok(result, "Login successful."));
        }
    }
}
