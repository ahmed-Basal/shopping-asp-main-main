using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Xml.Linq;

namespace core.Entities
{
    public  class comment
    {
        public int Id { get; set; } 
        public string Content { get; set; }

        public int ProductId { get; set; }
        public product Product { get; set; }

        public string UserId { get; set; }
        public AppUser User { get; set; }

        public int? ParentCommentId { get; set; }
        public comment ParentComment { get; set; }

        public ICollection<comment> Replies { get; set; } = new List<comment>();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
