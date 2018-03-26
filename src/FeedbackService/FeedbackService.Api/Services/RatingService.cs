using System;
using System.Threading.Tasks;
using FeedbackService.Api.Data;
using FeedbackService.Api.Domain;

namespace FeedbackService.Api.Services
{
    public interface IRatingService
    {
        Task<Result> AddRating(Guid deviceId, string ipAddress, Guid pageId, int score);

        Task<Result> AddComment(Guid ratingId, string comment);
    }

    public class RatingService : IRatingService
    {
        private readonly IRatingRepository _ratingRepository;
        private readonly IRatingCommentRepository _ratingCommentRepository;

        public RatingService(IRatingRepository ratingRepository, IRatingCommentRepository ratingCommentRepository)
        {
            _ratingRepository = ratingRepository;
            _ratingCommentRepository = ratingCommentRepository;
        }
        
        public async Task<Result> AddRating(Guid deviceId, string ipAddress, Guid pageId, int score)
        {
            if (score < 1 || score > 10)
            {
                return Result.ErrorResult(
                    ErrorCodes.InvalidScore,
                    new[] {"Score must be between 1 and 10."});
            }

            var rating = new Rating()
            {
                DeviceId = deviceId,
                IpAddress = ipAddress,
                PageId = pageId,
                Score = score
            };

            await _ratingRepository.Add(rating);

            return Result<Guid>.SuccessResult(rating.Id);
        }

        public async Task<Result> AddComment(Guid ratingId, string comment)
        {
            var rating = await _ratingRepository.Get(ratingId);

            if (rating == null)
            {
                return Result.ErrorResult(ErrorCodes.RatingNotFound);
            }

            var ratingComment = new RatingComment()
            {
                RatingId = ratingId,
                Comment = comment
            };

            await _ratingCommentRepository.Add(ratingComment);

            return Result.SuccessResult();
        }
    }
}
