using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wasalnyy.DAL.Repo.Abstraction
{
    public interface IWalletTransactionRepo
    {
        Task CreateAsync(WalletTransaction transaction);
        Task<IEnumerable<WalletTransaction>> GetByWalletIdAsync(Guid walletId);
        Task<WalletTransaction?> GetByIdAsync(Guid transactionId);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
