using core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace core.interfaces
{
    public  interface  IPaymentServices
    {
        Task<CustomerBasket> CreateOrUpdatePaymentIntent(string basketId, int? delieveredmethodid);
    }
}
