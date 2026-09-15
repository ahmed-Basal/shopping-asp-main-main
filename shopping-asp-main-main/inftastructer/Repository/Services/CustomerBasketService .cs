using core.Dto;
using core.Entities;
using core.interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace inftastructer.Repository.Services
{
    public class CustomerBasketService : ICustomerBasketService
    {
        private readonly IBasketRepository _basketRepo;
        private readonly IUnitOfWork unitOfWork;

        public CustomerBasketService(IBasketRepository basketRepo, IUnitOfWork unitOfWork)
        {
            _basketRepo = basketRepo;
            this.unitOfWork = unitOfWork;
        }


        public async Task<CustomerBasketDto> CreateBasketAsync(string userId, List<AddToBasketDto> itemsDto)
        {
            
            var existingBasket = await _basketRepo.GetBasketAsync(userId);
            if (existingBasket != null)
            {
                await _basketRepo.DeleteBasketAsync(userId);
            }

            var basketItems = new List<BasketItem>();
            foreach (var dto in itemsDto)
            {
              
                if (dto.Quantity <= 0) continue;

                var product = await unitOfWork.ProductRepository.GetByNameAsync(dto.ProductName);
                if (product == null)
                    throw new Exception($"Product {dto.ProductName} not found");

                basketItems.Add(new BasketItem
                {
                    Name = product.Name,
                    Price = product.NewPrice,
                    PictureUrl = product.photos?.FirstOrDefault()?.iamgename ?? string.Empty,
                    Quantity = dto.Quantity
                });
            }

            var basket = new CustomerBasket(userId)
            {
                basketItems = basketItems
            };

            var added = await _basketRepo.AddBasketAsync(basket);
            if (added == null)
                return null;

            return new CustomerBasketDto
            {
                Id = basket.Id,
                Items = basket.basketItems.Select(x => new BasketItemDto
                {
                    Name = x.Name,
                    Price = x.Price,
                    PictureUrl = x.PictureUrl,
                    Quantity = x.Quantity
                }).ToList()
            };
        }


        public async Task<CustomerBasketDto> AddOrUpdateItemAsync(string userId, AddToBasketDto dto)
        {
            var product = await unitOfWork.ProductRepository.GetByNameAsync(dto.ProductName);
            if (product == null)
                throw new Exception("Product not found");

            var basket = await _basketRepo.GetBasketAsync(userId);
            if (basket == null)
                throw new Exception("Basket does not exist");

            var existingItem = basket.basketItems.FirstOrDefault(x => x.Name == product.Name);

            if (dto.Quantity <= 0)
            {
                // لو الرقم صفر، نحذف العنصر لو موجود
                if (existingItem != null)
                    basket.basketItems.Remove(existingItem);
            }
            else
            {
                if (existingItem != null)
                {
                    // لو موجود، القيمة تتغير للقيمة الجديدة مباشرة
                    existingItem.Quantity = dto.Quantity;
                    existingItem.Price = product.NewPrice; // لو السعر ممكن يتغير
                }
                else
                {
                    // لو مش موجود، نضيفه جديد
                    basket.basketItems.Add(new BasketItem
                    {
                        Name = product.Name,
                        Price = product.NewPrice,
                        PictureUrl = product.photos?.FirstOrDefault()?.iamgename ?? string.Empty,
                        Quantity = dto.Quantity
                    });
                }
            }

            // نحفظ التغييرات في الريبو
            await _basketRepo.UpdateBasketAsync(basket);

            return new CustomerBasketDto
            {
                Id = basket.Id,
                Items = basket.basketItems.Select(x => new BasketItemDto
                {
                    Name = x.Name,
                    Price = x.Price,
                    PictureUrl = x.PictureUrl,
                    Quantity = x.Quantity
                }).ToList()
            };
        }
        public async Task<CustomerBasketDto> GetBasketAsync(string userId)
        {
            var basket = await _basketRepo.GetBasketAsync(userId);
            if (basket == null) return new CustomerBasketDto { Id = userId };
            return new CustomerBasketDto
            {
                Id = basket.Id,
                Items = basket.basketItems.Select(x => new BasketItemDto
                {
                    Name = x.Name,
                    Price = x.Price,
                    PictureUrl = x.PictureUrl,
                    Quantity = x.Quantity
                }).ToList()
            };
        }

        public async Task<bool> DeleteBasketAsync(string userId)
        {
            return await _basketRepo.DeleteBasketAsync(userId);
        }
        public async Task<CustomerBasketDto> UpdateItemQuantityAsync(string userId, AddToBasketDto dto)
        {
            var basket = await _basketRepo.GetBasketAsync(userId);
            if (basket == null)
                throw new Exception("Basket not found");

            var item = basket.basketItems.FirstOrDefault(x => x.Name == dto.ProductName);
            if (item == null)
                throw new Exception("Item not found");

            item.Quantity = dto.Quantity;

            await _basketRepo.UpdateBasketAsync(basket);

            return new CustomerBasketDto
            {
                Id = basket.Id,
                Items = basket.basketItems.Select(i => new BasketItemDto
                {
                    Name = i.Name,
                    Price = i.Price,
                    PictureUrl = i.PictureUrl,
                    Quantity = i.Quantity
                }).ToList()
            };
        }


    }
}

