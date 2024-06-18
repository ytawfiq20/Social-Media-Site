﻿

using SocialMedia.Api.Data.Models.Authentication;

namespace SocialMedia.Api.Data.Models
{
    public class UserChat
    {
        public string Id { get; set; } = null!;
        public string User1Id { get; set; } = null!;
        public string User2Id { get; set; } = null!;
        public SiteUser? User1 { get; set; }
        public SiteUser? User2 { get; set; }
        public List<ChatMessage>? ChatMessages { get; set; }
        public List<ArchievedChat>? ArchievedChats { get; set; }
    }
}