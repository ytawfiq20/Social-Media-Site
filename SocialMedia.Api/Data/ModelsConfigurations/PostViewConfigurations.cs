﻿
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SocialMedia.Api.Data.Models;

namespace SocialMedia.Api.Data.ModelsConfigurations
{
    public class PostViewConfigurations : IEntityTypeConfiguration<PostView>
    {
        public void Configure(EntityTypeBuilder<PostView> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.PostId).IsRequired().HasColumnName("Post Id");
            builder.HasOne(e => e.Post).WithMany(e => e.PostViews).HasForeignKey(e => e.PostId);
            builder.Property(e => e.ViewNumber).HasDefaultValue(0).HasColumnName("Post view number");
            builder.HasIndex(e => e.PostId).IsUnique();
        }
    }
}
