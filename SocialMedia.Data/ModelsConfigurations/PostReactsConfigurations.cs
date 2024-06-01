﻿

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SocialMedia.Data.Models;

namespace SocialMedia.Data.ModelsConfigurations
{
    public class PostReactsConfigurations : IEntityTypeConfiguration<PostReacts>
    {
        public void Configure(EntityTypeBuilder<PostReacts> builder)
        {
            builder.HasKey(e => e.Id);
            builder.HasOne(e => e.Post).WithMany(e => e.PostReacts).HasForeignKey(e => e.PostId);
            builder.HasOne(e => e.React).WithMany(e => e.PostReacts).HasForeignKey(e => e.SpecialPostReactId);
            builder.HasOne(e => e.User).WithMany(e => e.PostReacts).HasForeignKey(e => e.UserId);
            builder.Property(e => e.UserId).IsRequired().HasColumnName("User Id");
            builder.Property(e => e.SpecialPostReactId).IsRequired().HasColumnName("Post React Id");
            builder.Property(e => e.PostId).IsRequired().HasColumnName("Post Id");
            builder.HasIndex(e => new { e.PostId, e.UserId }).IsUnique();
        }
    }
}