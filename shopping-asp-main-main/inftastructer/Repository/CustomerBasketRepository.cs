using core.Entities;
using core.interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Storage;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

using IDatabase = StackExchange.Redis.IDatabase;

namespace inftastructer.Repository
{
    public class CustomerBasketRepository : IBasketRepository
    {
        private readonly IDatabase _database;

        public CustomerBasketRepository(IConnectionMultiplexer redis)
        {
            _database = redis.GetDatabase();
        }

        public Task<bool> DeleteBasketAsync(string basketId)
        {
            return _database.KeyDeleteAsync(basketId);
        }

        public async Task<CustomerBasket> GetBasketAsync(string basketId)
        {
            var result = await _database.StringGetAsync(basketId);
            var json = result.HasValue ? result.ToString() : null;
            if (!string.IsNullOrEmpty(json))
                return JsonSerializer.Deserialize<CustomerBasket>(json);
            return null;
        }

        public async Task<CustomerBasket> UpdateBasketAsync(CustomerBasket basket)
        {
            var success = await _database.StringSetAsync(basket.Id, JsonSerializer.Serialize(basket), TimeSpan.FromDays(3));
            if (success)
                return await GetBasketAsync(basket.Id);
            return null;
        }

        // إنشاء سلة جديدة فقط
        public async Task<CustomerBasket> AddBasketAsync(CustomerBasket basket)
        {
            var existing = await GetBasketAsync(basket.Id);
            if (existing != null)
                return null; // السلة موجودة → ممنوع الإضافة
            await UpdateBasketAsync(basket);
            return basket;
        }

        // إضافة أو تحديث عنصر → السلة يجب أن تكون موجودة
        public async Task<CustomerBasket> AddOrUpdateItemAsync(string basketId, BasketItem item)
        {
            var basket = await GetBasketAsync(basketId);
            if (basket == null)
                return null; // السلة مش موجودة → ممنوع الإضافة

            var existingItem = basket.basketItems.FirstOrDefault(x => x.Name.Equals(item.Name, StringComparison.OrdinalIgnoreCase));
            if (existingItem != null)
            {
                existingItem.Quantity += item.Quantity;
                existingItem.Price = item.Price;
                existingItem.PictureUrl = item.PictureUrl;
            }
            else
            {
                basket.basketItems.Add(item);
            }

            await UpdateBasketAsync(basket);
            return basket;
        }
    }
}


