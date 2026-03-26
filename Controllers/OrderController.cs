using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using pr_49.Context;
using pr_49.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace pr_49.Controllers
{
    /// <summary>
    /// Контроллер для работы с заказами
    /// </summary>
    [Route("api/OrderController")]
    [ApiExplorerSettings(GroupName = "v1")]
    public class OrderController : Controller
    {
        private readonly OrderContext _orderContext;
        private readonly DishContext _dishContext;
        private readonly UserContext _userContext;

        public OrderController()
        {
            _orderContext = new OrderContext();
            _dishContext = new DishContext();
            _userContext = new UserContext();
        }

        /// <summary>
        /// Отправка заказа
        /// </summary>
        /// <remarks>Данный метод создает новый заказ. Требуется токен в параметрах</remarks>
        /// <param name="token">Токен пользователя (Id пользователя)</param>
        /// <response code="200">Заказ успешно создан</response>
        /// <response code="400">Требуется авторизация</response>
        /// <response code="401">При выполнении запроса возникли ошибки</response>
        [HttpPost("CreateOrder")]
        public IActionResult CreateOrder([FromQuery] string token, [FromBody] OrderRequest request)
        {
            try
            {
                // Проверяем токен
                if (string.IsNullOrEmpty(token))
                {
                    return Unauthorized(new { message = "Требуется токен авторизации" });
                }

                // Получаем пользователя по токену (токен = Id пользователя)
                if (!int.TryParse(token, out int userId))
                {
                    return Unauthorized(new { message = "Неверный токен" });
                }

                var user = _userContext.Users.FirstOrDefault(u => u.Id == userId);
                if (user == null)
                {
                    return Unauthorized(new { message = "Пользователь не найден" });
                }

                // Получаем блюда и считаем стоимость с учетом количества
                var orderItems = new List<object>();
                decimal totalPrice = 0;

                foreach (var item in request.Dishes)
                {
                    var dish = _dishContext.Dishes.FirstOrDefault(d => d.Id == item.DishId);
                    if (dish != null)
                    {
                        decimal itemPrice = dish.Price * item.Count;
                        totalPrice += itemPrice;

                        orderItems.Add(new
                        {
                            dish.Id,
                            dish.Name,
                            dish.Price,
                            item.Count,
                            TotalPrice = itemPrice
                        });
                    }
                }

                // Создаем заказ
                var order = new Order
                {
                    UserId = userId,
                    Address = request.Address,
                    DeliveryDate = request.Date
                };

                _orderContext.Orders.Add(order);
                _orderContext.SaveChanges();

                return Ok(new
                {
                    message = "Заказ успешно создан",
                    orderId = order.Id,
                    totalPrice = totalPrice,
                    address = order.Address,
                    deliveryDate = order.DeliveryDate
                });
            }
            catch (Exception ex)
            {
                return StatusCode(401, new { message = "Ошибка при создании заказа", error = ex.Message });
            }
        }

        /// <summary>
        /// Получение истории заказов
        /// </summary>
        /// <remarks>Данный метод получает историю заказов текущего пользователя. Требуется токен в параметрах</remarks>
        /// <param name="token">Токен пользователя (Id пользователя)</param>
        /// <response code="200">История успешно получена</response>
        /// <response code="400">Требуется авторизация</response>
        /// <response code="401">При выполнении запроса возникли ошибки</response>
        [HttpGet("GetHistory")]
        public IActionResult GetHistory([FromQuery] string token)
        {
            try
            {
                // Проверяем токен
                if (string.IsNullOrEmpty(token))
                {
                    return Unauthorized(new { message = "Требуется токен авторизации" });
                }

                // Получаем пользователя по токену (токен = Id пользователя)
                if (!int.TryParse(token, out int userId))
                {
                    return Unauthorized(new { message = "Неверный токен" });
                }

                var user = _userContext.Users.FirstOrDefault(u => u.Id == userId);
                if (user == null)
                {
                    return Unauthorized(new { message = "Пользователь не найден" });
                }

                // Получаем историю заказов пользователя
                var orders = _orderContext.Orders
                    .Where(o => o.UserId == userId)
                    .ToList();

                return Ok(orders);
            }
            catch (Exception)
            {
                return StatusCode(401);
            }
        }
    }

    /// <summary>
    /// Модель запроса на создание заказа
    /// </summary>
    public class OrderRequest
    {
        public string Address { get; set; }
        public string Date { get; set; }
        public List<DishItem> Dishes { get; set; }
    }

    /// <summary>
    /// Модель блюда в заказе
    /// </summary>
    public class DishItem
    {
        public int DishId { get; set; }
        public int Count { get; set; }
    }
}