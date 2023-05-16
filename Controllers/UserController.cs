using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PerCare.Controllers
{
    [Route("auth")]
    [ApiController]
    public class UserController : ControllerBase
    {
        public PetcareContext _context;
        public IConfiguration _config;

        public UserController(PetcareContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }
        private User AuthenticateUser(LoginCred credential)
        {
            User user_ = null;
            var dbUser = _context.Users.SingleOrDefault(u => u.Username == credential.Username && u.Password == credential.Password);
            if (dbUser != null)
            {
                user_ = new User { Username = dbUser.Username, Password = dbUser.Password, Role = dbUser.Role };
            }
            return user_;
        }
        private string GenerateToken(User credential)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, credential.Username),
                new Claim(ClaimTypes.Role, credential.Role)
            };

            var token = new JwtSecurityToken(
                _config["Jwt:Issuer"],
                _config["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddHours(24),
                signingCredentials: credentials
                );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Login(LoginCred credential)
        {
            IActionResult response = Unauthorized();
            var user_ = AuthenticateUser(credential);
            if (user_ != null)
            {
                var Token = GenerateToken(user_);
                response = Ok(new { Token = Token });
            }
            return response;
        }
        [HttpGet("getUser/{email}")]
        public async Task<ActionResult<IEnumerable<object>>> GetUser(string email)
        {
            if (_context == null)
            {
                return NotFound("Database not found!");
            }
            var users = _context.Users;

            var query = users.Where(user => user.Username == email);
            return query.ToList();
        }
    }

    public class LoginCred
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
