using System;
using System.Collections.Generic;
using System.Text;

namespace core.interfaces
{
    public  interface  IUnitOfWork
    {
        public ICategoryRepository CategoryRepository { get; }
        public IPhotoRepository PhotoRepository { get; }
        public IProductRepository ProductRepository { get; }
        public IBasketRepository CustomerBasketRepository { get; }
        public IAuth AuthRepository { get; }
        public IAccountRepository accountRepository { get; }
    }
}
