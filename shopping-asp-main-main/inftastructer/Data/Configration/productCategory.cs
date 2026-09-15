using core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace inftastructer.Data.Configration
{
    public class productCategory : Microsoft.EntityFrameworkCore.IEntityTypeConfiguration<core.Entities.product>
    {
        public void Configure(EntityTypeBuilder<product> builder)
        {
            builder.Property(x => x.Name).IsRequired();
            builder.Property(x => x.Description).IsRequired();
          builder.Property(x=>x.oldPrice).HasColumnType("decimal(18,2)");
            builder.Property(x => x.NewPrice).HasColumnType("decimal(18,2)");
            builder.HasData(
                new product { Id = 1, Name = "test", Description = "test", CategoryId = 1, NewPrice = 20.5m });
        }
    }
}
