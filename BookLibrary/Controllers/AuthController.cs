using BookLibrary.Models;
using BookLibrary.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register(string username, string password)
        {
            if (await _authService.UserAlreadyExistAsync(username))
            {
                return BadRequest(new Response()
                {
                    Error = new SimpleError()
                    {
                        Message = "Пользователь с таким именем уже существует"
                    }
                });
            }

            else if (!Regex.IsMatch(password, @"(?=.*[0-9])(?=.*[A-Z])[0-9a-zA-Z!@#$%^&*]{6,}"))
            {
                return BadRequest(new Response()
                {
                    Error = new SimpleError()
                    {
                        Message = "Пароль должен содержать более 6 символов, как минимум 1 цифру и 1 букву в верхнем регистре"
                    }
                });
            }
            await _authService.AddUserAsync(username, password);
            return Ok(new Response()
            {
                Result = "Регистрация прошла успешно"
            });
        }
    }
}
