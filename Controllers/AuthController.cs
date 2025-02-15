using Microsoft.AspNetCore.Mvc;
using azspx_mgt_web.Models;
using azspx_mgt_web.Services;

namespace azspx_mgt_web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        IAuthService authService,
        ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
    {
        try
        {
            if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
            {
                return BadRequest(new { message = "Username and password are required" });
            }

            var response = await _authService.AuthenticateAsync(request);
            if (response == null)
            {
                return Unauthorized(new { message = "Invalid username or password" });
            }

            _logger.LogInformation("User {Username} logged in successfully", request.Username);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login attempt for user {Username}", request.Username);
            return StatusCode(500, new { message = "An error occurred during login. Please try again." });
        }
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        // In a real application, you might want to invalidate the token
        // or perform other cleanup operations
        return Ok(new { message = "Logged out successfully" });
    }
}