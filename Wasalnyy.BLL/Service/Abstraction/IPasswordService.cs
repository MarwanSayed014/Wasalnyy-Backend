using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wasalnyy.BLL.Service.Abstraction
{
	public interface IPasswordService
	{
		Task<bool> ForgotPasswordAsync(ForgotPasswordDto dto);
		Task<(bool Success, IEnumerable<string> Errors)> ResetPasswordAsync(ResetPasswordDto dto);
	}
}
