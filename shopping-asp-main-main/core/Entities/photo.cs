using System;
using System.Collections.Generic;
using System.Text;

namespace core.Entities
{
    public  class photo:BaseEntities<int>
    {
        public string iamgename { get; set; }
        public int productId { get; set; }
        [System.ComponentModel.DataAnnotations.Schema.ForeignKey("productId")]
      public virtual product product { get; set; }
    }
}
