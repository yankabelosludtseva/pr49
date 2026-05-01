using Microsoft.AspNetCore.Mvc;
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
        /// Создание нового заказа
        /// </summary>
        /// <remarks>
        /// Данный метод создает новый заказ для авторизованного пользователя.
        /// </remarks>
        /// <param name="token">Токен авторизации пользователя (ID пользователя)</param>
        /// <param name="request">Объект запроса с данными заказа</param>
        /// <response code="200">Заказ успешно создан. Возвращает информацию о созданном заказе</response>
        /// <response code="400">Неверный запрос: отсутствуют обязательные поля или неверный формат данных</response>
        /// <response code="401">Ошибка авторизации: неверный или отсутствующий токен, пользователь не найден</response>
        /// <response code="500">Внутренняя ошибка сервера при создании заказа</response>
        [HttpPost("CreateOrder")]
        public IActionResult CreateOrder([FromQuery] string token, [FromBody] OrderRequest request)
        {
            try
            {
                // Проверка токена
                if (string.IsNullOrEmpty(token))
                {
                    return Unauthorized(new { message = "Требуется токен авторизации" });
                }

                if (!int.TryParse(token, out int userId))
                {
                    return Unauthorized(new { message = "Неверный токен" });
                }

                var user = _userContext.Users.FirstOrDefault(u => u.Id == userId);
                if (user == null)
                {
                    return Unauthorized(new { message = "Пользователь не найден" });
                }

                // Проверка наличия тела запроса
                if (request == null)
                {
                    return BadRequest(new { message = "Тело запроса не может быть пустым" });
                }

                // Проверка обязательных полей
                if (string.IsNullOrWhiteSpace(request.Address))
                {
                    return BadRequest(new { message = "Адрес доставки обязателен для заполнения" });
                }

                if (string.IsNullOrWhiteSpace(request.Date))
                {
                    return BadRequest(new { message = "Дата доставки обязательна для заполнения" });
                }

                if (request.Dishes == null || !request.Dishes.Any())
                {
                    return BadRequest(new { message = "Список блюд не может быть пустым" });
                }

                // Проверка корректности каждого блюда
                foreach (var item in request.Dishes)
                {
                    if (item.DishId <= 0)
                    {
                        return BadRequest(new { message = $"Неверный идентификатор блюда: {item.DishId}" });
                    }
                    if (item.Count <= 0)
                    {
                        return BadRequest(new { message = $"Количество блюда с ID {item.DishId} должно быть больше 0" });
                    }
                }

                // Создаем заказ
                var order = new Order
                {
                    UserId = userId,
                    Address = request.Address,
                    DeliveryDate = request.Date,
                    CreatedAt = DateTime.Now,
                    TotalPrice = 0,
                    OrderDishes = new List<OrderDish>()
                };

                decimal totalPrice = 0;
                var notFoundDishes = new List<int>();
                var unavailableDishes = new List<string>();

                foreach (var item in request.Dishes)
                {
                    var dish = _dishContext.Dishes.FirstOrDefault(d => d.Id == item.DishId);

                    if (dish == null)
                    {
                        notFoundDishes.Add(item.DishId);
                        continue;
                    }

                    if (!dish.IsAvailable)
                    {
                        unavailableDishes.Add(dish.Name);
                        continue;
                    }

                    var orderDish = new OrderDish
                    {
                        DishId = dish.Id,
                        Count = item.Count,
                        PriceAtOrder = dish.Price
                    };

                    order.OrderDishes.Add(orderDish);
                    totalPrice += dish.Price * item.Count;
                }

                // Проверка наличия блюд в заказе
                if (notFoundDishes.Any())
                {
                    return BadRequest(new
                    {
                        message = "Некоторые блюда не найдены",
                        notFoundDishIds = notFoundDishes
                    });
                }

                if (unavailableDishes.Any())
                {
                    return BadRequest(new
                    {
                        message = "Некоторые блюда временно недоступны",
                        unavailableDishes = unavailableDishes
                    });
                }

                if (!order.OrderDishes.Any())
                {
                    return BadRequest(new { message = "Не удалось добавить ни одного блюда в заказ" });
                }

                order.TotalPrice = totalPrice;
                _orderContext.Orders.Add(order);
                _orderContext.SaveChanges();

                return Ok(new
                {
                    message = "Заказ успешно создан",
                    orderId = order.Id,
                    totalPrice = totalPrice,
                    address = order.Address,
                    deliveryDate = order.DeliveryDate,
                    createdAt = order.CreatedAt,
                    dishesCount = order.OrderDishes.Count
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Ошибка при создании заказа", error = ex.Message });
            }
        }

        /// <summary>
        /// Получение истории заказов пользователя
        /// </summary>
        /// <remarks>
        /// Данный метод возвращает список всех заказов авторизованного пользователя с полной информацией о заказанных блюдах.
        /// </remarks>
        /// <param name="token">Токен авторизации пользователя (ID пользователя)</param>
        /// <response code="200">Успешное получение истории заказов. Возвращает список заказов с блюдами</response>
        /// <response code="401">Ошибка авторизации: неверный или отсутствующий токен, пользователь не найден</response>
        /// <response code="500">Внутренняя ошибка сервера при получении истории</response>
        [HttpGet("GetHistory")]
        public IActionResult GetHistory([FromQuery] string token)
        {
            try
            {
                if (string.IsNullOrEmpty(token))
                {
                    return Unauthorized(new { message = "Требуется токен авторизации" });
                }

                if (!int.TryParse(token, out int userId))
                {
                    return Unauthorized(new { message = "Неверный токен" });
                }

                var user = _userContext.Users.FirstOrDefault(u => u.Id == userId);
                if (user == null)
                {
                    return Unauthorized(new { message = "Пользователь не найден" });
                }

                // Получаем заказы пользователя
                var orders = _orderContext.Orders
                    .Where(o => o.UserId == userId)
                    .OrderByDescending(o => o.CreatedAt)
                    .ToList();

                // Для каждого заказа получаем блюда
                var result = new List<object>();

                foreach (var order in orders)
                {
                    // Получаем блюда для текущего заказа
                    var orderDishes = _orderContext.OrderDishes
                        .Where(od => od.OrderId == order.Id)
                        .ToList();

                    var dishes = new List<object>();

                    foreach (var orderDish in orderDishes)
                    {
                        var dish = _dishContext.Dishes.FirstOrDefault(d => d.Id == orderDish.DishId);
                        if (dish != null)
                        {
                            dishes.Add(new
                            {
                                dish.Id,
                                dish.Name,
                                dish.Category,
                                orderDish.Count,
                                orderDish.PriceAtOrder,
                                TotalPrice = orderDish.PriceAtOrder * orderDish.Count
                            });
                        }
                    }

                    result.Add(new
                    {
                        order.Id,
                        order.Address,
                        order.DeliveryDate,
                        order.TotalPrice,
                        order.CreatedAt,
                        Dishes = dishes
                    });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Ошибка при получении истории", error = ex.Message });
            }
        }

        /// <summary>
        /// Модель запроса для создания заказа
        /// </summary>
        public class OrderRequest
        {
            /// <summary>
            /// Адрес доставки
            /// </summary>
            public string Address { get; set; }

            /// <summary>
            /// Дата и время доставки
            /// </summary>
            public string Date { get; set; }

            /// <summary>
            /// Список заказываемых блюд
            /// </summary>
            public List<DishItem> Dishes { get; set; }
        }

        /// <summary>
        /// Модель блюда в заказе
        /// </summary>
        public class DishItem
        {
            /// <summary>
            /// Идентификатор блюда
            /// </summary>
            public int DishId { get; set; }

            /// <summary>
            /// Количество порций
            /// </summary>
            public int Count { get; set; }
        }
    }
}