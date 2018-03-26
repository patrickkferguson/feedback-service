using System;
using System.Threading.Tasks;
using FeedbackService.Api.Domain;

namespace FeedbackService.Api.Data
{
    public interface IRatingRepository
    {
        Task<Rating> Add(Rating rating);

        Task<Rating> Get(Guid ratingId);
    }

    public class RatingRepository : IRatingRepository
    {
        public Task<Rating> Add(Rating rating)
        {
            rating.Id = Guid.NewGuid();

            return Task.FromResult(rating);
        }

        public Task<Rating> Get(Guid ratingId)
        {
            var rating = new Rating() { Id = ratingId };
            return Task.FromResult(rating);
        }
    }
}
