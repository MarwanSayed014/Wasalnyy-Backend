using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wasalnyy.DAL.Repo.Abstraction
{
    public interface IWalletRepo
    {
        Task<Wallet?> GetByUserIdAsync(string userId);
        Task<Wallet?> GetByIdAsync(Guid walletId);
        Task CreateAsync(Wallet wallet);
        Task UpdateAsync(Wallet wallet);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
