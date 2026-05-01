namespace pr_49.Model
{
    public class OrderDish
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int DishId { get; set; }
        public int Count { get; set; }
        public decimal PriceAtOrder { get; set; } // Цена на момент заказа

        // Навигационные свойства
        public virtual Order Order { get; set; }
        public virtual Dish Dish { get; set; }
    }
}