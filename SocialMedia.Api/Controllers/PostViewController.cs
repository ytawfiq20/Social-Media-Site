﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SocialMedia.Api.Data.Models;
using SocialMedia.Api.Data.Models.ApiResponseModel;
using SocialMedia.Api.Repository.PostRepository;
using SocialMedia.Api.Repository.PostViewRepository;
using SocialMedia.Api.Service.GenericReturn;

namespace SocialMedia.Api.Controllers
{
    [ApiController]
    public class PostViewController : ControllerBase
    {
        private readonly IPostViewRepository _postViewRepository;
        private readonly IPostRepository _postRepository;
        public PostViewController(IPostViewRepository _postViewRepository, IPostRepository _postRepository)
        {
            this._postViewRepository = _postViewRepository;
            this._postRepository = _postRepository;
        }

        [HttpGet("postView/{postId}")]
        public async Task<IActionResult> GetPostViewsAsync([FromRoute] string postId)
        {
            try
            {
                var post = await _postRepository.GetPostByIdAsync(postId);
                if (post != null)
                {
                    var postView = await _postViewRepository.GetPostViewByPostIdAsync(postId);
                    return StatusCode(StatusCodes.Status200OK, StatusCodeReturn<PostView>
                        ._200_Success("Post views found successfully", postView));
                }
                return StatusCode(StatusCodes.Status404NotFound, StatusCodeReturn<string>
                    ._404_NotFound("Post not found"));
            }
            catch(Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, StatusCodeReturn<string>
                    ._500_ServerError(ex.Message));
            }
        }


    }
}
