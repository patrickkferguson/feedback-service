using System;
using System.Threading.Tasks;
using AutoFixture;
using FeedbackService.Api.Data;
using FeedbackService.Api.Domain;
using FeedbackService.Api.Services;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace FeedbackService.Api.Tests.Unit.Services
{
    [TestFixture]
    public class RatingServiceTest : UnitTest
    {
        [TestCase(-1)]
        [TestCase(0)]
        [TestCase(11)]
        public async Task GivenInvalidScoreWhenAddRatingThenReturnErrorResult(int score)
        {
            var service = Fixture.Create<RatingService>();

            var result = await service.AddRating(Guid.NewGuid(), "ip", Guid.NewGuid(), score);

            result.Success.Should().BeFalse();
            result.ErrorCodes.Should().HaveFlag(ErrorCodes.InvalidScore);
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        [TestCase(5)]
        [TestCase(6)]
        [TestCase(7)]
        [TestCase(8)]
        [TestCase(9)]
        [TestCase(10)]
        public async Task GivenValidScoreWhenAddRatingThenReturnSuccessResult(int score)
        {
            var service = Fixture.Create<RatingService>();

            var result = await service.AddRating(Guid.NewGuid(), "ip", Guid.NewGuid(), score);

            result.Success.Should().BeTrue();
            result.ErrorCodes.Should().Be(ErrorCodes.None);
        }

        [Test]
        public async Task GivenValidScoreWhenAddRatingThenAddRatingToRepository()
        {
            var deviceId = Guid.NewGuid();
            var ipAddress = Fixture.Create<string>();
            var pageId = Guid.NewGuid();
            var score = 1;

            var repository = Fixture.Freeze<Mock<IRatingRepository>>();
            var service = Fixture.Create<RatingService>();

            await service.AddRating(deviceId, ipAddress, pageId, score);

            repository.Verify(m => m.Add(It.Is<Rating>(r => r.DeviceId == deviceId && r.IpAddress == ipAddress && r.PageId == pageId && r.Score == score)), Times.Exactly(1));
        }

        [Test]
        public async Task GivenRatingAddedToRepositoryWhenAddRatingThenReturnRatingId()
        {
            var ratingId = Guid.NewGuid();
            var repository = Fixture.Freeze<Mock<IRatingRepository>>();
            repository.Setup(m => m.Add(It.IsAny<Rating>())).Returns<Rating>(r =>
            {
                r.Id = ratingId;
                return Task.FromResult(r);
            });

            var service = Fixture.Create<RatingService>();

            var result = await service.AddRating(Guid.NewGuid(), "ip", Guid.NewGuid(), 1);

            result.Should().BeOfType<Result<Guid>>();
            result.As<Result<Guid>>().Data.Should().Be(ratingId);
        }

        [Test]
        public async Task GivenNoRatingFoundInRepositoryWhenAddCommentThenReturnErrorResult()
        {
            var ratingRepository = Fixture.Freeze<Mock<IRatingRepository>>();
            ratingRepository.Setup(m => m.Get(It.IsAny<Guid>())).ReturnsAsync(null as Rating);

            var service = Fixture.Create<RatingService>();

            var result = await service.AddComment(Guid.NewGuid(), Fixture.Create<string>());

            result.Success.Should().BeFalse();
            result.ErrorCodes.Should().HaveFlag(ErrorCodes.RatingNotFound);
        }

        [Test]
        public async Task GivenRatingFoundInRepositoryWhenAddCommentThenAddCommentToRepository()
        {
            var rating = Fixture.Create<Rating>();
            var comment = Fixture.Create<string>();
            var ratingRepository = Fixture.Freeze<Mock<IRatingRepository>>();
            ratingRepository.Setup(m => m.Get(It.IsAny<Guid>())).ReturnsAsync(rating);

            var commentRepository = Fixture.Freeze<Mock<IRatingCommentRepository>>();

            var service = Fixture.Create<RatingService>();

            await service.AddComment(rating.Id, comment);

            commentRepository.Verify(m => m.Add(It.Is<RatingComment>(c => c.RatingId == rating.Id && c.Comment == comment)), Times.Exactly(1));
        }

        [Test]
        public async Task GivenCommentAddedToRepositoryWhenAddCommentThenReturnSuccessResult()
        {
            var rating = Fixture.Create<Rating>();
            var comment = Fixture.Create<string>();
            var ratingRepository = Fixture.Freeze<Mock<IRatingRepository>>();
            ratingRepository.Setup(m => m.Get(It.IsAny<Guid>())).ReturnsAsync(rating);

            var service = Fixture.Create<RatingService>();

            var result = await service.AddComment(rating.Id, comment);

            result.Success.Should().BeTrue();
            result.ErrorCodes.Should().Be(ErrorCodes.None);
        }
    }
}
