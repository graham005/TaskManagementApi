using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using TaskManagementApi.Data_Models;
using TaskManagementApi.DTO;

namespace TaskManagementApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<TaskController> _logger;
        private readonly IConfiguration _config;

        public UserController(UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        ILogger<TaskController> logger,
        IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _config = configuration;
           
        }
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register(Register model)
        {
            var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                return Ok("Registration successful");
            }

            return BadRequest(result.Errors);
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(Login model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                return BadRequest("Invalid credentials");
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);

            if (result.Succeeded)
            {
                var token = GenerateJwtToken(user);
                return Ok(token);
            }

            return BadRequest("Invalid credentials");
        }
        private string GenerateJwtToken(ApplicationUser user)
        {
            var jwt = _config["JwtSettings:secret"];
            if (jwt == null)
            {
                throw new InvalidOperationException("JWT secret key is null or empty.");
            }
            var securitykey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt));
            var credentials = new SigningCredentials(securitykey,SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(_config["JwtSettings:Issuer"], _config["JwtSettings:Audience"], null,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: credentials
                );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
