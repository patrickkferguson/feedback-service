using System;
using System.Threading.Tasks;
using FeedbackService.Api.Domain;

namespace FeedbackService.Api.Data
{
    public interface IRatingCommentRepository
    {
        Task<RatingComment> Add(RatingComment ratingComment);
    }

    public class RatingCommentRepository : IRatingCommentRepository
    {
        public Task<RatingComment> Add(RatingComment ratingComment)
        {
            ratingComment.Id = Guid.NewGuid();

            return Task.FromResult(ratingComment);
        }
    }
}
