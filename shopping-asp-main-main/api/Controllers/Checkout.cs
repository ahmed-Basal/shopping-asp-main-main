using core.interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class Checkout : ControllerBase
    {
        private readonly IPaymentServices paymentServices;
        public Checkout(IPaymentServices paymentServices)
        {
            this.paymentServices = paymentServices;
        }
        [HttpPost("Create/{delieveredmethodid}")]
        public async Task<ActionResult> CreateOrUpdatePaymentIntent(string basketId, int? delieveredmethodid)
        {
            var intent = await paymentServices.CreateOrUpdatePaymentIntent(basketId, delieveredmethodid);
            if (intent == null) return BadRequest(new ProblemDetails { Title = "problem with your basket" });
            return Ok(intent);
        }
    }
}
