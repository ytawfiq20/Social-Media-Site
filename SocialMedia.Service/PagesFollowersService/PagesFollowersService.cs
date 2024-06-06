﻿
using SocialMedia.Data.DTOs;
using SocialMedia.Data.Extensions;
using SocialMedia.Data.Models;
using SocialMedia.Data.Models.ApiResponseModel;
using SocialMedia.Data.Models.Authentication;
using SocialMedia.Repository.PageRepository;
using SocialMedia.Repository.PagesFollowersRepository;
using SocialMedia.Service.GenericReturn;

namespace SocialMedia.Service.PagesFollowersService
{
    public class PagesFollowersService : IPagesFollowersService
    {
        private readonly IPageRepository _pageRepository;
        private readonly IPagesFollowersRepository _pagesFollowersRepository;
        private readonly UserManagerReturn _userManagerReturn;
        public PagesFollowersService(IPageRepository _pageRepository,
            IPagesFollowersRepository _pagesFollowersRepository, UserManagerReturn _userManagerReturn)
        {
            this._pageRepository = _pageRepository;
            this._pagesFollowersRepository = _pagesFollowersRepository;
            this._userManagerReturn = _userManagerReturn;
        }
        public async Task<ApiResponse<object>> FollowPageAsync(FollowPageDto followPageDto, SiteUser user)
        {
            var page = await _pageRepository.GetPageByIdAsync(followPageDto.PageId);
            if (page != null)
            {
                var pageFollower = await _pagesFollowersRepository.GetPageFollowerByPageIdAndFollowerIdAsync(
                    followPageDto.PageId, user.Id);
                if (pageFollower == null)
                {
                    var followPage = await _pagesFollowersRepository.FollowPageAsync(
                        ConvertFromDto.ConvertFromFollowPageDto_Add(followPageDto, user));
                    SetNull(followPage);
                    return StatusCodeReturn<object>
                        ._201_Created("Followed successfully", followPage);
                }
                return StatusCodeReturn<object>
                    ._403_Forbidden("You already following this page");
            }
            return StatusCodeReturn<object>
                ._404_NotFound("page you want to follow not found");
        }

        public async Task<ApiResponse<object>> FollowPageAsync(FollowPageUserDto followPageUserDto)
        {
            var page = await _pageRepository.GetPageByIdAsync(followPageUserDto.PageId);
            if (page != null)
            {
                var user = await _userManagerReturn.GetUserByUserNameOrEmailOrIdAsync(
                    followPageUserDto.UserIdOrUserNameOrEmail);
                if (user != null)
                {
                    followPageUserDto.UserIdOrUserNameOrEmail = user.Id;
                    var pageFollower = await _pagesFollowersRepository.GetPageFollowerByPageIdAndFollowerIdAsync(
                        followPageUserDto.PageId, followPageUserDto.UserIdOrUserNameOrEmail);
                    if (pageFollower == null)
                    {
                        var followPage = await _pagesFollowersRepository.FollowPageAsync(
                            ConvertFromDto.ConvertFromFollowPageUserDto_Add(followPageUserDto));
                        SetNull(followPage);
                        return StatusCodeReturn<object>
                            ._201_Created("Followed successfully", followPage);
                    }
                    return StatusCodeReturn<object>
                    ._403_Forbidden("You already following this page");
                }
                return StatusCodeReturn<object>
                ._404_NotFound("User not found");
            }
            return StatusCodeReturn<object>
                ._404_NotFound("page you want to follow not found");
        }

        public async Task<ApiResponse<object>> GetPageFollowerAsync(string pageId, SiteUser user)
        {
            var page = await _pageRepository.GetPageByIdAsync(pageId);
            if (page != null)
            {
                var pageFollower = await _pagesFollowersRepository.GetPageFollowerByPageIdAndFollowerIdAsync(
                pageId, user.Id);
                if (pageFollower != null)
                {
                    SetNull(pageFollower);
                    return StatusCodeReturn<object>
                        ._200_Success("Page follower found successfully", pageFollower);
                }
                return StatusCodeReturn<object>
                    ._404_NotFound("Page follower not found");
            }
            return StatusCodeReturn<object>
                    ._404_NotFound("Page not found");
        }

        public async Task<ApiResponse<object>> GetPageFollowerAsync(string pageFollowersId)
        {
            var pageFollower = await _pagesFollowersRepository.GetPageFollowerByIdAsync(pageFollowersId);
            if (pageFollower != null)
            {
                SetNull(pageFollower);
                return StatusCodeReturn<object>
                    ._200_Success("Page follower found successfully", pageFollower);
            }
            return StatusCodeReturn<object>
                ._404_NotFound("Page follower not found");
        }

        public async Task<ApiResponse<object>> GetPageFollowersByPageIdAsync(string pageId)
        {
            var followers = await _pagesFollowersRepository.GetPageFollowersAsync(pageId);
            if (followers.ToList().Count == 0)
            {
                return StatusCodeReturn<object>
                    ._200_Success("No followers found for this page", followers);
            }
            return StatusCodeReturn<object>
                    ._200_Success("Followers for this page found successfully", followers);
        }

        public async Task<ApiResponse<object>> UnFollowPageAsync(
            UnFollowPageDto unFollowPageDto, SiteUser user)
        {
            var page = await _pageRepository.GetPageByIdAsync(unFollowPageDto.PageId);
            if (page != null)
            {
                var pageFollower = await _pagesFollowersRepository.GetPageFollowerByPageIdAndFollowerIdAsync(
                unFollowPageDto.PageId, user.Id);
                if (pageFollower != null)
                {
                    await _pagesFollowersRepository.UnfollowPageByPageIdAsync(unFollowPageDto.PageId, user.Id);
                    SetNull(pageFollower);
                    return StatusCodeReturn<object>
                        ._200_Success("Unfollowed successfully", pageFollower);
                }
                return StatusCodeReturn<object>
                    ._404_NotFound("Page follower not found");
            }
            return StatusCodeReturn<object>
                    ._404_NotFound("Page not found");
        }

        public async Task<ApiResponse<object>> UnFollowPageAsync(string pageFollowersId, SiteUser user)
        {
            var pageFollower = await _pagesFollowersRepository.GetPageFollowerByIdAsync(pageFollowersId);
            if (pageFollower != null)
            {
                if(pageFollower.FollowerId == user.Id)
                {
                    await _pagesFollowersRepository.UnfollowPageByPageFollowerIdAsync(pageFollowersId);
                    SetNull(pageFollower);
                    return StatusCodeReturn<object>
                        ._200_Success("Unfollowed successfully", pageFollower);
                }
                return StatusCodeReturn<object>
                    ._403_Forbidden();
            }
            return StatusCodeReturn<object>
                ._404_NotFound("Page follower not found");
        }


        private object SetNull(PageFollower pageFollower)
        {
            pageFollower.User = null;
            pageFollower.Page = null;
            return pageFollower;
        }
    }
}
