﻿

using SocialMedia.Api.Data.DTOs;
using SocialMedia.Api.Data.Extensions;
using SocialMedia.Api.Data.Models;
using SocialMedia.Api.Data.Models.ApiResponseModel;
using SocialMedia.Api.Data.Models.Authentication;
using SocialMedia.Api.Repository.BlockRepository;
using SocialMedia.Api.Repository.FriendsRepository;
using SocialMedia.Api.Repository.PolicyRepository;
using SocialMedia.Api.Service.GenericReturn;

namespace SocialMedia.Api.Service.FriendsService
{
    public class FriendService : IFriendService
    {
        private readonly IFriendsRepository _friendsRepository;
        private readonly IBlockRepository _blockRepository;
        private readonly IPolicyRepository _policyRepository;
        private readonly UserManagerReturn _userManagerReturn;
        public FriendService(IFriendsRepository _friendsRepository, IBlockRepository _blockRepository,
            IPolicyRepository _policyRepository, UserManagerReturn _userManagerReturn)
        {
            this._friendsRepository = _friendsRepository;
            this._blockRepository = _blockRepository;
            this._policyRepository = _policyRepository;
            this._userManagerReturn = _userManagerReturn;
        }
        public async Task<ApiResponse<Friend>> AddFriendAsync(AddFriendDto addFriendDto)
        {
            var existFriend = await _friendsRepository.GetByUserAndFriendIdAsync(addFriendDto.UserId,
                addFriendDto.FriendId);
            if (existFriend == null)
            {
                var isBlocked = await _blockRepository.GetBlockByUserIdAndBlockedUserIdAsync(
                    addFriendDto.FriendId, addFriendDto.UserId);
                if (isBlocked == null)
                {
                    var newFriend = await _friendsRepository.AddAsync(
                    ConvertFromDto.ConvertFromFriendtDto_Add(addFriendDto));
                    newFriend.User = _userManagerReturn.SetUserToReturn(await _userManagerReturn
                        .GetUserByUserNameOrEmailOrIdAsync(addFriendDto.FriendId));
                    return StatusCodeReturn<Friend>
                        ._201_Created("Friend added successfully to your friend list", newFriend);
                }
                return StatusCodeReturn<Friend>
                    ._403_Forbidden();
            }
            return StatusCodeReturn<Friend>
                    ._403_Forbidden("You are already friends");
        }

        public async Task<ApiResponse<Friend>> DeleteFriendAsync(string userId, string friendId)
        {
            if (userId != friendId)
            {
                var isYourFriend = await _friendsRepository.GetByUserAndFriendIdAsync(userId, friendId);
                if (isYourFriend != null)
                {
                    await _friendsRepository.DeleteFriendAsync(userId, friendId);
                    isYourFriend.User = _userManagerReturn.SetUserToReturn(await _userManagerReturn
                        .GetUserByUserNameOrEmailOrIdAsync(friendId));
                    return StatusCodeReturn<Friend>
                        ._200_Success("Friend deleted successfully", isYourFriend);
                }
                return StatusCodeReturn<Friend>
                        ._404_NotFound("Friend not in your friend list");
            }
            return StatusCodeReturn<Friend>
                ._403_Forbidden();
        }

        public async Task<ApiResponse<Friend>> DeleteFriendAsync(string id, SiteUser user)
        {
            var friendship = await _friendsRepository.GetByIdAsync(id);
            if (friendship != null)
            {
                if(friendship.UserId == user.Id || friendship.FriendId == user.Id)
                {
                    var deletedFriendShip = await _friendsRepository.DeleteByIdAsync(id);
                    deletedFriendShip.User = _userManagerReturn.SetUserToReturn(user);
                    return StatusCodeReturn<Friend>
                        ._200_Success("Unfriend successfully", deletedFriendShip);
                }
                return StatusCodeReturn<Friend>
                    ._403_Forbidden();
            }
            return StatusCodeReturn<Friend>
                    ._404_NotFound("Friendship not found");
        }

        public async Task<ApiResponse<IEnumerable<Friend>>> GetAllUserFriendsAsync(SiteUser user, 
            SiteUser routeUser)
        {
            var friends = await _friendsRepository.GetAllUserFriendsAsync(routeUser.Id);
            if((await CheckGetFriendPolicyAsync<IEnumerable<Friend>>(user, routeUser)).IsSuccess)
            {
                if (friends.ToList().Count == 0)
                {
                    return StatusCodeReturn<IEnumerable<Friend>>
                        ._200_Success("No friends found");
                }
                return StatusCodeReturn<IEnumerable<Friend>>
                        ._200_Success("Friends found successfully", friends);
            }
            return await CheckGetFriendPolicyAsync<IEnumerable<Friend>>(user, routeUser);
        }

        public async Task<ApiResponse<IEnumerable<Friend>>> GetAllUserFriendsAsync(SiteUser user)
        {
            var friends = await _friendsRepository.GetAllUserFriendsAsync(user.Id);
            if (friends.ToList().Count == 0)
            {
                return StatusCodeReturn<IEnumerable<Friend>>
                    ._200_Success("No friends found");
            }
            return StatusCodeReturn<IEnumerable<Friend>>
                    ._200_Success("Friends found successfully", friends);
        }

        public async Task<ApiResponse<IEnumerable<Friend>>> GetSharedFriendsAsync(SiteUser user,
            SiteUser routeUser)
        {
            var sharedFriends = await _friendsRepository.GetSharedFriendsAsync(user.Id, routeUser.Id);
            if (sharedFriends.ToList().Count == 0)
            {
                return StatusCodeReturn<IEnumerable<Friend>>
                    ._200_Success("No shared friends found", sharedFriends);
            }
            return StatusCodeReturn<IEnumerable<Friend>>
                    ._200_Success("Shared friends found successfully", sharedFriends);
        }

        public async Task<ApiResponse<bool>> IsUserFriendAsync(string userId, string friendId)
        {
            var check = await _friendsRepository.GetByUserAndFriendIdAsync(userId, friendId);
            if (check == null)
            {
                return StatusCodeReturn<bool>
                    ._404_NotFound("Not friend", false);
            }
            return StatusCodeReturn<bool>
                    ._200_Success("Friend", true);
        }

        public async Task<ApiResponse<bool>> IsUserFriendOfFriendAsync(string routeUserId, string userId)
        {
            var friendsOfFriends = (await _friendsRepository.GetUserFriendsOfFriendsAsync(routeUserId))
                .ToList();
            foreach(var userFriends in friendsOfFriends)
            {
                foreach(var friend in userFriends)
                {
                    if(friend.FriendId == userId || friend.UserId == userId)
                    {
                        return StatusCodeReturn<bool>
                            ._200_Success("Friend of friend", true);
                    }
                }
            }
            return StatusCodeReturn<bool>
                    ._404_NotFound("Not friend of friend", false);
        }


        private async Task<ApiResponse<T>> CheckGetFriendPolicyAsync<T>(SiteUser user, SiteUser routeUser)
        {
            var policy = await _policyRepository.GetByIdAsync(routeUser.FriendListPolicyId);
            if (policy != null)
            {
                if (user.Id != routeUser.Id)
                {
                    var isFriend = await IsUserFriendAsync(user.Id, routeUser.Id);
                    if (policy.PolicyType == "FRIENDS ONLY")
                    {
                        if (!isFriend.IsSuccess)
                        {
                            return StatusCodeReturn<T>
                                ._403_Forbidden();
                        }
                    }
                    else if (policy.PolicyType == "FRIENDS OF FRIENDS")
                    {
                        var isFriendOfFriend = await IsUserFriendOfFriendAsync(routeUser.Id, user.Id);
                        if (!isFriendOfFriend.IsSuccess && !isFriend.IsSuccess)
                        {
                            return StatusCodeReturn<T>
                                ._403_Forbidden();
                        }
                    }

                    else if (policy.PolicyType == "PRIVATE")
                    {
                        return StatusCodeReturn<T>
                            ._403_Forbidden();
                    }
                }
                return StatusCodeReturn<T>
                    ._200_Success("Success");
            }
            return StatusCodeReturn<T>
                            ._404_NotFound("Policy not found");
        }


    }
}
