﻿

using SocialMedia.Api.Data.DTOs;
using SocialMedia.Api.Data.Extensions;
using SocialMedia.Api.Repository.RoleRepository;
using SocialMedia.Api.Service.GenericReturn;

namespace SocialMedia.Api.Service.RolesService
{
    public class RolesService : IRolesService
    {
        private readonly IRoleRepository _roleRepository;
        private readonly Policies policies = new();
        public RolesService(IRoleRepository _roleRepository)
        {
            this._roleRepository = _roleRepository;
        }

        public async Task<object> AddGroupRoleAsync(AddGroupRoleDto addGroupRoleDto)
        {
            var groupRole = await _roleRepository.GetRoleByRoleNameAsync(addGroupRoleDto.RoleName);
            if (groupRole == null)
            {
                if (policies.GroupRoles.Contains(addGroupRoleDto.RoleName.ToUpper()))
                {
                    var newGroupRole = await _roleRepository.AddAsync(ConvertFromDto
                    .ConvertFromGroupRoleDto_Add(addGroupRoleDto));
                    return StatusCodeReturn<object>
                        ._201_Created("Group role added successfully", newGroupRole);
                }
                return StatusCodeReturn<object>
                ._403_Forbidden("Invalid role");
            }
            return StatusCodeReturn<object>
                ._403_Forbidden("Group role already exists");
        }

        public async Task<object> DeleteGroupRoleByIdAsync(string groupRoleId)
        {
            var groupRole = await _roleRepository.GetByIdAsync(groupRoleId);
            if (groupRole != null)
            {
                await _roleRepository.DeleteByIdAsync(groupRoleId);
                return StatusCodeReturn<object>
                    ._200_Success("Group role deleted successfully", groupRole);
            }
            return StatusCodeReturn<object>
                ._404_NotFound("Group role not found");
        }

        public async Task<object> DeleteGroupRoleByRoleNameAsync(string groupRoleName)
        {
            var groupRole = await _roleRepository.GetRoleByRoleNameAsync(groupRoleName);
            if (groupRole != null)
            {
                await _roleRepository.DeleteRoleByRoleNameAsync(groupRoleName);
                return StatusCodeReturn<object>
                    ._200_Success("Group role deleted successfully", groupRole);
            }
            return StatusCodeReturn<object>
                ._404_NotFound("Group role not found");
        }

        public async Task<object> GetGroupRoleByIdAsync(string groupRoleId)
        {
            var groupRole = await _roleRepository.GetByIdAsync(groupRoleId);
            if (groupRole != null)
            {
                return StatusCodeReturn<object>
                    ._200_Success("Group role found successfully", groupRole);
            }
            return StatusCodeReturn<object>
                ._404_NotFound("Group role not found");
        }

        public async Task<object> GetGroupRoleByRoleNameAsync(string groupRoleName)
        {
            var groupRole = await _roleRepository.GetRoleByRoleNameAsync(groupRoleName);
            if (groupRole != null)
            {
                return StatusCodeReturn<object>
                    ._200_Success("Group role found successfully", groupRole);
            }
            return StatusCodeReturn<object>
                ._404_NotFound("Group role not found");
        }

        public async Task<object> GetGroupRolesAsync()
        {
            var groupRoles = await _roleRepository.GetAllAsync();
            if (groupRoles.ToList().Count == 0)
            {
                return StatusCodeReturn<object>
                    ._200_Success("No group roles found", groupRoles);
            }
            return StatusCodeReturn<object>
                    ._200_Success("Group roles found successfully", groupRoles);
        }

        public async Task<object> UpdateGroupRoleAsync(UpdateGroupRoleDto updateGroupRoleDto)
        {
            var groupRoleById = await _roleRepository.GetByIdAsync(updateGroupRoleDto.Id);
            if (groupRoleById != null)
            {
                var groupRoleByName = await _roleRepository.GetRoleByRoleNameAsync(
                    updateGroupRoleDto.RoleName);
                if (groupRoleByName == null)
                {
                    if (policies.GroupRoles.Contains(updateGroupRoleDto.RoleName.ToUpper()))
                    {
                        var updatedGroupRole = await _roleRepository.UpdateAsync(
                        ConvertFromDto.ConvertFromGroupRoleDto_Update(updateGroupRoleDto));
                        return StatusCodeReturn<object>
                            ._200_Success("Group role updated successfully", updatedGroupRole);
                    }
                    return StatusCodeReturn<object>
                    ._403_Forbidden("Invalid group role");
                }
                return StatusCodeReturn<object>
                    ._403_Forbidden("Group role already exists");
            }
            return StatusCodeReturn<object>
                    ._404_NotFound("Group role not found");
        }
    }
}