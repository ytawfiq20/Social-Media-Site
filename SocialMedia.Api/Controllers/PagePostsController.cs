﻿using Microsoft.AspNetCore.Mvc;
using SocialMedia.Data.DTOs;
using SocialMedia.Service.GenericReturn;
using SocialMedia.Service.PagePostsService;

namespace SocialMedia.Api.Controllers
{
    [ApiController]
    public class PagePostsController : ControllerBase
    {
        private readonly IPagePostsService _pagePostsService;
        private readonly UserManagerReturn _userManagerReturn;
        public PagePostsController(IPagePostsService _pagePostsService, UserManagerReturn _userManagerReturn)
        {
            this._pagePostsService = _pagePostsService;
            this._userManagerReturn = _userManagerReturn;
        }

        [HttpPost("addPagePost")]
        public async Task<IActionResult> AddPagePostAsync([FromForm] AddPagePostDto addPagePostDto)
        {
            try
            {
                if(HttpContext.User != null && HttpContext.User.Identity != null 
                    && HttpContext.User.Identity.Name != null)
                {
                    var user = await _userManagerReturn.GetUserByUserNameOrEmailOrIdAsync(
                        HttpContext.User.Identity.Name);
                    if (user != null)
                    {
                        var response = await _pagePostsService.AddPagePostAsync(addPagePostDto, user);
                        return Ok(response);
                    }
                    return StatusCode(StatusCodes.Status404NotFound, StatusCodeReturn<string>
                        ._404_NotFound("User not found"));
                }
                return StatusCode(StatusCodes.Status401Unauthorized, StatusCodeReturn<string>
                        ._401_UnAuthorized());
            }
            catch(Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, StatusCodeReturn<string>
                    ._500_ServerError(ex.Message));
            }
        }

        [HttpDelete("deletePagePost/{pagePostId}")]
        public async Task<IActionResult> AddPagePostAsync([FromRoute] string pagePostId)
        {
            try
            {
                if (HttpContext.User != null && HttpContext.User.Identity != null
                    && HttpContext.User.Identity.Name != null)
                {
                    var user = await _userManagerReturn.GetUserByUserNameOrEmailOrIdAsync(
                        HttpContext.User.Identity.Name);
                    if (user != null)
                    {
                        var response = await _pagePostsService.DeletePagePostByIdAsync(pagePostId, user);
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


    }
}
