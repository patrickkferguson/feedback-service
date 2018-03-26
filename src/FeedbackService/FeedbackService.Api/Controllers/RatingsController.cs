using System;
using System.Threading.Tasks;
using FeedbackService.Api.Domain;
using FeedbackService.Api.Models;
using Microsoft.AspNetCore.Mvc;
using FeedbackService.Api.Services;

namespace FeedbackService.Api.Controllers
{
    [Route("ratings")]
    public class RatingsController : Controller
    {
        private readonly IRatingService _ratingService;

        public RatingsController(IRatingService ratingService)
        {
            _ratingService = ratingService;
        }

        [HttpPost]
        public async Task<IActionResult> AddRating([FromBody]AddRatingRequest addRatingRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var ipAddress = Request?.HttpContext?.Connection?.RemoteIpAddress?.ToString();

            var ratingResult = await _ratingService.AddRating(addRatingRequest.DeviceId, ipAddress, addRatingRequest.PageId, addRatingRequest.Score);

            if (!ratingResult.Success)
            {
                return BadRequest(ratingResult.Errors);
            }

            var ratingId = ((Result<Guid>) ratingResult).Data;

            return Ok(new AddRatingResponse() { RatingId = ratingId });

        }

        [HttpPost("{ratingId}/comments")]
        public async Task<IActionResult> AddComment(Guid ratingId, [FromBody]AddCommentRequest addCommentRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _ratingService.AddComment(ratingId, addCommentRequest.Comment);

            if (result.ErrorCodes.HasFlag(ErrorCodes.RatingNotFound))
            {
                return NotFound();
            }

            return Ok();
        }
    }
}
