﻿

using Microsoft.AspNetCore.Identity;

namespace SocialMedia.Data.Models.Authentication
{
    public class SiteUser : IdentityUser
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string DisplayName { get; set; } = null!;
        public string? RefreshToken { get; set; } = null!;
        public DateTime? RefreshTokenExpiry { get; set; }
        public bool IsFriendListPrivate { get; set; }
        public List<FriendRequest>? FriendRequests { get; set; }
        public List<Friend>? Friends { get; set; }
        public List<Follower>? Followers { get; set; }
    }
}
