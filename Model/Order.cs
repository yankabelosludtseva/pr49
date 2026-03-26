namespace pr_49.Model
{
    public class Order
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Address { get; set; }
        public string DeliveryDate { get; set; }
    }
}
