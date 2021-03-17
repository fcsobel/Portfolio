using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Portfolio.Data.Model;

namespace Portfolio.Data.Context.Mapping
{
    public class UserAccessMap : IEntityTypeConfiguration<UserAccess>
    {
        public void Configure(EntityTypeBuilder<UserAccess> builder)
        {
            builder.ToTable("UserAccess");

            builder.HasKey(c =>new { c.UserId, c.RoleId });

            builder.HasOne(x => x.User)
                .WithMany(x => x.UserAccess)
                .HasForeignKey(x => x.UserId);

            builder.HasOne(x => x.Role)
               .WithMany(x => x.UserAccess)
               .HasForeignKey(x => x.RoleId);
        }
    }
}

        