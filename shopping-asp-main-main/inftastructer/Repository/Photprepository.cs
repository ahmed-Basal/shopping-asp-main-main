using inftastructer.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace inftastructer.Repository
{
    public class Photprepository : GenericRepositories<core.Entities.photo>, core.interfaces.IPhotoRepository
    {
        public Photprepository(AppDbContext context) : base(context)
        {
        }

       
    }
}
