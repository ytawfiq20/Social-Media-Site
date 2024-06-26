﻿

using System.ComponentModel.DataAnnotations;

namespace SocialMedia.Api.Data.DTOs
{
    public class AddPostReactDto
    {
        [Required]
        public string ReactId { get; set; } = null!;

        [Required]
        public string PostId { get; set; } = null!;
    }
}
