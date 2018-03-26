using System;

namespace FeedbackService.Api.Domain
{
    public class Rating
    {
        public Guid Id { get; set; }

        public Guid DeviceId { get; set; }

        public string IpAddress { get; set; }

        public Guid PageId { get; set; }

        public int Score { get; set; }
    }
}
