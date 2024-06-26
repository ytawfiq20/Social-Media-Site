﻿
namespace SocialMedia.Api.Service.GenericReturn
{
    public class Policies
    {

        public List<string> PostPolicies { get; private set; } = new List<string> 
        {
            "PUBLIC", "PRIVATE", "FRIENDS ONLY", "FRIENDS OF FRIENDS"
        };

        public List<string> BasicPolicies { get; private set; } = new List<string>
        {
            "PUBLIC", "PRIVATE", "FRIENDS ONLY", "FRIENDS OF FRIENDS"
        };

        public List<string> ReactPolicies { get; private set; } = new List<string>
        {
            "PUBLIC", "PRIVATE", "FRIENDS ONLY", "FRIENDS OF FRIENDS"
        };

        public List<string> CommentPolicies { get; private set; } = new List<string>
        {
            "PUBLIC", "PRIVATE", "FRIENDS ONLY", "FRIENDS OF FRIENDS"
        };

        public List<string> AccountPolicies { get; private set; } = new List<string>
        {
            "PUBLIC", "PRIVATE"
        };

        public List<string> SarehneMessagePolicies { get; private set; } = new List<string>
        {
            "PUBLIC", "PRIVATE"
        };

        public List<string> AccountPostPolicies { get; private set; } = new List<string>
        {
            "PUBLIC", "PRIVATE", "FRIENDS ONLY", "FRIENDS OF FRIENDS"
        };

        public List<string> FriendListPolicies { get; private set; } = new List<string>
        {
            "PUBLIC", "PRIVATE", "FRIENDS ONLY", "FRIENDS OF FRIENDS"
        };

        public List<string> GroupPostsPolicies { get; private set; } = new List<string>
        {
            "PUBLIC", "PRIVATE"
        };


        public List<string> GroupRoles { get; private set; } = new List<string>
        {
            "USER", "ADMIN", "PUBLISHER"
        };

        public List<string> GroupChatRoles { get; private set; } = new List<string>
        {
            "USER", "ADMIN"
        };

        public List<string> Roles { get; private set; } = new List<string>
        {
            "USER", "ADMIN", "PUBLISHER"
        };


    }
}
