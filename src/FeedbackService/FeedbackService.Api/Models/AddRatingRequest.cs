using System;

namespace FeedbackService.Api.Models
{
    public class AddRatingRequest
    {
        public Guid DeviceId { get; set; }

        public Guid PageId { get; set; }

        public int Score { get; set; }
    }
}
