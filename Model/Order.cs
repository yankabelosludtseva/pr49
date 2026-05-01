using System.Collections.Generic;

namespace pr_49.Model
{
    public class Order
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Address { get; set; }
        public string DeliveryDate { get; set; }
        public decimal TotalPrice { get; set; } // Общая сумма заказа
        public DateTime CreatedAt { get; set; } // Дата создания заказа

        // Навигационное свойство
        public virtual ICollection<OrderDish> OrderDishes { get; set; }
    }
}