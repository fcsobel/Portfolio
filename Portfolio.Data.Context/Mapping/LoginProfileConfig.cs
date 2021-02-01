using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Portfolio.Data.Model;

namespace Portfolio.Data.Context.Mapping
{
    public class LoginProfileConfig : IEntityTypeConfiguration<LoginProfile>
    {
        public void Configure(EntityTypeBuilder<LoginProfile> builder)
        {
            builder.ToTable("LoginProfiles");
            builder.HasKey(c => new { c.UserId, c.ProviderName } );
            builder.Property(c => c.ProviderName).HasMaxLength(10);
            builder.Property(c => c.UserName).HasMaxLength(50);
            builder.Property(c => c.Url).HasMaxLength(1000);
            builder.Property(c => c.Picture).HasMaxLength(1000);
        }
    }
}

        