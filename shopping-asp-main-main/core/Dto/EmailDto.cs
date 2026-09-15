using System;
using System.Collections.Generic;
using System.Text;

namespace core.Dto
{
    public  class EmailDto
    {


        public EmailDto() { }
        public EmailDto(string to, string from, string subject, string content)
        {
            this.to = to;
            this.from = from;
            this.subject = subject;
            this.content = content;
        }

        public string to { get; set; }  
        public string from { get; set; }
        public string subject { get; set; }
        public string content { get; set; }
       
    }
}
