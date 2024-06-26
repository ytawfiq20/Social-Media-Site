﻿

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SocialMedia.Api.Data.Models;

namespace SocialMedia.Api.Data.ModelsConfigurations
{
    public class UserSavedPostsFoldersConfigurations : IEntityTypeConfiguration<UserSavedPostsFolders>
    {
        public void Configure(EntityTypeBuilder<UserSavedPostsFolders> builder)
        {
            builder.HasKey(e => e.Id);
            builder.HasOne(e => e.User).WithMany(e => e.UserSavedPostsFolders)
                .HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Restrict);
            builder.Property(e => e.UserId).IsRequired().HasColumnName("User Id");
            builder.Property(e => e.FolderName).IsRequired().HasColumnName("Folder Name")
                .HasMaxLength(30);
            builder.HasIndex(e => new { e.FolderName, e.UserId }).IsUnique();
        }
    }
}
