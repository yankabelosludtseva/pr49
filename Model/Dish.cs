namespace pr_49.Model
{
    public class Dish
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Category { get; set; }
        public bool IsAvailable { get; set; }
        public int MenuId { get; set; }
    }
}
