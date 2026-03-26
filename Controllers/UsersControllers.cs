using Microsoft.AspNetCore.Mvc;

namespace pr_49.Controllers
{
    public class UsersControllers
    {
        /// <summary>
        /// Контроллер для работы с аутентификацией пользователей
        /// </summary>
        [Route("api/auth")]
        [ApiExplorerSettings(GroupName = "v1")]
        public class AuthController : Controller
        {
            private readonly UserContext _context;

            public AuthController()
            {
                _context = new UserContext();
            }

            /// <summary>
            /// Регистрация нового пользователя
            /// </summary>
            /// <remarks>Создает учетную запись пользователя в системе</remarks>
            /// <param name="registerModel">Данные для регистрации</param>
            /// <response code="201">Пользователь успешно зарегистрирован</response>
            /// <response code="400">Ошибка валидации данных</response>
            /// <response code="500">Внутренняя ошибка сервера</response>
            [HttpPost("register")]
            [ProducesResponseType(201)]
            [ProducesResponseType(400)]
            [ProducesResponseType(500)]
            public ActionResult Register([FromBody] RegisterModel registerModel)
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(registerModel.Username) ||
                        string.IsNullOrWhiteSpace(registerModel.Email) ||
                        string.IsNullOrWhiteSpace(registerModel.Password))
                    {
                        return BadRequest("Все поля обязательны для заполнения");
                    }

                    if (_context.Users.Any(u => u.Email == registerModel.Email))
                    {
                        return BadRequest("Пользователь с таким email уже существует");
                    }

                    var newUser = new User
                    {
                        Username = registerModel.Username,
                        Email = registerModel.Email,
                        Password = registerModel.Password,
                    };

                    _context.Users.Add(newUser);
                    _context.SaveChanges();

                    return StatusCode(201, new
                    {
                        id = newUser.Id,
                        message = "User registered successfully"
                    });
                }
                catch (Exception)
                {
                    return StatusCode(500);
                }
            }

            /// <summary>
            /// Авторизация пользователя
            /// </summary>
            /// <remarks>Возвращает токен доступа (хэшированный ID пользователя)</remarks>
            /// <param name="loginModel">Данные для входа</param>
            /// <response code="200">Успешная авторизация</response>
            /// <response code="401">Неверные учетные данные</response>
            /// <response code="500">Внутренняя ошибка сервера</response>
            [HttpPost("login")]
            [ProducesResponseType(200)]
            [ProducesResponseType(401)]
            [ProducesResponseType(500)]
            public ActionResult Login([FromBody] LoginModel loginModel)
            {
                try
                {
                    var user = _context.Users
                        .FirstOrDefault(u => u.Email == loginModel.Email && u.Password == loginModel.Password);

                    if (user == null)
                    {
                        return Unauthorized();
                    }

                    var token = TokenHelper.GenerateToken(user.Id);

                    return Ok(new
                    {
                        user_id = user.Id,
                        token = token,
                        expires_in = 3600
                    });
                }
                catch (Exception)
                {
                    return StatusCode(500);
                }
            }
        }
    }
}
