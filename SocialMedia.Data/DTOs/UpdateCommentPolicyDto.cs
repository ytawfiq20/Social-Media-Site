﻿

using System.ComponentModel.DataAnnotations;

namespace SocialMedia.Data.DTOs
{
    public class UpdateCommentPolicyDto
    {
        [Required]
        public string Id { get; set; } = null!;

        [Required]
        public string PolicyIdOrName { get; set; } = null!;
    }
}