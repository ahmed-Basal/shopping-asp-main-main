using AutoMapper;
using core.Entities;
using core.interfaces;
using core.Services;
using inftastructer.Data;
using inftastructer.Repository.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace inftastructer.Repository
{
    public class UnitOfWork : core.interfaces.IUnitOfWork
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly IIamgeServices _imageManagementService;
        private readonly IConnectionMultiplexer redis;
        private readonly UserManager<AppUser    > _userManager;
        private readonly IEmailServices _emailSender;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ITokenGenerate tokenGenerate;
        private readonly IConfiguration configuration;
        public ICategoryRepository CategoryRepository { get; }
        public IPhotoRepository PhotoRepository { get; }
        public IProductRepository ProductRepository { get; }


         public IAuth AuthRepository
        { get; }
        public IBasketRepository CustomerBasketRepository { get; }
        public IAccountRepository accountRepository { get; }
       


        public UnitOfWork(AppDbContext context, IMapper mapper, IIamgeServices imageManagementService, IConnectionMultiplexer redis,UserManager<AppUser> userManager,IEmailServices emailServices,SignInManager<AppUser> signInManager
            ,ITokenGenerate tokenGenerate,IConfiguration configuration)
        {
            _context = context;
            _mapper = mapper;
            _imageManagementService = imageManagementService;
            this.redis = redis;
            this._userManager = userManager;
            this._emailSender = emailServices;
            this._signInManager = signInManager;
            this.tokenGenerate = tokenGenerate;
            CategoryRepository = new CategoryRepository(_context);
            PhotoRepository = new Photprepository(_context);
            ProductRepository = new ProductRepository(_context, _mapper, _imageManagementService);
            CustomerBasketRepository = new CustomerBasketRepository(redis);
            AuthRepository = new AuthRepository(_userManager, emailServices, signInManager, tokenGenerate, configuration);
            accountRepository = new AccountRepository(_userManager);
           
        }
    }

}
