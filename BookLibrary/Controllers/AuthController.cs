using BookLibrary.Models;
using BookLibrary.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookLibrary.Controllers
{
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly AuthService _authService;

        public AuthController(AuthService service)
        {
            _authService = service;
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login(string username, string password)
        {
            var identity = await _authService.GetIdentityAsync(username, password);
            if (identity == null)
            {
                return BadRequest(new Response()
                {
                    Error = new SimpleError() { Message = "Неправильный логин или пароль" }
                });
            }
            return Ok(new Response()
            {
                Result = _authService.GetToken(identity)
            });
        }
    }
}
