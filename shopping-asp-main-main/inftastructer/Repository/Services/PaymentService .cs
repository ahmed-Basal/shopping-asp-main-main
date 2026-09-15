using core.Entities;
using core.interfaces;
using Stripe;
using System;
using System.Collections.Generic;
using System.Text;

namespace inftastructer.Repository.Services
{
    public class PaymentService : IPaymentService
  
    {
        private readonly ICustomerBasketService _customerBasket;
        private readonly IGenricRepo<DeliveryMethod> _deliveryRepo;

        public PaymentService(ICustomerBasketService customerBasket,
                              IGenricRepo<DeliveryMethod> deliveryRepo)
        {
            _customerBasket = customerBasket;
            _deliveryRepo = deliveryRepo;
        }

        public async Task<string> CreateOrUpdatePaymentIntent(string userId, int deliveryMethodId)
        {
            // جلب سلة المشتريات
            var cart = await _customerBasket.GetBasketAsync(userId);
            if (cart == null) throw new Exception("Cart not found");

            // جلب طريقة التوصيل
            var deliveryMethod = await _deliveryRepo.GetByIdAsync(deliveryMethodId);
            if (deliveryMethod == null) throw new Exception("Delivery method not found");

         
            var subtotal = cart.Items.Sum(i => i.Price * i.Quantity);
            var total = subtotal + deliveryMethod.Price;

            // إعداد Stripe
            StripeConfiguration.ApiKey = "YOUR_SECRET_KEY";

            var service = new PaymentIntentService();

            // إعداد PaymentIntent
            var options = new PaymentIntentCreateOptions
            {
                Amount = (long)(total * 100), // بالدولار سنت
                Currency = "usd",
                PaymentMethodTypes = new List<string> { "card" }
            };

            var intent = await service.CreateAsync(options);

            // **لا تخزن ClientSecret في الكارت**
            // إذا احتجت لتتبع الدفع لاحقًا ممكن تخزن intent.Id

            // إرجاع ClientSecret مباشرة للـ Frontend
            return intent.ClientSecret;
        }
    } 
}

