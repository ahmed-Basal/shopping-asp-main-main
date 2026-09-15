using System;
using System.Collections.Generic;
using System.Text;

namespace core.Entities
{
    public  class category : BaseEntities<int>
    {
        public string name { get; set; }
        public string description { get; set; }
       public virtual ICollection<product> products { get; set; }=new HashSet<product>();
    }
}
