﻿

using SocialMedia.Api.Data.Models;
using SocialMedia.Api.Data.Models.Authentication;
using SocialMedia.Api.Repository.GenericCrudInterface;

namespace SocialMedia.Api.Repository.ChatRequestRepository
{
    public interface IChatRequestRepository : ICrud<ChatRequest>
    {
        Task<IEnumerable<ChatRequest>> GetReceivedChatRequestsAsync(SiteUser user);
        Task<IEnumerable<ChatRequest>> GetSentChatRequestsAsync(SiteUser user);
        Task<ChatRequest> GetChatRequestAsync(SiteUser user1, SiteUser user2);
    }
}