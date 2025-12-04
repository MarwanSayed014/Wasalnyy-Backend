using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wasalnyy.DAL.Database;
using Wasalnyy.DAL.Repo.Abstraction;

namespace Wasalnyy.DAL.Repo.Implementation
{
    public class ReviewRepo : IReviewRepo
    {
        private readonly WasalnyyDbContext _context;

        public ReviewRepo(WasalnyyDbContext context)
        {
            _context = context;
        }

       
        public async Task AddAsync(Review review)
        {
            if (review == null)
                throw new ArgumentNullException(nameof(review), "Review cannot be null");

            await _context.Reviews.AddAsync(review);
        }


        public async Task<double> GetDriverAverageRatingAsync(string driverId)
        {
            if (string.IsNullOrEmpty(driverId))
                throw new ArgumentNullException("no driver found");

            var rating = await _context.Reviews
                .Where(e => e.DriverId == driverId && e.ReviewerType == ReviewerType.Rider)
                .OrderByDescending(e => e.CreatedAt)
                .Take(500)
                .Select(r => (double?)r.Stars)  
                .AverageAsync();

            return rating ?? 0;
        }




        public async Task<IEnumerable<Review>> GetDriverReviewsAsync(string driverId)
        {
            if (string.IsNullOrEmpty(driverId))
                throw new ArgumentNullException(nameof(driverId), "Driver ID cannot be null or empty");

            var reviews = await _context.Reviews
                .Where(e => e.DriverId == driverId && e.ReviewerType == ReviewerType.Rider)
                .OrderByDescending(e => e.CreatedAt)
                .AsNoTracking()
                .ToListAsync();

            return reviews;
        }

        
        // Get all reviews FOR a rider (driver reviews about rider)
        
        public async Task<IEnumerable<Review>> GetRiderReviewsAsync(string riderId)
        {
            if (string.IsNullOrEmpty(riderId))
                throw new ArgumentNullException(nameof(riderId), "Rider ID cannot be null or empty");

            var reviews = await _context.Reviews
                .Where(e => e.RiderId == riderId && e.ReviewerType == ReviewerType.Driver)
                .OrderByDescending(e => e.CreatedAt)
                .AsNoTracking()
                .ToListAsync();

            return reviews;
        }

        
        // Get average rating for a rider (last 500 reviews)
       
        public async Task<double> GetRiderAverageRatingAsync(string riderId)
        {
            if (string.IsNullOrEmpty(riderId))
                throw new ArgumentNullException(nameof(riderId), "Rider ID cannot be null or empty");

            var rating = await _context.Reviews
                .Where(e => e.RiderId == riderId && e.ReviewerType == ReviewerType.Driver)
                .OrderByDescending(e => e.CreatedAt)
                .Take(500)  
                .AverageAsync(e => e.Stars);

            return rating;
        }

        
     
       
        public async Task<bool> HasReviewedTripAsync(Guid tripId, string userId, ReviewerType reviewerType)
        {
            if (tripId == Guid.Empty)
                throw new ArgumentNullException(nameof(tripId), "Trip ID cannot be empty");

            if (string.IsNullOrEmpty(userId))
                throw new ArgumentNullException(nameof(userId), "User ID cannot be null or empty");

            return await _context.Reviews
                .AnyAsync(r =>
                    r.TripId == tripId &&
                    r.ReviewerType == reviewerType &&
                    (
                        (reviewerType == ReviewerType.Rider && r.RiderId == userId) ||
                        (reviewerType == ReviewerType.Driver && r.DriverId == userId)
                    )
                );
        }

        
       
        
        public async Task<Review> GetReviewById(Guid reviewId)
        {
            if (reviewId == Guid.Empty)
                throw new ArgumentNullException(nameof(reviewId), "Review ID cannot be empty");

            return await _context.Reviews.FindAsync(reviewId);
        }

        
       
       
        public async Task<Review> UpdateAsync(Review review)
        {
            if (review == null)
                throw new ArgumentNullException(nameof(review), "Review cannot be null");

            if (review.Id == Guid.Empty)
                throw new ArgumentNullException(nameof(review.Id), "Review ID cannot be empty");

            var existingReview = await _context.Reviews.FindAsync(review.Id);
            if (existingReview == null)
                throw new InvalidOperationException("Review not found");

            _context.Reviews.Update(review);
            return review;
        }

        
       
        
        public async Task DeleteAsync(Guid reviewId)
        {
            if (reviewId == Guid.Empty)
                throw new ArgumentNullException(nameof(reviewId), "Review ID cannot be empty");

            var review = await _context.Reviews.FindAsync(reviewId);
            if (review != null)
            {
                _context.Reviews.Remove(review);
            }
        }

        
      
       
        public Task<int> SaveChangesAsync()
        {
            return _context.SaveChangesAsync();
        }

       
        // Get reviews by a specific reviewer (for analytics-admin)
        
        public async Task<IEnumerable<Review>> GetReviewsByReviewerAsync(string reviewerId, ReviewerType reviewerType)
        {
            if (string.IsNullOrEmpty(reviewerId))
                throw new ArgumentNullException(nameof(reviewerId), "Reviewer ID cannot be null or empty");

            var reviews = await _context.Reviews
                .Where(e =>
                    (reviewerType == ReviewerType.Rider && e.RiderId == reviewerId) ||
                    (reviewerType == ReviewerType.Driver && e.DriverId == reviewerId)
                )
                .OrderByDescending(e => e.CreatedAt)
                .AsNoTracking()
                .ToListAsync();

            return reviews;
        }

       
       
        
        public async Task<ReviewStatistics> GetReviewStatisticsAsync(string userId, ReviewerType userType)
        {
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentNullException(nameof(userId), "User ID cannot be null or empty");

            IQueryable<Review> query;

            if (userType == ReviewerType.Driver)
            {
                query = _context.Reviews.Where(r => r.DriverId == userId && r.ReviewerType == ReviewerType.Rider);
            }
            else
            {
                query = _context.Reviews.Where(r => r.RiderId == userId && r.ReviewerType == ReviewerType.Driver);
            }

            var count = await query.CountAsync();
            var avgRating = count > 0 ? await query.AverageAsync(r => r.Stars) : 0;

            var ratingsDistribution = await query
                .GroupBy(r => r.Stars)
                .Select(g => new { Stars = g.Key, Count = g.Count() })
                .OrderBy(x => x.Stars)
                .ToListAsync();

            return new ReviewStatistics
            {
                TotalReviews = count,
                AverageRating = avgRating,
                RatingsDistribution = ratingsDistribution
                    .ToDictionary(x => (int)x.Stars, x => x.Count)
            };
        }

        
    }

   
 

   
}