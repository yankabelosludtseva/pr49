using Microsoft.AspNetCore.Mvc;
using pr_49.Context;
using pr_49.Model;
using System;
using System.Linq;

namespace pr_49.Controllers
{
    /// <summary>
    /// Контроллер для работы с меню
    /// </summary>
    [Route("api/MenuController")]
    [ApiExplorerSettings(GroupName = "v1")]
    public class MenuController : Controller
    {
        private readonly MenuContext _context;

        public MenuController()
        {
            _context = new MenuContext();
        }

        /// <summary>
        /// Получение списка версий меню по типу
        /// </summary>
        /// <remarks>Данный метод получает меню по типу (завтрак, обед, ужин)</remarks>
        /// <param name="type">Тип меню</param>
        /// <response code="200">Список успешно получен</response>
        /// <response code="400">При выполнении запроса возникли ошибки</response>
        [HttpGet("GetByType")]
        public IActionResult GetByType([FromQuery] string type)
        {
            try
            {
                var menus = _context.Menus
                    .Where(m => m.Type == type)
                    .ToList();
                return Ok(menus);
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }

        /// <summary>
        /// Получение всех меню
        /// </summary>
        /// <remarks>Данный метод получает список всех меню</remarks>
        /// <response code="200">Список успешно получен</response>
        /// <response code="400">При выполнении запроса возникли ошибки</response>
        [HttpGet("List")]
        public IActionResult List()
        {
            try
            {
                var menus = _context.Menus.ToList();
                return Ok(menus);
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }
    }
}