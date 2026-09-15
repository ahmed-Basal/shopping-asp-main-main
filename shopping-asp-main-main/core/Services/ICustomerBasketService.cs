using core.Dto;

namespace inftastructer.Repository.Services
{
    public interface ICustomerBasketService
    {
        Task<CustomerBasketDto> AddOrUpdateItemAsync(string userId, AddToBasketDto dto);
        Task<CustomerBasketDto> CreateBasketAsync(string userId, List<AddToBasketDto> itemsDto);
        Task<bool> DeleteBasketAsync(string userId);
        Task<CustomerBasketDto> GetBasketAsync(string userId);

        Task<CustomerBasketDto> UpdateItemQuantityAsync(string userId, AddToBasketDto dto);
    }
}