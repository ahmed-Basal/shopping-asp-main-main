using AutoMapper;
using core.Dto;
using core.Entities;
using core.interfaces;
using core.Services;
using inftastructer.Data;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace inftastructer.Repository.Services
{
    public class OrderServices 
    {
        //    private readonly IUnitOfWork _unitOfWork;
        //    private readonly AppDbContext _context;
        //    private readonly IMapper _mapper;

        //    public OrderServices(IUnitOfWork unitOfWork, AppDbContext context, IMapper mapper)
        //    {
        //        _unitOfWork = unitOfWork;
        //        _context = context;
        //        _mapper = mapper;
        //    }

        //    // إنشاء طلب جديد
        //    public async Task<Order> CreateOrderAsync(orderDto orderDTO, string buyerEmail)
        //    {
        //        // جلب السلة
        //        var basket = await _unitOfWork.CustomerBasketRepository.GetBasketAsync(orderDTO.basketId);//.basketyRepository.GetBasketAsync(orderDTO.basketId);
        //        if (basket == null || basket.basketItems == null || !basket.basketItems.Any())
        //            throw new Exception("Basket is empty or not found");

        //        var orderItems = new List<orderitem>();

        //        foreach (var item in basket.basketItems)
        //        {
        //            // جلب المنتج من الـ ProductId
        //            var product = await _unitOfWork.ProductRepository.GetByIdAsync(item.ProductId);
        //            if (product == null)
        //                throw new Exception($"Product with id {item.ProductId} not found");

        //            var orderItem = new orderitem(
        //                product.Id,
        //                product.photos,   
        //                product.Name,       
        //                product.NewPrice,    
        //                item.Quantity        
        //            );

        //            orderItems.Add(orderItem);
        //        }

        //        if (!orderItems.Any())
        //            throw new Exception("No valid products found in basket");

        //        // جلب طريقة التوصيل
        //        var deliveryMethod = await _context.deliveryMethods
        //            .FirstOrDefaultAsync(m => m.Id == orderDTO.deliveryMethodId);
        //        if (deliveryMethod == null)
        //            throw new Exception("Delivery method not found");

        //        // حساب المجموع
        //        var subTotal = orderItems.Sum(i => i.Price * i.Quantity);

        //        // عمل mapping للعنوان
        //        var shipAddress = orderDTO.shipaddressDto != null
        //            ? _mapper.Map<shoppingAddress>(orderDTO.shipaddressDto)
        //            : null;

        //        // إنشاء الطلب
        //        var order = new Order(
        //            buyerEmail,
        //            subTotal,
        //            shipAddress,
        //            deliveryMethod,
        //            orderItems,
        //            basket.PaymentIntentId
        //        );

        //        await _context.orders.AddAsync(order);
        //        await _context.SaveChangesAsync();

        //        return order;
        //    }

        //    // تحديث طلب موجود
        //    public async Task<Order> UpdateOrderAsync(int orderId, orderDto orderDTO)
        //    {
        //        var order = await _context.orders
        //            .Include(o => o.OrderItems)
        //            .Include(o => o.DeliveryMethod)
        //            .FirstOrDefaultAsync(o => o.Id == orderId);

        //        if (order == null)
        //            throw new Exception("Order not found");

        //        // تحديث طريقة التوصيل إذا اتغيرت
        //        if (orderDTO.deliveryMethodId != order.DeliveryMethod.Id)
        //        {
        //            var deliveryMethod = await _context.deliveryMethods
        //                .FirstOrDefaultAsync(m => m.Id == orderDTO.deliveryMethodId);
        //            if (deliveryMethod == null)
        //                throw new Exception("Delivery method not found");

        //            order.DeliveryMethod = deliveryMethod;
        //        }

        //        // تحديث عنوان الشحن
        //        if (orderDTO.shipaddressDto != null)
        //        {
        //            order.Address = _mapper.Map<shoppingAddress>(orderDTO.shipaddressDto);
        //        }

        //        // إعادة حساب المجموع
        //        order.Subtotal = order.OrderItems.Sum(i => i.Price * i.Quantity);

        //        await _context.SaveChangesAsync();
        //        return order;
        //    }

        //    // جلب كل طرق التوصيل
        //    public async Task<IReadOnlyList<DeliveryMethod>> GetDeliveryMethodsAsync()
        //    {
        //        return await _context.deliveryMethods.AsNoTracking().ToListAsync();
        //    }

        //    // جلب طلب محدد حسب Id للمستخدم
        //    public async Task<orderToReturn> GetOrderByIdAsync(int id, string buyerEmail)
        //    {
        //        var order = await _context.orders
        //            .Where(o => o.Id == id && o.BuyerEmail == buyerEmail)
        //            .Include(o => o.DeliveryMethod)
        //            .Include(o => o.OrderItems)
        //            .FirstOrDefaultAsync();

        //        if (order == null) return null;

        //        return _mapper.Map<orderToReturn>(order);
        //    }

        //    // جلب كل الطلبات الخاصة بالمستخدم
        //    public async Task<IReadOnlyList<orderToReturn>> GetAllOrdersForUserAsync(string buyerEmail)
        //    {
        //        var orders = await _context.orders
        //            .Where(o => o.BuyerEmail == buyerEmail)
        //            .Include(o => o.OrderItems)
        //            .Include(o => o.DeliveryMethod)
        //            .OrderByDescending(o => o.OrderDate)
        //            .AsNoTracking()
        //            .ToListAsync();

        //        return _mapper.Map<IReadOnlyList<orderToReturn>>(orders);
        //    }
        //}
    }

}