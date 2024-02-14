using AccountManagmentAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AccountManagmentAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly ILogger<UserController> _logger;

        public UserController(UserManager<IdentityUser> userManager, IConfiguration configuration, ILogger<UserController> logger)
        {
            _userManager = userManager;
            _configuration = configuration;
            _logger = logger;
        }
        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterModel model)
        {
            _logger.LogInformation("RegisterUser method of UserController");

            var user = new IdentityUser { UserName = model.FullName, Id = model.UserId, Email = model.Email };
            var result = await _userManager.CreateAsync(user, model.Password);

            _logger.LogDebug($"user:{user}, result: {result}");

            if (result.Succeeded)
            {
                return Ok();
            }
            return BadRequest(result.Errors);
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginUser([FromBody] LoginModel model)
        {
            _logger.LogInformation("LoginUser method of UserController");
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                var token = GenerateJwtToken(user);
                _logger.LogDebug($"user:{user}, token: {token}");
                return Ok(new { token });

            }
            return Unauthorized();
        }

        private string GenerateJwtToken(IdentityUser user)
        {
            _logger.LogInformation("GenerateJwtToken method of UserController");
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
