using core.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace inftastructer.Data.Configration
{
    public class Photoconfigration : IEntityTypeConfiguration<photo>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<photo> builder)
        {
            builder.HasData(
                new photo
                {
                    Id = 1,
                    iamgename = "image1.jpg",
                    productId = 1
                }
            );
        }
    }
}
