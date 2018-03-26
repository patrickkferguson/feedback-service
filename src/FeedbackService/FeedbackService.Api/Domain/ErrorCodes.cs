using System;

namespace FeedbackService.Api.Domain
{
    [Flags]
    public enum ErrorCodes
    {
        None = 0,
        InvalidScore = 1,
        RatingNotFound = 2
    }
}
