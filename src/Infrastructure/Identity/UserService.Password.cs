﻿using Microsoft.AspNetCore.WebUtilities;
using ICISAdminPortal.Application.Common.Exceptions;
using ICISAdminPortal.Application.Common.Mailing;
using ICISAdminPortal.Application.Identity.Users.Password;
using System.Net;

namespace ICISAdminPortal.Infrastructure.Identity;
internal partial class UserService
{
    public async Task<string> ForgotPasswordAsync(ForgotPasswordRequest request, string origin)
    {
        EnsureValidTenant();

        var user = await _userManager.FindByEmailAsync(request.Email.Normalize());
        if (user is null || !await _userManager.IsEmailConfirmedAsync(user))
        {
            // Don't reveal that the user does not exist or is not confirmed
            throw new Application.Exceptions.ValidationException(_t["An Error has occurred!"], (int)HttpStatusCode.BadRequest);
        }

        // For more information on how to enable account confirmation and password reset please
        // visit https://go.microsoft.com/fwlink/?LinkID=532713
        string code = await _userManager.GeneratePasswordResetTokenAsync(user);
        const string route = "account/reset-password";
        var endpointUri = new Uri(string.Concat($"{origin}/", route));
        string passwordResetUrl = QueryHelpers.AddQueryString(endpointUri.ToString(), "Token", code);
        var mailRequest = new MailRequest(
            new List<string> { request.Email },
            _t["Reset Password"],
            _t[$"Your Password Reset Token is '{code}'. You can reset your password using the {endpointUri} Endpoint."]);
        _jobService.Enqueue(() => _mailService.SendAsync(mailRequest, CancellationToken.None));

        return _t["Password Reset Mail has been sent to your authorized Email."];
    }

    public async Task<string> ResetPasswordAsync(ResetPasswordRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email?.Normalize()!);

        // Don't reveal that the user does not exist
        _ = user ?? throw new Application.Exceptions.ValidationException(_t["An Error has occurred!"], (int)HttpStatusCode.BadRequest);

        var result = await _userManager.ResetPasswordAsync(user, request.Token!, request.Password!);

        return result.Succeeded
            ? _t["Password Reset Successful!"]
            : throw new Application.Exceptions.ValidationException(_t["An Error has occurred!"], (int)HttpStatusCode.BadRequest);
    }

    public async Task ChangePasswordAsync(ChangePasswordRequest model, string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);

        _ = user ?? throw new Application.Exceptions.ValidationException(_t["User Not Found."], (int)HttpStatusCode.BadRequest);

        var result = await _userManager.ChangePasswordAsync(user, model.Password, model.NewPassword);

        if (!result.Succeeded)
        {
            throw new Application.Exceptions.ValidationException(result.GetErrors(_t), (int)HttpStatusCode.BadRequest);
        }
    }
}