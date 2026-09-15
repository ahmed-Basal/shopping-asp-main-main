using inftastructer.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace inftastructer.Repository
{
    public class CategoryRepository : GenericRepositories<core.Entities.category>, core.interfaces.ICategoryRepository
    {
        public CategoryRepository(AppDbContext context) : base(context)
        {
        }

        
    }
}
