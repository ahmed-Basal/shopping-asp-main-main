namespace core.Entities
{
    public class DeliveryMethod:BaseEntities<int>
    {
        public DeliveryMethod() { }
        public DeliveryMethod(string name,decimal price,string Description,string DeliveryTime)
        {
           this.Name = name;
            this.Price = price;
            this.Description = Description;
            this.DeliveryTime = DeliveryTime;
        }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string DeliveryTime { get; set; }
    }
}