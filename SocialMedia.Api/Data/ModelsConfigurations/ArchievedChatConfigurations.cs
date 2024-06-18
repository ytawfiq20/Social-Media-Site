﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SocialMedia.Api.Data.Models;


namespace SocialMedia.Api.Data.ModelsConfigurations
{
    public class ArchievedChatConfigurations : IEntityTypeConfiguration<ArchievedChat>
    {
        public void Configure(EntityTypeBuilder<ArchievedChat> builder)
        {
            builder.HasKey(e => e.Id);
            builder.HasOne(e => e.UserChat).WithMany(e => e.ArchievedChats).HasForeignKey(e => e.ChatId)
                .OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(e => e.User).WithMany(e => e.ArchievedChats).HasForeignKey(e => e.UserId);
            builder.Property(e => e.UserId).IsRequired().HasColumnName("User Id");
            builder.Property(e => e.ChatId).IsRequired().HasColumnName("Chat Id");
            builder.HasIndex(e => new { e.ChatId, e.UserId }).IsUnique();
        }
    }
}