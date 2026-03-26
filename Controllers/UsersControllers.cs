using Microsoft.AspNetCore.Mvc;
using pr_49.Context;
using pr_49.Model;
using System;
using System.Linq;

namespace pr_49.Controllers
{
    /// <summary>
    /// Контроллер для работы с авторизацией
    /// </summary>
    [Route("api/AuthController")]
    [ApiExplorerSettings(GroupName = "v1")]
    public class AuthController : Controller
    {
        private readonly UserContext _context;

        public AuthController()
        {
            _context = new UserContext();
        }

        /// <summary>
        /// Регистрация пользователя
        /// </summary>
        /// <remarks>Данный метод регистрирует нового пользователя</remarks>
        /// <response code="201">Регистрация успешна</response>
        /// <response code="400">Проблемы при регистрации</response>
        /// <response code="500">При выполнении запроса возникли ошибки</response>
        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterRequest request)
        {
            try
            {
                // Проверяем, существует ли пользователь
                var existingUser = _context.Users.FirstOrDefault(u => u.Login == request.Login);
                if (existingUser != null)
                {
                    return BadRequest(new { message = "Пользователь уже существует" });
                }

                var user = new User
                {
                    Email = request.Email,
                    Password = request.Password,
                    Login = request.Login,

                };

                _context.Users.Add(user);
                _context.SaveChanges();

                return Ok(new { message = "Регистрация успешна" });
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }

        /// <summary>
        /// Авторизация пользователя
        /// </summary>
        /// <remarks>Данный метод выполняет вход пользователя</remarks>
        /// <response code="200">Авторизация успешна</response>
        /// <response code="400">Неверный логин или пароль</response>
        /// <response code="500">При выполнении запроса возникли ошибки</response>
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            try
            {
                // Ищем пользователя
                var user = _context.Users.FirstOrDefault(u => u.Email == request.Email && u.Password == request.Password);

                if (user == null)
                {
                    return Unauthorized(new { message = "Неверный логин или пароль" });
                }

                // Токен = Id пользователя
                string token = user.Id.ToString();

                return Ok(new { token = token });
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }
    }

    // Классы для запросов
    public class RegisterRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Login { get; set; }
    }

    public class LoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}