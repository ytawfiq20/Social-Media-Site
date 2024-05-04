﻿

using SocialMedia.Data.DTOs;
using SocialMedia.Data.Extensions;
using SocialMedia.Data.Models;
using SocialMedia.Data.Models.ApiResponseModel;
using SocialMedia.Repository.PolicyRepository;

namespace SocialMedia.Service.PolicyService
{
    public class PolicyService : IPolicyService
    {
        private readonly IPolicyRepository _policyRepository;
        public PolicyService(IPolicyRepository _policyRepository)
        {
            this._policyRepository = _policyRepository;
        }

        public async Task<ApiResponse<Policy>> AddPolicyAsync(PolicyDto policyDto)
        {
            var newPolicy = await _policyRepository.AddPolicyAsync(
                ConvertFromDto.ConvertFromPolicyDto_Add(policyDto));
            if (newPolicy == null)
            {
                return new ApiResponse<Policy>
                {
                    IsSuccess = false,
                    Message = "Can't add policy",
                    StatusCode = 500
                };
            }
            return new ApiResponse<Policy>
            {
                IsSuccess = true,
                Message = "Policy added successfully",
                StatusCode = 201,
                ResponseObject = newPolicy
            };
        }

        public async Task<ApiResponse<Policy>> DeletePolicyByIdAsync(Guid policyId)
        {
            var policy = await _policyRepository.GetPolicyByIdAsync(policyId);
            if (policy == null)
            {
                return new ApiResponse<Policy>
                {
                    IsSuccess = false,
                    Message = "Policy not found",
                    StatusCode = 404
                };
            }
            var deletedPolicy = await _policyRepository.DeletePolicyByIdAsync(policyId);
            return new ApiResponse<Policy>
            {
                IsSuccess = true,
                Message = "Policy deleted successfully",
                StatusCode = 200,
                ResponseObject = deletedPolicy
            };
        }

        public async Task<ApiResponse<IEnumerable<Policy>>> GetPoliciesAsync()
        {
            var policies = await _policyRepository.GetPoliciesAsync();
            if (policies.ToList().Count == 0)
            {
                return new ApiResponse<IEnumerable<Policy>>
                {
                    IsSuccess = true,
                    Message = "No policies found",
                    StatusCode = 200,
                    ResponseObject = policies
                };
            }
            return new ApiResponse<IEnumerable<Policy>>
            {
                IsSuccess = true,
                Message = "Policies found successfully",
                StatusCode = 200,
                ResponseObject = policies
            };
        }

        public async Task<ApiResponse<Policy>> GetPolicyByIdAsync(Guid policyId)
        {
            var policy = await _policyRepository.GetPolicyByIdAsync(policyId);
            if (policy == null)
            {
                return new ApiResponse<Policy>
                {
                    IsSuccess = false,
                    Message = "Policy not found",
                    StatusCode = 404
                };
            }
            return new ApiResponse<Policy>
            {
                IsSuccess = true,
                Message = "Policy found successfully",
                StatusCode = 200,
                ResponseObject = policy
            };
        }

        public async Task<ApiResponse<Policy>> UpdatePolicyAsync(PolicyDto policyDto)
        {
            if (policyDto.Id == null)
            {
                return new ApiResponse<Policy>
                {
                    IsSuccess = false,
                    Message = "Policy id must not be null",
                    StatusCode = 400
                };
            }
            var updatedPolicy = await _policyRepository.UpdatePolicyAsync(
                ConvertFromDto.ConvertFromPolicyDto_Update(policyDto));
            return new ApiResponse<Policy>
            {
                IsSuccess = true,
                Message = "Policy updated successfully",
                StatusCode = 200,
                ResponseObject = updatedPolicy
            };
        }
    }
}