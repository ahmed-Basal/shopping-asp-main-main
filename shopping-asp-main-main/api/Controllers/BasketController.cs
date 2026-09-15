using api.halper;
using AutoMapper;
using core.Dto;
using core.Entities;
using core.interfaces;
using core.Services;
using inftastructer.Repository.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BasketsController : ControllerBase
    {
        private readonly ICustomerBasketService _basketService;
        private readonly IUnitOfWork unitOfWork;
        private string GetUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
        public BasketsController(ICustomerBasketService basketService, IUnitOfWork unitOfWork)
        {
            _basketService = basketService;
            this.unitOfWork = unitOfWork;
        }


        [HttpGet("get-basket")]


        public async Task<ActionResult<CustomerBasketDto>> GetBasket()
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();
            var basket = await _basketService.GetBasketAsync(userId);
            // Manual Mapping
            var basketDto = new CustomerBasketDto
            {
                Id = basket.Id,
                Items = basket.Items.Select(i => new BasketItemDto
                {
                    Name = i.Name,
                    Price = i.Price,
                    PictureUrl = i.PictureUrl,
                    Quantity = i.Quantity
                }).ToList()
            };

            return Ok(basketDto);
        }

        [HttpPost("create-cart")]
        public async Task<ActionResult<CustomerBasketDto>> CreateBasket(List<AddToBasketDto> items)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            var basket = await _basketService.CreateBasketAsync(userId, items);

            return Ok(basket);
        }

      
      
      
        [HttpPut("update-item")]
        public async Task<ActionResult<CustomerBasketDto>> UpdateItem(AddToBasketDto dto)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            var basket = await _basketService.AddOrUpdateItemAsync(userId, dto);

            return Ok(basket);
        }

       
        [HttpDelete("delete-cart")]
        public async Task<IActionResult> DeleteBasket()
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            var deleted = await _basketService.DeleteBasketAsync(userId);

            if (!deleted)
                return NotFound("Basket not found");

            return NoContent();
        }
    }
}