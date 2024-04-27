﻿

using SocialMedia.Data.DTOs.Authentication.EmailConfirmation;
using SocialMedia.Data.DTOs.Authentication.Login;
using SocialMedia.Data.DTOs.Authentication.Register;
using SocialMedia.Data.DTOs.Authentication.ResetEmail;
using SocialMedia.Data.DTOs.Authentication.ResetPassword;
using SocialMedia.Data.DTOs.Authentication.User;
using SocialMedia.Data.Models.ApiResponseModel;
using SocialMedia.Data.Models.Authentication;

namespace SocialMedia.Service.UserAccountService
{
    public interface IUserManagement
    {
        Task<ApiResponse<CreateUserResponse>> CreateUserWithTokenAsync(RegisterDto registerDto);
        Task<ApiResponse<List<string>>> AssignRolesToUserAsync(List<string> roles, SiteUser user);
        Task<ApiResponse<LoginOtpResponse>> LoginUserAsync(LoginDto loginDto);
        Task<ApiResponse<LoginResponse>> GetJwtTokenAsync(SiteUser user);
        Task<ApiResponse<LoginResponse>> LoginUserWithOTPAsync(string otp, string userNameOrEmail);
        Task<ApiResponse<LoginResponse>> RenewAccessTokenAsync(LoginResponse tokens);
        Task<ApiResponse<string>> ConfirmEmail(string userNameOrEmail, string token);
        Task<ApiResponse<string>> ResetPasswordAsync(ResetPasswordDto resetPasswordDto);
        Task<ApiResponse<ResetPasswordDto>> GenerateResetPasswordTokenAsync(string email);
        Task<ApiResponse<EmailConfirmationDto>> GenerateEmailConfirmationTokenAsync(string email);
        Task<ApiResponse<ResetEmailDto>> GenerateResetEmailTokenAsync(ResetEmailObjectDto resetEmailObjectDto);
        Task<ApiResponse<string>> EnableTwoFactorAuthenticationAsync(string email);
        Task<ApiResponse<string>> DeleteAccountAsync(string userNameOrEmail);
    }
}
