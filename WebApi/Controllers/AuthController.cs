using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Demo.Authentication.Data;
using Demo.Authentication.Dto;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Demo.Authentication.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        private readonly Database _database;

        public AuthController(Database database)
        {
            _database = database;
        }

        [HttpPost]
        [Route("Login")]
        public async Task<bool> Login(AuthDto dto)
        {
            // todo Проверяем пароль
            if (dto.Login != "admin" || dto.Password != "admin")
            {
                return false;
            }
            
            var claims = new List<Claim>();
            claims.Add(new Claim("OTUS", "ASP.NET"));
            claims.Add(new Claim(ClaimTypes.Role, dto.Login));
            claims.Add(new Claim("Coding-Skill", "ASP.NET Core MVC"));
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext
                .SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

            return true;
        }

        [HttpPost]
        [Route("logout")]
        public async Task Logout(string token)
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }
    }
}
