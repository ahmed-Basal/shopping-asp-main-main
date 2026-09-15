using core.Entities;
using core.interfaces;
using inftastructer.Data;
using inftastructer.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Stripe;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;

namespace core.Services
{
    public class PaymentServices : IPaymentServices
    {
        private readonly  IUnitOfWork unitOfWork;
        private readonly IConfiguration configuration;
        private readonly AppDbContext _context;
        public PaymentServices(IUnitOfWork unitOfWork, IConfiguration configuration, AppDbContext context)
        {
            this.configuration = configuration;
            this.unitOfWork = unitOfWork;
            _context = context;
        }
        public async Task<CustomerBasket?> CreateOrUpdatePaymentIntent(string basketId, int? deliveryMethodId)
        {
            var basket = await unitOfWork.CustomerBasketRepository.GetBasketAsync(basketId);//basketyRepository.GetBasketAsync(basketId);
            if (basket == null || basket.basketItems == null || !basket.basketItems.Any())
                return null;

            var stripeKey = configuration["StripeSettings:SecretKey"];
            if (string.IsNullOrEmpty(stripeKey))
                throw new InvalidOperationException("Stripe API key is not configured.");

            var stripeClient = new Stripe.StripeClient(stripeKey);
            var paymentIntentService = new Stripe.PaymentIntentService(stripeClient);

            // حساب تكلفة الشحن
            decimal shippingPrice = 0m;
            if (deliveryMethodId.HasValue)
            {
                var delivery = await _context.deliveryMethods
                    .AsNoTracking()
                    .FirstOrDefaultAsync(d => d.Id == deliveryMethodId.Value);

                if (delivery != null)
                    shippingPrice = delivery.Price;
            }

            // حساب subtotal من المنتجات مباشرة بدون تعديل BasketItem
            decimal subtotal = 0m;
            foreach (var item in basket.basketItems)
            {
                var product = await unitOfWork.ProductRepository.GetByIdAsync(item.ProductId);
                if (product == null)
                    throw new Exception($"Product with id {item.ProductId} not found.");

                subtotal += product.NewPrice * item.Quantity;
            }

            var amount = (long)((subtotal + shippingPrice) * 100); // بالـ cents لـ Stripe

            // إنشاء أو تحديث PaymentIntent
            if (string.IsNullOrEmpty(basket.PaymentIntentId))
            {
                var createOptions = new Stripe.PaymentIntentCreateOptions
                {
                    Amount = amount,
                    Currency = "usd",
                    PaymentMethodTypes = new List<string> { "card" }
                };

                var intent = await paymentIntentService.CreateAsync(createOptions);
                basket.PaymentIntentId = intent.Id;
                basket.ClientSecret = intent.ClientSecret;
            }
            else
            {
                var updateOptions = new Stripe.PaymentIntentUpdateOptions
                {
                    Amount = amount
                };

                try
                {
                    await paymentIntentService.UpdateAsync(basket.PaymentIntentId, updateOptions);
                }
                catch (Stripe.StripeException ex) when (ex.StripeError.Code == "resource_missing")
                {
                    // إعادة إنشاء الـ PaymentIntent إذا مفقود
                    var createOptions = new Stripe.PaymentIntentCreateOptions
                    {
                        Amount = amount,
                        Currency = "usd",
                        PaymentMethodTypes = new List<string> { "card" }
                    };

                    var intent = await paymentIntentService.CreateAsync(createOptions);
                    basket.PaymentIntentId = intent.Id;
                    basket.ClientSecret = intent.ClientSecret;
                }
            }


            await unitOfWork.CustomerBasketRepository.UpdateBasketAsync(basket);//.basketyRepository.UpdateBasketAsync(basket);

            return basket;
        }
    }
}
