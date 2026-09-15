using api.halper;
using AutoMapper;
using core.Dto;
using core.Entities;
using core.interfaces;
using inftastructer.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
   
        public class CategoriesController : BaseController
        {
            public CategoriesController(IUnitOfWork work, IMapper mapper) : base(work, mapper)
            {
            }

            [HttpGet("get-all")]
            public async Task<IActionResult> get()
            {
                try
                {
                    var category = await work.CategoryRepository.GetAllAsync();
                    if (category is null)
                        return BadRequest(new ResponseAPI(400,"failed"));
                    return Ok(category);
                }
                catch (Exception ex)
                {

                    return BadRequest(ex.Message);
                }
            }
            [HttpGet("get-by-id/{id}")]
            public async Task<IActionResult> getbyId(int id)
            {
                try
                {
                    var category = await work.CategoryRepository.GetByIdAsync(id);
                    if (category is null) return BadRequest(new ResponseAPI(400, $"not found category id={id}"));
                    return Ok(category);
                }
                catch (Exception ex)
                {

                    return BadRequest(ex.Message);
                }
            }
            [HttpPost("add-category")]
            public async Task<IActionResult> add(CategoryDto categoryDTO)
            {
                try
                {
                    var category = mapper.Map<category>(categoryDTO);
                    await work.CategoryRepository.AddAsync(category);
                    return Ok(new ResponseAPI(200, "Item has been added"));
                }
                catch (Exception ex)
                {

                    return BadRequest(ex.Message);
                }
            }
            [HttpPut("update-category")]
            public async Task<IActionResult> update(updateRecordCategory categoryDTO)
            {
                try
                {
                    var category = mapper.Map<category>(categoryDTO);
                    await work.CategoryRepository.UpdateAsync(category);
                    return Ok(new ResponseAPI(200, "Item has been updated"));
                }
                catch (Exception ex)
                {

                    return BadRequest(ex.Message);
                }
            }
            [HttpDelete("delete-category/{id}")]
            public async Task<IActionResult> delete(int id)
            {
                try
                {
                    await work.CategoryRepository.DeleteAsync(id);
                    return Ok(new ResponseAPI(200, "item has been deleted"));
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
        }
    } 
