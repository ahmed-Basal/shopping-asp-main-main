using core.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace inftastructer.Data.Configration
{
    public class CategoryConfigration : Microsoft.EntityFrameworkCore.IEntityTypeConfiguration<core.Entities.category>
    {
     
             public void Configure(EntityTypeBuilder<category> builder)
        {
            builder.Property(x => x.name).IsRequired().HasMaxLength(30);
            builder.Property(x => x.Id).IsRequired();
            builder.HasData(
                new category { Id = 1, name = "test", description = "test" }
                );
        }
    }
    }
