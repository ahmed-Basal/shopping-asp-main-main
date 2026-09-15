using System;
using System.Collections.Generic;
using System.Text;

namespace core.interfaces
{
    public  interface IPaymentService
    {
        Task<string> CreateOrUpdatePaymentIntent(string userId, int deliveryMethodId);
    }
}
