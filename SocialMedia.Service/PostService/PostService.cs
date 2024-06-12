﻿

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using SocialMedia.Data;
using SocialMedia.Data.DTOs;
using SocialMedia.Data.Extensions;
using SocialMedia.Data.Models;
using SocialMedia.Data.Models.ApiResponseModel;
using SocialMedia.Data.Models.Authentication;
using SocialMedia.Repository.CommentPolicyRepository;
using SocialMedia.Repository.FriendsRepository;
using SocialMedia.Repository.GroupPolicyRepository;
using SocialMedia.Repository.GroupPostsRepository;
using SocialMedia.Repository.GroupRepository;
using SocialMedia.Repository.PolicyRepository;
using SocialMedia.Repository.PostRepository;
using SocialMedia.Repository.PostsPolicyRepository;
using SocialMedia.Repository.PostViewRepository;
using SocialMedia.Repository.ReactPolicyRepository;
using SocialMedia.Repository.UserPostsRepository;
using SocialMedia.Service.BlockService;
using SocialMedia.Service.FriendsService;
using SocialMedia.Service.GenericReturn;
using SocialMedia.Service.PolicyService;

namespace SocialMedia.Service.PostService
{
    public class PostService : IPostService
    {
        private readonly IPostRepository _postRepository;
        private readonly ICommentPolicyRepository _commentPolicyRepository;
        private readonly IReactPolicyRepository _reactPolicyRepository;
        private readonly IPolicyRepository _policyRepository;
        private readonly IFriendsRepository _friendsRepository;
        private readonly IFriendService _friendService;
        private readonly IPolicyService _policyService;
        private readonly IUserPostsRepository _userPostsRepository;
        private readonly IPostsPolicyRepository _postsPolicyRepository;
        private readonly IPostViewRepository _postViewRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IGroupPolicyRepository _groupPolicyRepository;
        private readonly IGroupPostsRepository _groupPostsRepository;
        private readonly IGroupRepository _groupRepository;
        private readonly IBlockService _blockService;
        

        public PostService(IPostRepository _postRepository,
             ICommentPolicyRepository _commentPolicyRepository,
            IReactPolicyRepository _reactPolicyRepository, IPolicyRepository _policyRepository,
            IFriendsRepository _friendsRepository, IFriendService _friendService,
            IUserPostsRepository _userPostsRepository, IPolicyService _policyService,
            IPostsPolicyRepository _postsPolicyRepository, IGroupRepository _groupRepository,
            IPostViewRepository _postViewRepository, IWebHostEnvironment _webHostEnvironment,
            IGroupPolicyRepository _groupPolicyRepository, IGroupPostsRepository _groupPostsRepository,
            IBlockService _blockService)
        {
            this._postRepository = _postRepository;
            this._commentPolicyRepository = _commentPolicyRepository;
            this._policyRepository = _policyRepository;
            this._reactPolicyRepository = _reactPolicyRepository;
            this._friendsRepository = _friendsRepository;
            this._friendService = _friendService;
            this._userPostsRepository = _userPostsRepository;
            this._policyService = _policyService;
            this._postsPolicyRepository = _postsPolicyRepository;
            this._postViewRepository = _postViewRepository;
            this._webHostEnvironment = _webHostEnvironment;
            this._groupPostsRepository = _groupPostsRepository;
            this._groupPolicyRepository = _groupPolicyRepository;
            this._groupRepository = _groupRepository;
            this._blockService = _blockService;
        }
        public async Task<ApiResponse<PostDto>> AddPostAsync(SiteUser user, AddPostDto createPostDto)
        {
            var postsPolicy = await _postsPolicyRepository.GetPostPolicyByIdAsync(
                user.AccountPostPolicyId!);
            var reactPolicy = await _reactPolicyRepository.GetReactPolicyByIdAsync(user.ReactPolicyId!);
            var commentPolicy = await _commentPolicyRepository.GetCommentPolicyByIdAsync(
                user.CommentPolicyId!);

            var post = ConvertFromDto.ConvertFromCreatePostDto_Add(createPostDto, postsPolicy, reactPolicy,
                commentPolicy, user);
            var postImages = new List<PostImages>();
            if (createPostDto.Images != null)
            {
                foreach(var i in createPostDto.Images)
                {
                    postImages.Add(new PostImages
                    {
                        ImageUrl = SavePostImages(i),
                        PostId = post.Id,
                        Id = Guid.NewGuid().ToString()
                    });
                }
            }
            var newPostDto = await _postRepository.AddPostAsync(post, postImages);
            SetNull(newPostDto);
            return StatusCodeReturn<PostDto>
                    ._201_Created("Post created successfully", newPostDto);
        }

        public async Task<ApiResponse<bool>> DeletePostAsync(SiteUser user, string postId)
        {
            var post = await _postRepository.GetPostWithImagesByPostIdAsync(postId);
            if (post != null)
            {
                if(post.Post.UserId == user.Id)
                {
                    if(post.Images != null && post.Images.Count > 0)
                    {
                        foreach(var i in post.Images)
                        {
                            DeletePostImage(i.ImageUrl);
                        }
                    }
                    await _postRepository.DeletePostAsync(postId);
                    return StatusCodeReturn<bool>
                    ._200_Success("Post deleted successfully");
                }
                return StatusCodeReturn<bool>
                    ._403_Forbidden();
            }
            return StatusCodeReturn<bool>
                    ._404_NotFound("Post not found");
        }

        public async Task<ApiResponse<PostDto>> GetPostByIdAsync(SiteUser user, string postId)
        {
            var existPost = await _postRepository.GetPostByIdAsync(postId);
            if (existPost != null)
            {
                if(!(await IsBlockedAsync(user.Id, existPost.UserId)).ResponseObject)
                {
                    if ((await CheckPostPolicyAsync(user, existPost)).IsSuccess)
                    {
                        var post = await _postRepository.GetPostWithImagesByPostIdAsync(postId);
                        SetNull(post);
                        return StatusCodeReturn<PostDto>
                                ._200_Success("Post found successfully", post);
                    }
                    return StatusCodeReturn<PostDto>
                    ._403_Forbidden();
                }
                return StatusCodeReturn<PostDto>
                    ._403_Forbidden();
            }
            return StatusCodeReturn<PostDto>
                    ._404_NotFound("Post not found");
        }

        public async Task<ApiResponse<IEnumerable<PostDto>>> GetUserPostsAsync(SiteUser user)
        {
            var posts = await _postRepository.GetUserPostsAsync(user);
            foreach (var p in posts)
            {
                SetNull(p);
            }
            if (posts.ToList().Count == 0)
            {
                return StatusCodeReturn<IEnumerable<PostDto>>
                    ._200_Success("No posts found", posts);
            }
            return StatusCodeReturn<IEnumerable<PostDto>>
                    ._200_Success("Posts found successfully", posts);
        }

        public async Task<ApiResponse<IEnumerable<PostDto>>> GetUserPostsAsync(SiteUser user,
            SiteUser routeUser)
        {
            if (user.Id == routeUser.Id)
            {
                return await GetUserPostsAsync(user);
            }
            else if (!(await IsBlockedAsync(user.Id, routeUser.Id)).ResponseObject)
            {
                return await CheckFriendShipAndGetPostsAsync(user, routeUser); 
            }
            return StatusCodeReturn<IEnumerable<PostDto>>
                                ._403_Forbidden();
        }

        public async Task<ApiResponse<IEnumerable<PostDto>>> GetUserPostsByPolicyAsync(
            SiteUser user, PostsPolicy policy)
        {
            var checkPolicy = await _policyRepository.GetPolicyByIdAsync(policy.PolicyId);
            if (checkPolicy == null)
            {
                return StatusCodeReturn<IEnumerable<PostDto>>
                    ._404_NotFound("Policy not found");
            }
            var userPosts = await _postRepository.GetUserPostsByPolicyAsync(user, policy);
            foreach (var p in userPosts)
            {
                SetNull(p);
            }
            if (userPosts.ToList().Count == 0)
            {
                return StatusCodeReturn<IEnumerable<PostDto>>
                    ._200_Success("No posts found");
            }
            return StatusCodeReturn<IEnumerable<PostDto>>
                    ._200_Success("Posts found successfully", userPosts);
        }
        public async Task<ApiResponse<IEnumerable<PostDto>>> GetPostsForFriendsAsync(SiteUser user)
        {
            var posts = await _postRepository.GetUserPostsForFriendsAsync(user);
            foreach (var p in posts)
            {
                SetNull(p);
            }
            if (posts.ToList().Count == 0)
            {
                return StatusCodeReturn<IEnumerable<PostDto>>
                    ._200_Success("No posts found");
            }
            return StatusCodeReturn<IEnumerable<PostDto>>
                    ._200_Success("Posts found successfully", posts);
        }

        public async Task<ApiResponse<IEnumerable<PostDto>>> GetPostsForFriendsOfFriendsAsync(
            SiteUser user)
        {
            var posts = await _postRepository.GetUserPostsForFriendsOfFriendsAsync(user);
            foreach (var p in posts)
            {
                SetNull(p);
            }
            if (posts.ToList().Count == 0)
            {
                return StatusCodeReturn<IEnumerable<PostDto>>
                    ._200_Success("No posts found");
            }
            return StatusCodeReturn<IEnumerable<PostDto>>
                    ._200_Success("Posts found successfully", posts);
        }

        public async Task<ApiResponse<IEnumerable<PostDto>>> CheckFriendShipAndGetPostsAsync(
            SiteUser currentUser, SiteUser routeUser)
        {
            if(!(await IsBlockedAsync(currentUser.Id, routeUser.Id)).ResponseObject)
            {
                var isFriend = await _friendService.IsUserFriendAsync(routeUser.Id, currentUser.Id);
                if (isFriend.ResponseObject)
                {
                    return await GetPostsForFriendsAsync(routeUser);
                }
                var isFriendOfFriend = await _friendService.IsUserFriendOfFriendAsync(routeUser.Id,
                    currentUser.Id);
                if (isFriendOfFriend.ResponseObject)
                {
                    return await GetPostsForFriendsOfFriendsAsync(routeUser);
                }
                var policy = await _policyRepository.GetPolicyByNameAsync("public");
                var postPolicy = await _postsPolicyRepository.GetPostPolicyByPolicyIdAsync(policy.Id);
                var publicPosts = (await GetUserPostsByPolicyAsync(routeUser, postPolicy)).ResponseObject;
                if (publicPosts != null)
                {
                    foreach(var p in publicPosts)
                    {
                        SetNull(p);
                    }
                    return StatusCodeReturn<IEnumerable<PostDto>>
                        ._200_Success("Posts found successfully", publicPosts.ToList());
                }
                return StatusCodeReturn<IEnumerable<PostDto>>
                        ._404_NotFound("No posts found");
            }
            return StatusCodeReturn<IEnumerable<PostDto>>
                    ._403_Forbidden();
        }


        public async Task<ApiResponse<PostDto>> UpdatePostAsync(SiteUser user, UpdatePostDto updatePostDto)
        {
            var post = await _postRepository.GetPostWithImagesByPostIdAsync(updatePostDto.PostId);
            var postImages = new List<PostImages>();
            if (post != null)
            {
                if (post.Post.UserId == user.Id)
                {
                    if (updatePostDto.Images != null)
                    {
                        if (post.Images != null || post.Images!.Count > 0)
                        {
                            foreach (var p in post.Images)
                            {
                                DeletePostImage(p.ImageUrl);
                            }
                            await _postRepository.DeletePostImagesAsync(post.Post.Id);
                        }
                        foreach (var i in updatePostDto.Images)
                        {
                            var postImage = new PostImages
                            {
                                ImageUrl = SavePostImages(i),
                                PostId = updatePostDto.PostId,
                                Id = Guid.NewGuid().ToString()
                            };
                            postImages.Add(postImage);
                        }
                    }
                    post.Post.Content = updatePostDto.PostContent;
                    var updatedPost = await _postRepository.UpdatePostAsync(
                            ConvertFromPostDto(post), postImages);
                    SetNull(updatedPost);
                    return StatusCodeReturn<PostDto>
                            ._200_Success("Post updated successfully", updatedPost);
                }
                return StatusCodeReturn<PostDto>
                    ._403_Forbidden();
            }
            return StatusCodeReturn<PostDto>
            ._404_NotFound("Post not found");
        }

        public async Task<ApiResponse<bool>> UpdatePostPolicyAsync(SiteUser user,
            UpdatePostPolicyDto updatePostPolicyDto)
        {
            var post = await _postRepository.GetPostByIdAsync(updatePostPolicyDto.PostId);
            if (post != null)
            {
                if (post.UserId == user.Id)
                {
                    var policy = await _policyService.GetPolicyByIdOrNameAsync(
                        updatePostPolicyDto.PolicyIdOrName);
                    if (policy != null && policy.ResponseObject != null)
                    {
                        var postPolicy = await _postsPolicyRepository.GetPostPolicyByPolicyIdAsync(
                            policy.ResponseObject.Id);
                        post.PostPolicyId = postPolicy.Id;
                        await _postRepository.UpdatePostAsync(post);
                        return StatusCodeReturn<bool>
                            ._200_Success("Post policy updated successfully", true);
                    }
                    return StatusCodeReturn<bool>
                            ._404_NotFound("Policy not found", false);
                }
                return StatusCodeReturn<bool>
                            ._403_Forbidden();
            }
            return StatusCodeReturn<bool>
                            ._404_NotFound("Post not found", false);
        }

        public async Task<ApiResponse<bool>> UpdatePostReactPolicyAsync(SiteUser user,
            UpdatePostReactPolicyDto updatePostReactPolicy)
        {
            var post = await _postRepository.GetPostByIdAsync(updatePostReactPolicy.PostId);
            if (post != null)
            {
                if (post.UserId == user.Id)
                {
                    var policy = await _policyService.GetPolicyByIdOrNameAsync(
                        updatePostReactPolicy.PolicyIdOrName);
                    if (policy != null && policy.ResponseObject != null)
                    {
                        var postReactPolicy = await _reactPolicyRepository.GetReactPolicyByPolicyIdAsync(
                            policy.ResponseObject.Id);
                        if (postReactPolicy != null)
                        {
                            post.ReactPolicyId = postReactPolicy.Id;
                            await _postRepository.UpdatePostAsync(post);
                            return StatusCodeReturn<bool>
                                ._200_Success("Post react policy updated successfully", true);
                        }
                        return StatusCodeReturn<bool>
                            ._404_NotFound("Post react policy not found", false);
                    }
                    return StatusCodeReturn<bool>
                            ._404_NotFound("Policy not found", false);
                }
                return StatusCodeReturn<bool>
                            ._403_Forbidden();
            }
            return StatusCodeReturn<bool>
                            ._404_NotFound("Post not found", false);
        }

        public async Task<ApiResponse<bool>> UpdatePostCommentPolicyAsync(
            SiteUser user, UpdatePostCommentPolicyDto updatePostCommentPolicyDto)
        {
            var post = await _postRepository.GetPostByIdAsync(updatePostCommentPolicyDto.PostId);
            if (post != null)
            {
                if (post.UserId == user.Id)
                {
                    var policy = await _policyService.GetPolicyByIdOrNameAsync(
                        updatePostCommentPolicyDto.PolicyIdOrName);
                    if (policy != null && policy.ResponseObject != null)
                    {
                        var postCommentPolicy = await _commentPolicyRepository.GetCommentPolicyByPolicyIdAsync(
                            policy.ResponseObject.Id);
                        if (postCommentPolicy != null)
                        {
                            post.CommentPolicyId = postCommentPolicy.Id;
                            await _postRepository.UpdatePostAsync(post);
                            return StatusCodeReturn<bool>
                                ._200_Success("Post comment policy updated successfully", true);
                        }
                        return StatusCodeReturn<bool>
                            ._404_NotFound("Post comment policy not found", false);
                    }
                    return StatusCodeReturn<bool>
                            ._404_NotFound("Policy not found", false);
                }
                return StatusCodeReturn<bool>
                            ._403_Forbidden();
            }
            return StatusCodeReturn<bool>
                            ._404_NotFound("Post not found", false);
        }

        public async Task<ApiResponse<bool>> UpdateUserPostsPolicyToLockedProfileAsync(SiteUser user)
        {
            await _postRepository.UpdateUserPostsPolicyToLockedAccountAsync(user.Id);
            return StatusCodeReturn<bool>
                ._200_Success("Posts policy updated successfully to locked account", true);
        }

        public async Task<ApiResponse<bool>> UpdateUserPostsPolicyToUnLockedProfileAsync(SiteUser user)
        {
            await _postRepository.UpdateUserPostsPolicyToUnLockedAccountAsync(user.Id);
            return StatusCodeReturn<bool>
                ._200_Success("Posts policy updated successfully to unlocked account", true);
        }

        private string SavePostImages(IFormFile image)
        {
            var path = Path.Combine(_webHostEnvironment.ContentRootPath, @"wwwroot\Images\Post_Images");
            var uniqueFileName = Guid.NewGuid().ToString() + "_" + image.FileName;
            if (!System.IO.Directory.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path);
            }
            string filePath = Path.Combine(path, uniqueFileName);
            using(var fileStream = new FileStream(filePath, FileMode.Create))
            {
                image.CopyTo(fileStream);
                fileStream.Flush();
            }
            return uniqueFileName;
        }

        private bool DeletePostImage(string imageUrl)
        {
            var path = Path.Combine(_webHostEnvironment.ContentRootPath, @"wwwroot\Images\Post_Images\");
            var file = Path.Combine(path, $"{imageUrl}");
            if (System.IO.File.Exists(file))
            {
                System.IO.File.Delete(file);
                return true;
            }
            return false;
        }
        private async Task<ApiResponse<bool>> CheckPostPolicyAsync(SiteUser user, Post post)
        {
            var postPolicy = await _postsPolicyRepository.GetPostPolicyByIdAsync(post.PostPolicyId);
            var policy = await _policyRepository.GetPolicyByIdAsync(postPolicy.PolicyId);
            if (post.UserId != user.Id)
            {
                if (policy.PolicyType == "PRIVATE")
                {
                    return StatusCodeReturn<bool>
                        ._403_Forbidden();
                }
                else if (policy.PolicyType == "FRIENDS ONLY")
                {
                    var isFriend = await _friendService.IsUserFriendAsync(user.Id, post.UserId);
                    if (isFriend != null && !isFriend.ResponseObject)
                    {
                        return StatusCodeReturn<bool>
                        ._403_Forbidden();
                    }
                }
                else if (policy.PolicyType == "FRIEND OF FRIEND")
                {
                    var isFriendOfFriend = await _friendService.IsUserFriendOfFriendAsync(user.Id, post.UserId);
                    if (isFriendOfFriend != null && !isFriendOfFriend.ResponseObject)
                    {
                        return StatusCodeReturn<bool>
                        ._403_Forbidden();
                    }
                }
            }
            return StatusCodeReturn<bool>
                ._200_Success("Success", true);
        }

        private async Task<ApiResponse<bool>> IsBlockedAsync(string userId1, string userId2)
        {
            var isBlocked = await _blockService.GetBlockByUserIdAndBlockedUserIdAsync(userId1, userId2);
            if (isBlocked != null && isBlocked.ResponseObject != null && isBlocked.IsSuccess)
            {
                return StatusCodeReturn<bool>
                    ._200_Success("Blocked", true);
            }
            return StatusCodeReturn<bool>
                    ._200_Success("Not blocked", false);
        }

        private Post ConvertFromPostDto(PostDto post)
        {
            return post.Post;
        }

        private void SetNull(PostDto post)
        {
            post.Post.User = null;
            post.Post.UserPosts = null;
            post.Post.SavedPosts = null;
            post.Post.ReactPolicy = null;
            post.Post.PostViews = null;
            post.Post.PostReacts = null;
            post.Post.PostPolicy = null;
            post.Post.PostImages = null;
            post.Post.PostComments = null;
            post.Post.PagePosts = null;
            post.Post.GroupPosts = null;
            post.Post.CommentPolicy = null;
            if (post.Images != null)
            {
                foreach(var p in post.Images)
                {
                    p.Post = null;
                }
            }
        }
        
    }
}
