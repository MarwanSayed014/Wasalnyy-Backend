
using Wasalnyy.DAL.Database;
using Wasalnyy.DAL.Repo.Abstraction;


namespace Wasalnyy.BLL.Service.Implementation
{
    public class WalletService : IWalletService
    {
        private readonly IWalletRepo _walletRepo;
        private readonly IWalletTransactionRepo _transactionRepo;
        private readonly WasalnyyDbContext _context;

        public WalletService(
            IWalletRepo walletRepo,
            IWalletTransactionRepo transactionRepo,
            WasalnyyDbContext context)
        {
            _walletRepo = walletRepo;
            _transactionRepo = transactionRepo;
            _context = context;
        }

        // ============================================================
        //   GET WALLET
        // ============================================================

        public async Task<Wallet?> GetWalletByUserIdAsync(string userId)
        {
            return await _walletRepo.GetByUserIdAsync(userId);
        }


        // ============================================================
        //   ADD MONEY TO WALLET
        // ============================================================

        public async Task<bool> AddToWalletAsync(string userId, decimal amount, string? reference = null)
        {
            if (amount <= 0)
                return false;

            using var transaction = await _context.Database.BeginTransactionAsync();

            var wallet = await _walletRepo.GetByUserIdAsync(userId);

            // Auto-create wallet if none exists
            if (wallet == null)
            {
                wallet = new Wallet
                {
                    UserId = userId,
                    Balance = 0,
                    CreatedAt = DateTime.UtcNow
                };

                await _walletRepo.CreateAsync(wallet);
            }

            wallet.Balance += amount;
            wallet.ModifiedAt = DateTime.UtcNow;

            await _walletRepo.UpdateAsync(wallet);

            var log = new WalletTransaction
            {
                WalletId = wallet.Id,
                Amount = amount,
                TransactionType = WalletTransactionType.Credit,
                Description = reference ?? "Wallet Top-Up",
                CreatedAt = DateTime.UtcNow
            };

            await _transactionRepo.CreateAsync(log);

            await _walletRepo.SaveChangesAsync();
            await _transactionRepo.SaveChangesAsync();

            await transaction.CommitAsync();
            return true;
        }


        // ============================================================
        //   WITHDRAW MONEY
        // ============================================================

        public async Task<bool> WithdrawFromWalletAsync(string userId, decimal amount, string? reference = null)
        {
            if (amount <= 0)
                return false;

            using var transaction = await _context.Database.BeginTransactionAsync();

            var wallet = await _walletRepo.GetByUserIdAsync(userId);
            if (wallet == null)
                return false;

            if (wallet.Balance < amount)
                return false; // not enough money

            wallet.Balance -= amount;
            wallet.ModifiedAt = DateTime.UtcNow;

            await _walletRepo.UpdateAsync(wallet);

            var log = new WalletTransaction
            {
                WalletId = wallet.Id,
                Amount = amount,
                TransactionType = WalletTransactionType.Debit,
                Description = reference ?? "Wallet Withdrawal",
                CreatedAt = DateTime.UtcNow
            };

            await _transactionRepo.CreateAsync(log);

            await _walletRepo.SaveChangesAsync();
            await _transactionRepo.SaveChangesAsync();

            await transaction.CommitAsync();
            return true;
        }


        // ============================================================
        //   TRANSFER (Rider -> Driver)
        // ============================================================

        public async Task<bool> TransferAsync(string fromUserId, string toUserId, decimal amount, string? tripId = null)
        {
            if (amount <= 0)
                return false;

            using var transaction = await _context.Database.BeginTransactionAsync();

            var senderWallet = await _walletRepo.GetByUserIdAsync(fromUserId);
            var receiverWallet = await _walletRepo.GetByUserIdAsync(toUserId);

            if (senderWallet == null || receiverWallet == null)
                return false;

            if (senderWallet.Balance < amount)
                return false;

            // Sender
            senderWallet.Balance -= amount;
            senderWallet.ModifiedAt = DateTime.UtcNow;
            await _walletRepo.UpdateAsync(senderWallet);

            var senderLog = new WalletTransaction
            {
                WalletId = senderWallet.Id,
                Amount = amount,
                TransactionType = WalletTransactionType.Debit,
                Description = $"Trip Payment {tripId}",
                CreatedAt = DateTime.UtcNow
            };
            await _transactionRepo.CreateAsync(senderLog);


            // Receiver
            receiverWallet.Balance += amount;
            receiverWallet.ModifiedAt = DateTime.UtcNow;
            await _walletRepo.UpdateAsync(receiverWallet);

            var receiverLog = new WalletTransaction
            {
                WalletId = receiverWallet.Id,
                Amount = amount,
                TransactionType = WalletTransactionType.Credit,
                Description = $"Trip Payment Received {tripId}",
                CreatedAt = DateTime.UtcNow
            };
            await _transactionRepo.CreateAsync(receiverLog);

            await _walletRepo.SaveChangesAsync();
            await _transactionRepo.SaveChangesAsync();

            await transaction.CommitAsync();
            return true;
        }
    }
}
