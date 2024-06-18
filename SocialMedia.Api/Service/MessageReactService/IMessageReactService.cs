﻿

using SocialMedia.Api.Data.DTOs;
using SocialMedia.Api.Data.Models;
using SocialMedia.Api.Data.Models.ApiResponseModel;
using SocialMedia.Api.Data.Models.Authentication;

namespace SocialMedia.Api.Service.MessageReactService
{
    public interface IMessageReactService
    {
        Task<ApiResponse<MessageReact>> AddReactToMessageAsync(AddMessageReactDto addMessageReactDto,
            SiteUser user);
        Task<ApiResponse<MessageReact>> GetReactToMessageAsync(string messageReactId, SiteUser user);
        Task<ApiResponse<MessageReact>> DeleteReactToMessageAsync(string messageReactId, SiteUser user);
        Task<ApiResponse<MessageReact>> UpdateReactToMessageAsync(UpdateMessageReactDto updateMessageReactDto,
            SiteUser user);
        Task<ApiResponse<IEnumerable<MessageReact>>> GetMessageReactsAsync(string messageId, SiteUser user);
    }
}