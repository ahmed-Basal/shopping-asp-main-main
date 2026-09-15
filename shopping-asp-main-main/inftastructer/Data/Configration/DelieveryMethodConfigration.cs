using core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace inftastructer.Data.Configration
{
    public class DelieveryMethodConfigration : Microsoft.EntityFrameworkCore.IEntityTypeConfiguration<core.Entities.DeliveryMethod>
    {
        public void Configure(EntityTypeBuilder<DeliveryMethod> builder)
        {
           builder.Property(x=>x.Price).HasColumnType("decimal(18,2)");
            builder.HasData(new DeliveryMethod
            {
                Id = 1,
                DeliveryTime = "only a week",
                Description = "the first fast delievey",
                Name = "Dhl",
                Price = 15
            });

            builder.HasData(new DeliveryMethod
            {
                Id = 2,
                DeliveryTime = "only a  tweo week",
                Description = "Makeyour product save",
                Name = "xxx",
                Price = 121
            });
        }
    }
}
