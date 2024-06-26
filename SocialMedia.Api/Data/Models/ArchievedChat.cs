﻿using SocialMedia.Api.Data.Models.Authentication;

namespace SocialMedia.Api.Data.Models
{
    public class ArchievedChat
    {
        public string Id { get; set; } = null!;
        public string UserId { get; set; } = null!;
        public string ChatId { get; set; } = null!;
        public SiteUser? User { get; set; }
        public Chat? Chat { get; set; }
    }
}
