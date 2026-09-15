using AutoMapper;
using core.Dto;
using core.Entities;
using core.Services;
using core.shareing;
using inftastructer.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.IO;

namespace inftastructer.Repository
{
    internal class ProductRepository : GenericRepositories<core.Entities.product>, core.interfaces.IProductRepository
    {
        private readonly AppDbContext context;
        private readonly IMapper mapper;
        private readonly IIamgeServices iamgeServices;

        public ProductRepository(AppDbContext context, IMapper mapper, IIamgeServices iamgeServices) : base(context)
        {
            this.context = context;
            this.mapper = mapper;
            this.iamgeServices = iamgeServices;
        }

        public async Task<bool> add(AddproductDto productDTO)
        {
            if (productDTO == null) return false;

            var product = mapper.Map<product>(productDTO);



            if (productDTO.Photo != null && productDTO.Photo.Any())
            {
                product.photos = new List<photo>();

                foreach (var file in productDTO.Photo)
                {
                    var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                    var path = Path.Combine("wwwroot/images", fileName);

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    product.photos.Add(new photo
                    {
                        iamgename = fileName
                    });
                }
            }

            await context.Products.AddAsync(product);
            await context.SaveChangesAsync();

            return true;
        }

        public async Task  delete(product product)
        {
           var photo= context.Photos.Where(m => m.productId == product.Id).ToList();
            foreach (var item in photo)
            {
                context.Photos.Remove(item);
            }
            context.Photos.RemoveRange(photo);
            context.Products.Remove(product);
            await  context.SaveChangesAsync();
        }

        public async Task<IEnumerable<ProductDto>> GetAll(ProductParameters productparameter)
        {
            var query = context.Products
                .Include(p => p.category)
                .Include(p => p.photos)
                .AsNoTracking()
                .AsQueryable();

            if(!string.IsNullOrEmpty(productparameter.Search))
            {
                var searchWords = productparameter.Search.Split(' ');
                query = query.Where(m => searchWords.All(word =>
                    m.Name.ToLower().Contains(word.ToLower()) ||
                    m.Description.ToLower().Contains(word.ToLower())
                ));
            }


            if (!string.IsNullOrWhiteSpace(productparameter.Sort))
            {
                query = productparameter.Sort.ToLower() switch
                {
                    "priceasc" => query.OrderBy(p => p.NewPrice),
                    "pricedesc" => query.OrderByDescending(p => p.NewPrice),
                    _ => query.OrderBy(p => p.Name) 
                };
            }
            else
            {
                query = query.OrderBy(p => p.Name); 
            }

            query = query.Skip((productparameter.PageNumber - 1) * productparameter.PageSize).Take(productparameter.PageSize);

            var products = await query.ToListAsync();

            return mapper.Map<IEnumerable<ProductDto>>(products);
        }



        public async Task<bool> UpdateAsync(int id, updateproductDto dto)
        {
            if (dto == null) return false;

            var product = await context.Products
                .Include(p => p.category)   // تأكد أن اسم الـ navigation property صحيح
                .Include(p => p.photos)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null) return false;

            // تحديث البيانات الأساسية
            if (!string.IsNullOrEmpty(dto.Name))
                product.Name = dto.Name;

            if (!string.IsNullOrEmpty(dto.Description))
                product.Description = dto.Description;

            if (dto.NewPrice.HasValue)
                product.NewPrice = dto.NewPrice.Value;

            if (dto.CategoryId.HasValue)
                product.CategoryId = dto.CategoryId.Value;

            // تحديث الصورة إذا تم تمرير صورة جديدة
            if (dto.Photo != null)
            {
                // حذف أي صور قديمة
                if (product.photos != null && product.photos.Any())
                    context.Photos.RemoveRange(product.photos);

                // رفع الصورة الجديدة والحصول على مسارها
                var imagePaths = await iamgeServices.AddImageAsync(dto.Photo, dto.Name ?? "product");
                var imagePath = imagePaths?.FirstOrDefault();

                // إنشاء photo جديد مرتبط بالمنتج
                var newPhoto = new photo
                {
                    iamgename = imagePath ?? string.Empty,   // مجرد صورة واحدة
                    productId = product.Id
                };

                await context.Photos.AddAsync(newPhoto);
            }


            await context.SaveChangesAsync();
            return true;
        }

        // Implement interface method 'update' expected by IProductRepository
        public Task<bool> update(int id, updateproductDto dto)
        {
            return UpdateAsync(id, dto);
        }

        public async Task<product> GetByNameAsync(string name)
        {
            return await context.Products
                           .FirstOrDefaultAsync(p => p.Name == name);
        }
    }
    }

