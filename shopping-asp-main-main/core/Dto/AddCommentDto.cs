using System;
using System.Collections.Generic;
using System.Text;

namespace core.Dto
{
    public  class AddCommentDto
    {
        public string Content { get; set; }

        public int ProductId { get; set; }

        public int? ParentCommentId { get; set; }
    }
}
