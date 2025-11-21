using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wasalnyy.DAL.Database;
using Wasalnyy.DAL.Repo.Abstraction;

namespace Wasalnyy.DAL.Repo.Implementation
{
    public class WalletTransactionRepo : IWalletTransactionRepo
    {
        private readonly WasalnyyDbContext _context;

        public WalletTransactionRepo(WasalnyyDbContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(WalletTransaction transaction)
        {
            await _context.WalletTransactions.AddAsync(transaction);
        }

        public async Task<WalletTransaction?> GetByIdAsync(Guid transactionId)
        {
            return await _context.WalletTransactions
                .AsNoTracking()
                .SingleOrDefaultAsync(t => t.Id == transactionId);
        }

        public async Task<IEnumerable<WalletTransaction>> GetByWalletIdAsync(Guid walletId)
        {
            return await _context.WalletTransactions
                .AsNoTracking()
                .Where(t => t.WalletId == walletId)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
