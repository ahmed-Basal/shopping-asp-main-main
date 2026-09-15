namespace core.Entities
{
    public class BasketItem
    {

        public int Id { get; set; }

        public int ProductId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string PictureUrl { get; set; }

        public int Quantity { get; set; }


    }
}