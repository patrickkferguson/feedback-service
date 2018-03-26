using System;

namespace FeedbackService.Api.Domain
{
    public class RatingComment
    {
        public Guid Id { get; set; }

        public Guid RatingId { get; set; }

        public string Comment { get; set; }
    }
}
