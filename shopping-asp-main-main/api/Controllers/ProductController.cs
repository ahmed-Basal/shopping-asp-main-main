using api.halper;
using AutoMapper;
using core.Dto;
using core.Entities;
using core.interfaces;
using core.shareing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace api.Controllers
{

    public class ProductController : BaseController
    {

        public ProductController(IUnitOfWork work, IMapper mapper) : base(work, mapper)
        {
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> get([FromQuery] ProductParameters productparameter)
        {
            try
            {
                var Product = await work.ProductRepository
                    .GetAll(productparameter); 

              var totalcount= await work.ProductRepository.ContAsync();
                var productList = Product.ToList().AsReadOnly();

                return Ok(new pagination<ProductDto>(productparameter.PageNumber,productparameter.PageSize,totalcount, productList));
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        [HttpGet("get-by-id/{id}")]
        public async Task<IActionResult> getById(int id)
        {
            try
            {


                var newproduc = await work.ProductRepository.GetByIdAsync(
    id,
    x => x.category,
    x => x.photos
);


                if (newproduc is null)
                    return BadRequest(new ResponseAPI(statusCode: 400,"failed"));

                var result = mapper.Map<ProductDto>(newproduc);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(error: new ResponseAPI(statusCode: 400, message: ex.Message));
            }
        }
        [HttpPost("Add-Product")]
        public async Task<IActionResult> add(AddproductDto productDTO)
        {
            try
            {
                await work.ProductRepository.add(productDTO);
                return Ok(new ResponseAPI(200, "item add"));
            }
            catch (Exception ex)
            {
                return BadRequest(error: new ResponseAPI(statusCode: 400, message: ex.Message));
            }
        }
        [HttpPut("update-product/{id}")]
        public async Task<IActionResult> update(int id,updateproductDto productDTO)
        {
            try
            {
                await work.ProductRepository.update(id,mapper.Map<updateproductDto>(productDTO));
                return Ok(new ResponseAPI(200, "item update"));
            }
            catch (Exception ex)
            {
                return BadRequest(error: new ResponseAPI(statusCode: 400, message: ex.Message));
            }

        }



        [HttpDelete(template: "Delete-Product/{Id}")]
        public async Task<IActionResult> delete(int Id)
        {
            try
            {
                var product = await work.ProductRepository.GetByIdAsync(
     Id,
     x => x.photos,
     x => x.category
 );


                await work.ProductRepository.delete(product);
                //productRepository.delete(product);

                return Ok(value: new ResponseAPI(statusCode: 200,"succed delated"));
            }
            catch (Exception ex)
            {
                return BadRequest(error: new ResponseAPI(statusCode: 400, message: ex.Message));
            }
        }
    }
}