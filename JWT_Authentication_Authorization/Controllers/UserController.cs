using JWT_Authentication_Authorization.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace JWT_Authentication_Authorization.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        public IConfiguration _config;
        public UserController(IConfiguration config)
        {
            _config = config;
        }

        private User AuthenticateUser(User user)
        {
            User _user = null;
            if (user.Username == "admin" && user.Password == "12345")
            {
                _user = new User { Username = "Selvi" };
            }
            return _user;
        }

        private string GenerateToken(User user)
        {
            var securitykey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credenntials = new SigningCredentials(securitykey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(_config["Jwt:Issuer"], _config["Jwt:Audience"], null,
                expires: DateTime.Now.AddMinutes(1),
                signingCredentials: credenntials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult User(User user)
        {
            IActionResult response = Unauthorized();
            var user_ = AuthenticateUser(user);
            if (user_ != null)
            {
                var token = GenerateToken(user_);
                response = Ok(new { token = token });
            }
            return response;
        }
    }

    

}
