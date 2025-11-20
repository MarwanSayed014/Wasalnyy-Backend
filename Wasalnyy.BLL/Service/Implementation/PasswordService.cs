using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using System.Net;
namespace Wasalnyy.BLL.Service.Implementation
{
	public class PasswordService : IPasswordService
	{
		private readonly UserManager<User> _userManager;
		private readonly IEmailService _emailService;

		public PasswordService(UserManager<User> userManager, IEmailService emailService)
		{
			_userManager = userManager;
			_emailService = emailService;
		}

		public async Task<bool> ForgotPasswordAsync(ForgotPasswordDto dto)
		{
			var user = await _userManager.FindByEmailAsync(dto.Email);
			if (user == null)
				return true; // Don't expose existence
			var token = await _userManager.GeneratePasswordResetTokenAsync(user);
			var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
			var resetUrl = $"{dto.Url}/reset-password?email={WebUtility.UrlEncode(user.Email)}&token={encodedToken}";
			await _emailService.SendEmail(user.Email, "Reset Password",
				$"<p>Click the link to reset your password:</p><a href='{resetUrl}'>Reset Password</a>");
			return true;
		}

		public async Task<(bool Success, IEnumerable<string> Errors)> ResetPasswordAsync(ResetPasswordDto dto)
		{
			var user = await _userManager.FindByEmailAsync(dto.Email);
			if (user == null)
				return (false, new[] { "User not found." });
			string decodedToken;
			try
			{
				decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(dto.Token));
			}
			catch
			{
				return (false, new[] { "Invalid token format." });
			}
			var result = await _userManager.ResetPasswordAsync(user, decodedToken, dto.NewPassword);
			if (!result.Succeeded)
			{
				var errors = result.Errors.Select(e => e.Description);
				return (false, errors);
			}
			return (true, Enumerable.Empty<string>());
		}
	}
}
