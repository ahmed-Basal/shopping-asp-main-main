using core.Dto;
using core.Entities;
using core.shareing;
using System;
using System.Collections.Generic;
using System.Text;

namespace core.interfaces
{
    public  interface  IProductRepository:IGenricRepo<core.Entities.product>
    {
        Task<IEnumerable<ProductDto>> GetAll(ProductParameters productparameter);
      
        Task<bool> add(AddproductDto productDto);
        Task<bool> update(int id, updateproductDto updateproductDto);
        Task  delete(product product);
        Task<product> GetByNameAsync(string name);
    }
}
