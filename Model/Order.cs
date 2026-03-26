namespace pr_49.Model
{
    public class Order
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int MenuId { get; set; }
        public string DishesJson { get; set; }
        public decimal TotalPrice { get; set; }
        public string Status { get; set; }
        public string CreatedAt { get; set; }
    }
}
