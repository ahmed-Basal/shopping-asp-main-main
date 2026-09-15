using core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace core.Dto
{
    public  record  orderDto
    {
        public int deliveryMethodId { get; set; }   
        public string  basketId { get; set; }
        public ShipaddressDto shipaddressDto {  get; set; }

    }
    public record ShipaddressDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zipcode { get; set; }
    }
}
