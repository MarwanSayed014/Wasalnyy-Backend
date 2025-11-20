using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Wasalnyy.PL.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class PasswordResetController : ControllerBase
	{
		private readonly IPasswordService _passService;
		public PasswordResetController(IPasswordService passService)
		{
			_passService = passService;
		}

		[HttpPost("forgot-password")]
		public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
		{
			await _passService.ForgotPasswordAsync(dto);
			return Ok("A reset link was sent.");
		}

		[HttpPost("reset-password")]
		public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
		{
			var result = await _passService.ResetPasswordAsync(dto);
			if (!result.Success)
				return BadRequest("Invalid token or user.");
			return Ok("Password reset successfully.");
		}
	}
}
