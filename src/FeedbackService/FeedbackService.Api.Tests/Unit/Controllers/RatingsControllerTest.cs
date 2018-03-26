using System;
using System.Threading.Tasks;
using AutoFixture;
using FeedbackService.Api.Controllers;
using FeedbackService.Api.Domain;
using FeedbackService.Api.Models;
using FeedbackService.Api.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace FeedbackService.Api.Tests.Unit.Controllers
{
    [TestFixture]
    public class RatingsControllerTest : UnitTest
    {
        [Test]
        public async Task GivenFailureResultFromServiceWhenAddRatingThenReturnBadRequest()
        {
            var ratingService = Fixture.Freeze<Mock<IRatingService>>();
            ratingService.Setup(m =>
                    m.AddRating(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<int>()))
                .ReturnsAsync(Result.ErrorResult(ErrorCodes.InvalidScore));

            var controller = CreateController(ratingService.Object);

            var result = await controller.AddRating(Fixture.Create<AddRatingRequest>());

            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Test]
        public async Task GivenSuccessResultFromServiceWhenAddRatingThenReturnOkRequest()
        {
            var ratingId = Guid.NewGuid();
            var ratingService = Fixture.Freeze<Mock<IRatingService>>();
            ratingService.Setup(m =>
                    m.AddRating(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<int>()))
                .ReturnsAsync(Result<Guid>.SuccessResult(ratingId));

            var controller = CreateController(ratingService.Object);

            var result = await controller.AddRating(Fixture.Create<AddRatingRequest>());

            result.Should().BeOfType<OkObjectResult>();
            result.As<OkObjectResult>().Value.Should().BeEquivalentTo(new AddRatingResponse() { RatingId = ratingId });
        }

        [Test]
        public async Task GivenNotFoundResultFromServiceWhenAddCommentThenReturnNotFound()
        {
            var ratingService = Fixture.Freeze<Mock<IRatingService>>();
            ratingService.Setup(m => m.AddComment(It.IsAny<Guid>(), It.IsAny<string>()))
                .ReturnsAsync(Result.ErrorResult(ErrorCodes.RatingNotFound));

            var controller = CreateController(ratingService.Object);

            var result = await controller.AddComment(Guid.NewGuid(), Fixture.Create<AddCommentRequest>());

            result.Should().BeOfType<NotFoundResult>();
        }

        [Test]
        public async Task GivenSuccessResultFromServiceWhenAddCommentThenReturnOk()
        {
            var ratingService = Fixture.Freeze<Mock<IRatingService>>();
            ratingService.Setup(m => m.AddComment(It.IsAny<Guid>(), It.IsAny<string>()))
                .ReturnsAsync(Result.SuccessResult);

            var controller = CreateController(ratingService.Object);

            var result = await controller.AddComment(Guid.NewGuid(), Fixture.Create<AddCommentRequest>());

            result.Should().BeOfType<OkResult>();
        }

        private RatingsController CreateController(IRatingService ratingService)
        {
            var controller = new RatingsController(ratingService); 
            return controller;
        }
    }
}
