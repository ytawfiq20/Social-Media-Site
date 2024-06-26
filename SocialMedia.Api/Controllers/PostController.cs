﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SocialMedia.Api.Data.DTOs;
using SocialMedia.Api.Data.Models.ApiResponseModel;
using SocialMedia.Api.Data.Models.Authentication;
using SocialMedia.Api.Repository.PostRepository;
using SocialMedia.Api.Service.BlockService;
using SocialMedia.Api.Service.GenericReturn;
using SocialMedia.Api.Service.PostService;

namespace SocialMedia.Api.Controllers
{

    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly IPostService _postService;
        private readonly UserManager<SiteUser> _userManager;
        private readonly IPostRepository _postRepository;
        private readonly IBlockService _blockService;
        private readonly UserManagerReturn _userManagerReturn;
        public PostController(IPostService _postService, UserManager<SiteUser> _userManager,
            IPostRepository _postRepository,
            IBlockService _blockService, 
             UserManagerReturn _userManagerReturn)
        {
            this._postService = _postService;
            this._userManager = _userManager;
            this._postRepository = _postRepository;
            this._blockService = _blockService;
            this._userManagerReturn = _userManagerReturn;
        }

        [HttpPost("post")]
        public async Task<IActionResult> PosTAsync([FromForm] AddPostDto createPostDto)
        {
            try
            {
                if (HttpContext.User != null && HttpContext.User.Identity != null
                    && HttpContext.User.Identity.Name!=null)
                {
                    var user = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                    if (user != null)
                    {
                        var response = await _postService.AddPostAsync(user, createPostDto);
                        return Ok(response);
                    }
                }
                return StatusCode(StatusCodes.Status401Unauthorized, new ApiResponse<string>
                {
                    StatusCode = 401,
                    IsSuccess = false,
                    Message = "Unauthorized"
                });
            }
            catch(Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<string>
                {
                    StatusCode = 500,
                    IsSuccess = false,
                    Message = ex.Message
                });
            }
        }


        [HttpGet("post/{postId}")]
        public async Task<IActionResult> GetPostByIdASync([FromRoute] string postId)
        {
            try
            {
                if (HttpContext.User != null && HttpContext.User.Identity != null
                    && HttpContext.User.Identity.Name != null)
                {
                    var currentUser = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                    if (currentUser != null)
                    {
                        var response = await _postService.GetPostByIdAsync(currentUser, postId);
                        return Ok(response);
                    }
                    return StatusCode(StatusCodes.Status404NotFound, StatusCodeReturn<string>
                    ._404_NotFound("User not found"));
                }
                return StatusCode(StatusCodes.Status401Unauthorized, StatusCodeReturn<string>
                    ._401_UnAuthorized());
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, StatusCodeReturn<string>
                    ._500_ServerError(ex.Message));
            }
        }

        [HttpDelete("post/{postId}")]
        public async Task<IActionResult> DeletePostByIdASync([FromRoute] string postId)
        {
            try
            {
                if (HttpContext.User != null && HttpContext.User.Identity != null
                    && HttpContext.User.Identity.Name != null)
                {
                    var user = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                    
                    if (user != null)
                    {
                        var response = await _postService.DeletePostAsync(user, postId);
                        return Ok(response);   
                    }
                    return StatusCode(StatusCodes.Status404NotFound, StatusCodeReturn<string>
                    ._404_NotFound("User not found"));
                }
                return StatusCode(StatusCodes.Status401Unauthorized, StatusCodeReturn<string>
                    ._401_UnAuthorized());
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, StatusCodeReturn<string>
                    ._500_ServerError(ex.Message));
            }
        }



        [HttpGet("posts/{userIdOrUserName}")]
        public async Task<IActionResult> GetUserPostsAsync([FromRoute] string userIdOrUserName)
        {
            try
            {
                if (HttpContext.User != null && HttpContext.User.Identity != null
                    && HttpContext.User.Identity.Name != null)
                {
                    var currentUser = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                    var routeUser = await _userManagerReturn.GetUserByUserNameOrEmailOrIdAsync(
                        userIdOrUserName);
                    if (currentUser != null)
                    {
                        if (routeUser != null)
                        {
                            var response = await _postService.GetUserPostsAsync(currentUser, routeUser);
                            return Ok(response);
                        }
                        return StatusCode(StatusCodes.Status404NotFound, StatusCodeReturn<string>
                               ._404_NotFound("User you want to get posts not found"));
                    }
                    return StatusCode(StatusCodes.Status404NotFound, StatusCodeReturn<string>
                    ._404_NotFound("User not found"));
                }
                return StatusCode(StatusCodes.Status401Unauthorized, StatusCodeReturn<string>
                    ._401_UnAuthorized());
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, StatusCodeReturn<string>
                    ._500_ServerError(ex.Message));
            }
        }

        [HttpGet("posts")]
        public async Task<IActionResult> GetUserPostsAsync()
        {
            try
            {
                if (HttpContext.User != null && HttpContext.User.Identity != null
                    && HttpContext.User.Identity.Name != null)
                {
                    var currentUser = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                    if (currentUser != null)
                    {
                        var response = await _postService.GetUserPostsAsync(currentUser);
                        return Ok(response);
                    }
                }
                return StatusCode(StatusCodes.Status401Unauthorized, StatusCodeReturn<string>
                    ._401_UnAuthorized());
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, StatusCodeReturn<string>
                    ._500_ServerError(ex.Message));
            }
        }


        [HttpPut("updatePost")]
        public async Task<IActionResult> UpdatePostAsync([FromForm] UpdatePostDto updatePostDto)
        {
            try
            {
                if (HttpContext.User != null && HttpContext.User.Identity != null
                    && HttpContext.User.Identity.Name != null)
                {
                    var user = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                    if (user != null)
                    {
                        var response = await _postService.UpdatePostAsync(user, updatePostDto);
                        return Ok(response);
                    }
                    return StatusCode(StatusCodes.Status404NotFound, StatusCodeReturn<string>
                    ._404_NotFound("Post not found"));
                }
                return StatusCode(StatusCodes.Status401Unauthorized, StatusCodeReturn<string>
                    ._401_UnAuthorized());
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, StatusCodeReturn<string>
                    ._500_ServerError(ex.Message));
            }
        }

        [HttpPut("updatePostPolicy")]
        public async Task<IActionResult> UpdatePostPolicyAsync(
            [FromBody] UpdatePostPolicyDto updatePostPolicyDto)
        {
            try
            {
                if (HttpContext.User != null && HttpContext.User.Identity != null
                    && HttpContext.User.Identity.Name!=null)
                {
                    var user = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                    if (user != null)
                    {
                        var response = await _postService.UpdatePostPolicyAsync(user, updatePostPolicyDto);
                        return Ok(response);
                    }
                }
                return StatusCode(StatusCodes.Status401Unauthorized, StatusCodeReturn<string>
                    ._401_UnAuthorized());
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, StatusCodeReturn<string>
                    ._500_ServerError(ex.Message));
            }
        }

        [HttpPut("updatePostReactPolicy")]
        public async Task<IActionResult> UpdatePostReactPolicyAsync
            ([FromBody] UpdatePostReactPolicyDto updatePostReactPolicyDto)
        {
            try
            {
                if (HttpContext.User != null && HttpContext.User.Identity != null
                    && HttpContext.User.Identity.Name != null)
                {
                    var user = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                    if (user != null)
                    {
                        var response = await _postService
                            .UpdatePostReactPolicyAsync(user, updatePostReactPolicyDto);
                        return Ok(response);
                    }
                }
                return StatusCode(StatusCodes.Status401Unauthorized, StatusCodeReturn<string>
                    ._401_UnAuthorized());
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, StatusCodeReturn<string>
                    ._500_ServerError(ex.Message));
            }
        }

        [HttpPut("updatePostCommentPolicy")]
        public async Task<IActionResult> UpdatePostCommentPolicyAsync
            ([FromBody] UpdatePostCommentPolicyDto updatePostCommentPolicyDto)
        {
            try
            {
                if (HttpContext.User != null && HttpContext.User.Identity != null
                    && HttpContext.User.Identity.Name != null)
                {
                    var user = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                    if (user != null)
                    {
                        var response = await _postService
                            .UpdatePostCommentPolicyAsync(user, updatePostCommentPolicyDto);
                        return Ok(response);
                    }
                }
                return StatusCode(StatusCodes.Status401Unauthorized, StatusCodeReturn<string>
                    ._401_UnAuthorized());
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, StatusCodeReturn<string>
                    ._500_ServerError(ex.Message));
            }
        }



    }
}
