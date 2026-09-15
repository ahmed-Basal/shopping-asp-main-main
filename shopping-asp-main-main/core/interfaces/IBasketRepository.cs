using core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace core.interfaces
{
    public  interface IBasketRepository
    {
        Task<CustomerBasket> GetBasketAsync(string basketId);
        Task<CustomerBasket> UpdateBasketAsync(CustomerBasket basket);
        Task<bool> DeleteBasketAsync(string basketId);

      
        Task<CustomerBasket> AddBasketAsync(CustomerBasket basket);

       
        Task<CustomerBasket> AddOrUpdateItemAsync(string basketId, BasketItem item);
    }
}
