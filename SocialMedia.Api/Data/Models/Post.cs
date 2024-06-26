﻿


using SocialMedia.Api.Data.Models.Authentication;

namespace SocialMedia.Api.Data.Models
{
    public class Post
    {
        public string Id { get; set; } = null!;
        public string UserId { get; set; } = null!;
        public string PostPolicyId { get; set; } = null!;
        public string ReactPolicyId { get; set; } = null!;
        public string CommentPolicyId { get; set; } = null!;
        public string Content { get; set; } = null!;
        public DateTime PostedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public SiteUser? User { get; set; }
        public Policy? PostPolicy { get; set; }
        public Policy? ReactPolicy { get; set; }
        public Policy? CommentPolicy { get; set; }
        public List<PostImages>? PostImages { get; set; }
        public List<PostView>? PostViews { get; set; }
        public List<SavedPosts>? SavedPosts { get; set; }
        public List<PostReacts>? PostReacts { get; set; }
        public List<PostComment>? PostComments { get; set; }
        public List<PagePost>? PagePosts { get; set; }
        public List<GroupPost>? GroupPosts { get; set; }
    }
}
