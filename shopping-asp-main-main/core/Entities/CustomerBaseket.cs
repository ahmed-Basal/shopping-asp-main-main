using System;
using System.Collections.Generic;
using System.Text;

namespace core.Entities
{
    public class CustomerBasket
    {
        public CustomerBasket()
        {

        }
        public CustomerBasket(string id)
        {
            Id = id;
        }
        public string Id { get; set; } //key

        public string PaymentIntentId { get; set; }
        public string ClientSecret { get; set; }
        public List<BasketItem> basketItems { get; set; } = new List<BasketItem>(); //value
    }
}
