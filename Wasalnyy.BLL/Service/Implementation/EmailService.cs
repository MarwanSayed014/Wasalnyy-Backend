using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using MimeKit;
using MimeKit.Text;

namespace Wasalnyy.BLL.Service.Implementation
{
	public class EmailService : IEmailService
	{
		private readonly IConfiguration _config;
		private readonly UserManager<User> _userManager;

		public EmailService(IConfiguration config,UserManager<User> userManager)
		{
			_userManager = userManager;
			_config = config;
		}
		public async Task SendEmail(string to, string subject, string htmlMessage)
		{
			var email = new MimeMessage();
			email.From.Add(MailboxAddress.Parse(_config["EmailSettings:Email"]));
			email.To.Add(MailboxAddress.Parse(to));
			email.Subject = subject;
			email.Body = new TextPart(TextFormat.Html) { Text = htmlMessage };

			using var smtp = new SmtpClient();
			await smtp.ConnectAsync(_config["EmailSettings:Host"],int.Parse(_config["EmailSettings:Port"]!),SecureSocketOptions.StartTls);
			await smtp.AuthenticateAsync(_config["EmailSettings:Email"], _config["EmailSettings:Password"]);
			await smtp.SendAsync(email);
			await smtp.DisconnectAsync(true);
		}
		public async Task<AuthResult> ConfirmEmailAsync(string userId, string token)
		{
			if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
				return new AuthResult { Success = false, Message = "Invalid email confirmation request" };

			var user = await _userManager.FindByIdAsync(userId);
			if (user == null)
				return new AuthResult { Success = false, Message = "User not found" };

			var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
			var result = await _userManager.ConfirmEmailAsync(user, decodedToken);

			if (!result.Succeeded)
				return new AuthResult { Success = false, Message = "Email confirmation failed" };

			return new AuthResult { Success = true, Message = "Email confirmed successfully!" };
		}
	}
}
