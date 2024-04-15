using JWT_Authentication_Authorization.Context;
using JWT_Authentication_Authorization.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace JWT_Authentication_Authorization.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JWTTokenController : ControllerBase
    {
        public IConfiguration _configuration;
        public readonly ApplicationDBContext _context;

        public JWTTokenController(IConfiguration configuration, ApplicationDBContext context)
        {
            _configuration = configuration;
            _context = context;
        }
        [HttpPost]
        public async Task<IActionResult> CreateUser(Users user)
        {
            if(user != null && user.UserName != null && user.Password != null) 
            {
                var userData = await GetUser(user.UserName,user.Password);
                if (userData == null) { return BadRequest("Invalid Credentials"); }
                if(user.UserName == userData.UserName && user.Password == userData.Password) 
                {
                    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                    var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                    var claims = new[] {
                            new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                            new Claim("ID",user.UserId.ToString()),
                            new Claim("Password",user.Password),
                            new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())
                    };

                    var token = new JwtSecurityToken(_configuration["Jwt:Issuer"],
                        _configuration["Jwt:Issuer"],
                        claims,
                        expires: DateTime.Now.AddMinutes(120),
                        signingCredentials: credentials); 

                    return Ok(new JwtSecurityTokenHandler().WriteToken(token));
                    
                }
            }
            return BadRequest("Invalid Credentials");
        }

        [HttpGet]
        public async Task<Users> GetUser(string username,string password)
        {
            Users user = null;
            user = _context.Users.FirstOrDefault(x => x.UserName== username);
            if (user.Password != password) return null;
            return user;
        }
    }
}
