using System.ComponentModel.DataAnnotations.Schema;

namespace core.Entities
{
    public class Address: BaseEntities<int>
    {
        public string firstname { get; set; }
        public string Lastname { get; set; }
        public string City{ get; set; }
        public string ZipCode { get; set; }
        public string street { get; set; }
        public string state { get; set; }
         public string AppuserId { get; set; }
        [ForeignKey("AppuserId")]
        public virtual AppUser AppUser { get; set; }
    }
}