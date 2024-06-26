﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialMedia.Api.Data.DTOs;
using SocialMedia.Api.Service.FollowerService;
using SocialMedia.Api.Service.GenericReturn;

namespace SocialMedia.Api.Controllers
{
    
    [ApiController]
    public class FollowersController : ControllerBase
    {

        private readonly IFollowerService _followerService;
        private readonly UserManagerReturn _userManagerReturn;
        public FollowersController(IFollowerService _followerService,UserManagerReturn _userManagerReturn)
        {
            this._followerService = _followerService;
            this._userManagerReturn = _userManagerReturn;
        }


        [HttpPost("Follow")]
        public async Task<IActionResult> FollowAsync([FromBody] FollowDto followDto)
        {
            try
            {
                if(HttpContext.User!=null && HttpContext.User.Identity!=null
                    && HttpContext.User.Identity.Name != null)
                {
                    var follower = await _userManagerReturn.GetUserByUserNameOrEmailOrIdAsync(
                        HttpContext.User.Identity.Name);
                    if(follower!=null)
                    {
                        var user = await _userManagerReturn.GetUserByUserNameOrEmailOrIdAsync(
                        followDto.UserIdOrUserNameOrEmail);
                        if (user != null)
                        {
                            var response = await _followerService.FollowAsync(followDto, follower);
                            return Ok(response);
                        }
                        return StatusCode(StatusCodes.Status404NotFound, StatusCodeReturn<string>
                        ._404_NotFound("User you want to follow not found"));
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

        [Authorize(Roles = "Admin")]
        [HttpGet("adminFollow")]
        public async Task<IActionResult> FollowAsync(string userIdOrUserNameOrEmail
            , string followerIdOrUserNameOrEmail)
        {
            try
            {
                var user = await _userManagerReturn.GetUserByUserNameOrEmailOrIdAsync(
                        userIdOrUserNameOrEmail);
                var follower = await _userManagerReturn.GetUserByUserNameOrEmailOrIdAsync(
                        followerIdOrUserNameOrEmail);
                if (follower != null)
                {
                    if (user != null)
                    {
                        if (user.Id != follower.Id)
                        {
                            var response = await _followerService.FollowAsync(user, follower);
                            return Ok(response);
                        }
                        return StatusCode(StatusCodes.Status403Forbidden, StatusCodeReturn<string>
                        ._403_Forbidden());
                    }
                    return StatusCode(StatusCodes.Status404NotFound, StatusCodeReturn<string>
                        ._404_NotFound("User you want to follow not found"));
                }
                return StatusCode(StatusCodes.Status404NotFound, StatusCodeReturn<string>
                    ._404_NotFound("User not found"));

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, StatusCodeReturn<string>
                    ._500_ServerError(ex.Message));
            }
        }

        [HttpGet("followers")]
        public async Task<IActionResult> FollowersAsync()
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
                        var response = await _followerService.GetAllFollowers(user.Id);
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


        [HttpGet("followers/{userIdOrName}")]
        public async Task<IActionResult> FollowersAsync([FromRoute] string userIdOrName)
        {
            try
            {
                var user = await _userManagerReturn.GetUserByUserNameOrEmailOrIdAsync(userIdOrName);
                if (HttpContext.User != null && HttpContext.User.Identity != null
                    && HttpContext.User.Identity.Name!=null)
                {
                    var currentUser = await _userManagerReturn.GetUserByUserNameOrEmailOrIdAsync(
                        HttpContext.User.Identity.Name);
                    if (currentUser != null)
                    { 
                        if (user != null)
                        {
                            var response = await _followerService.GetAllFollowers(user.Id, currentUser);
                            return Ok(response);
                        }
                        return StatusCode(StatusCodes.Status404NotFound, StatusCodeReturn<string>
                            ._404_NotFound("User you want to get followers not found"));
                    }
                    return StatusCode(StatusCodes.Status404NotFound, StatusCodeReturn<string>
                    ._404_NotFound("User not found"));
                }
                if (user != null)
                {
                    var response = await _followerService.GetAllFollowers(user.Id);
                    return Ok(response);
                }
                return StatusCode(StatusCodes.Status404NotFound, StatusCodeReturn<string>
                            ._404_NotFound("User you want to get followers not found"));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, StatusCodeReturn<string>
                    ._500_ServerError(ex.Message));
            }
        }

        [HttpPost("unfollow")]
        public async Task<IActionResult> UnfollowAsync([FromBody] UnFollowDto unFollowDto)
        {
            try
            {
                if (HttpContext.User != null && HttpContext.User.Identity != null
                    && HttpContext.User.Identity.Name != null)
                {
                    var follower = await _userManagerReturn.GetUserByUserNameOrEmailOrIdAsync(
                        HttpContext.User.Identity.Name);
                    if (follower != null)
                    {
                        var user = await _userManagerReturn.GetUserByUserNameOrEmailOrIdAsync(
                        unFollowDto.UserIdOrUserNameOrEmail);
                        if (user != null)
                        {
                            var response = await _followerService.UnfollowAsync(unFollowDto, follower);
                            return Ok(response);
                        }
                        return StatusCode(StatusCodes.Status404NotFound, StatusCodeReturn<string>
                            ._404_NotFound("User you want to unfollow not found"));
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

        [HttpDelete("unfollow/{followId}")]
        public async Task<IActionResult> UnfollowAsync([FromRoute] string followId)
        {
            try
            {
                if (HttpContext.User != null && HttpContext.User.Identity != null
                    && HttpContext.User.Identity.Name != null)
                {
                    var follower = await _userManagerReturn.GetUserByUserNameOrEmailOrIdAsync(
                        HttpContext.User.Identity.Name);
                    if (follower != null)
                    {
                        var response = await _followerService.UnfollowAsync(followId, follower.Id);
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
