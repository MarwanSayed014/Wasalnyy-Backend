using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Stripe;
using Stripe.Checkout;
using Stripe.V2;
using System;
using Wasalnyy.BLL.DTO.Payment;
using Wasalnyy.BLL.DTO.Wallet;
using Wasalnyy.DAL.Entities;
using Wasalnyy.DAL.Repo.Abstraction;
using Wasalnyy.DAL.Repo.Implementation;

namespace Wasalnyy.BLL.Service.Implementation
{
	public class PaymentService : IPaymentService
	{
		private readonly IConfiguration config;
		private readonly IPaymentGetwayRepo paymentGetwayRepo;
        private readonly IMapper _mapper;
		private readonly IWalletService	walletService;
        private readonly RiderService riderService;
        private readonly IWalletTransactionRepo walletTransactionRepo;
        public PaymentService(IConfiguration config, IWalletTransactionRepo walletTransactionRepo,IPaymentGetwayRepo  paymentGetwayRepo, IMapper mapper, IWalletService walletService,RiderService riderService)
		{
			this.config = config;
			StripeConfiguration.ApiKey = config["Stripe:SecretKey"];
			this.paymentGetwayRepo = paymentGetwayRepo;
            this._mapper= mapper;
            this.walletTransactionRepo = walletTransactionRepo;
            this.walletService = walletService;
            this.riderService = riderService;
        }
		public async Task<string> CreatePaymentSession(decimal amount, string currency, string successUrl, string cancelUrl)
		{
			var options = new SessionCreateOptions
			{
				PaymentMethodTypes = new List<string> { "card" },
				LineItems = new List<SessionLineItemOptions>
			{
				new SessionLineItemOptions
				{
					PriceData = new SessionLineItemPriceDataOptions
					{
						UnitAmount = (long)(amount * 100), // amount in cents
                        Currency = currency,
						ProductData = new SessionLineItemPriceDataProductDataOptions
						{
							Name = "Ride Payment",
						},
					},
					Quantity = 1,
				},
			},
				Mode = "payment",
				SuccessUrl = successUrl,
				CancelUrl = cancelUrl
			};

			var service = new SessionService();
			var session = await service.CreateAsync(options);
			return session.Url;
		}

        public async Task<RiderPaymentSuccessResponse> HandleRiderPayment(RiderPaymentDetailsDTO paymentDetails)
        {
            if (paymentDetails.Status == PaymentStatus.Success)
                return await HandleSuccessfulPaymentAsync(paymentDetails);
            else
                return await HandleFailedPaymentAsync(paymentDetails);
        }
        private async Task<RiderPaymentSuccessResponse> HandleSuccessfulPaymentAsync(RiderPaymentDetailsDTO paymentDetails)
        {
            DateTime dateTime=DateTime.Now;

            var paymentEntity = _mapper.Map<GatewayPaymentTransactions>(paymentDetails);

             var transaction =await  paymentGetwayRepo.BeginTransactionAsync();
            try 
            {
                //1-check if this rider valid or not  
                var rider = await riderService.GetByIdAsync(paymentDetails.RiderId);
                if (rider == null)
                    return new RiderPaymentSuccessResponse(false, "Rider not found ");
                //2-check if this rider has a wallet 
                var riderWallet = await walletService.GetWalletByUserIdAsync(paymentDetails.RiderId);
                if (riderWallet == null)
                    return new RiderPaymentSuccessResponse(false, "Rider Wallet not found ");

                //3- save payment transaction in payment getway table as successful
                await paymentGetwayRepo.AddPaymentAsync(paymentEntity);

                //4-increase Wallet balance of the rider
                var walletResponse = await walletService.IncreaseWalletAsync(paymentDetails.RiderId, paymentDetails.Amount,dateTime);
                if (!walletResponse.IsSuccess)
                    return new RiderPaymentSuccessResponse(false, $"Error updating wallet: {walletResponse.Message}");


                //5- log wallet transaction
                await walletTransactionRepo.CreateAsync(new WalletTransactionLogs
                {
                    WalletId = riderWallet.Id,
                    Amount = paymentDetails.Amount,
                    TransactionType = DAL.Enum.WalletTransactionType.Credit,
                    Description = $"user charge his wallet by {paymentDetails.Amount}",
                    CreatedAt = dateTime

                });

                //6-save all changes
                await paymentGetwayRepo.SaveChangesAsync();

                //7-commit transaction
                await transaction.CommitAsync();

                return new RiderPaymentSuccessResponse(true, "Payment processed successfully.");
            }
            catch (Exception ex)
            { 
                
                 await transaction.RollbackAsync();
                var innerMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return new RiderPaymentSuccessResponse(false, $"Error processing payment: {innerMessage}");

            }













            ////1-check if this rider valid or not  
            //var rider = await riderService.GetByIdAsync(paymentDetails.RiderId);
            //if (rider == null)
            //    return new RiderPaymentSuccessResponse(false, "Rider not found ");

            //var paymentEntity = _mapper.Map<GatewayPaymentTransactions>(paymentDetails);
            ////1- save payment transaction in payment getway table as successful

            //try
            //{

            //    await paymentGetwayRepo.AddPaymentAsync(paymentEntity);
            //}
            //catch (Exception ex)
            //{
            //    var innerMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
            //    return new RiderPaymentSuccessResponse(false, $"Error saving payment: {innerMessage}");
            //}
            ////2-Increase Wallet balance 

            //var walletResponse = await walletService.IncreaseWalletAsync(paymentDetails.RiderId, paymentDetails.Amount,DateTime.Now);
            //if (!walletResponse.IsSuccess)
            //    return new RiderPaymentSuccessResponse(false, $"Error updating wallet: {walletResponse.Message}");

            //return new RiderPaymentSuccessResponse(true, "Payment processed successfully.");
        }

        private async Task<RiderPaymentSuccessResponse> HandleFailedPaymentAsync(RiderPaymentDetailsDTO paymentDetails)
        {
            var paymentEntity = _mapper.Map<GatewayPaymentTransactions>(paymentDetails);
            //save payment transaction in payment getway table as failed
            try
            {
                await paymentGetwayRepo.AddPaymentAsync(paymentEntity);
            }
            catch (Exception ex)
            {
                var innerMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return new RiderPaymentSuccessResponse(false, $"Error saving payment: {innerMessage}");
            }


            return new RiderPaymentSuccessResponse(true, "Payment failer is saved in Db successfully.");
        }
    }
}
