using Microsoft.AspNetCore.Mvc;
using pr_49.Context;
using pr_49.Model;
using System;
using System.Linq;

namespace pr_49.Controllers
{
    /// <summary>
    /// Контроллер для работы с блюдами
    /// </summary>
    [Route("api/DishController")]
    [ApiExplorerSettings(GroupName = "v1")]
    public class DishController : Controller
    {
        private readonly DishContext _context;

        public DishController()
        {
            _context = new DishContext();
        }

        /// <summary>
        /// Получение списка блюд
        /// </summary>
        /// <remarks>Данный метод получает список блюд, можно фильтровать по MenuId</remarks>
        /// <param name="menuId">Идентификатор меню (необязательный)</param>
        /// <response code="200">Список успешно получен</response>
        /// <response code="400">При выполнении запроса возникли ошибки</response>
        [HttpGet("List")]
        public IActionResult List([FromQuery] int? menuId = null)
        {
            try
            {
                var query = _context.Dishes.AsQueryable();

                if (menuId.HasValue)
                {
                    query = query.Where(d => d.MenuId == menuId.Value);
                }

                var dishes = query.ToList();
                return Ok(dishes);
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }

        /// <summary>
        /// Получение блюд по категории
        /// </summary>
        /// <remarks>Данный метод получает блюда по категории</remarks>
        /// <param name="category">Категория блюда</param>
        /// <response code="200">Список успешно получен</response>
        /// <response code="400">При выполнении запроса возникли ошибки</response>
        [HttpGet("GetByCategory")]
        public IActionResult GetByCategory([FromQuery] string category)
        {
            try
            {
                var dishes = _context.Dishes
                    .Where(d => d.Category == category && d.IsAvailable)
                    .ToList();
                return Ok(dishes);
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }
    }
}