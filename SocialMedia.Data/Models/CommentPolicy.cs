﻿

using SocialMedia.Data.Models.Authentication;

namespace SocialMedia.Data.Models
{
    public class CommentPolicy
    {
        public string Id { get; set; } = null!;
        public string PolicyId { get; set; } = null!;
        public Policy? Policy { get; set; }
        public List<Post>? Posts { get; set; }
        public List<SiteUser>? Users { get; set; }
    }
}
