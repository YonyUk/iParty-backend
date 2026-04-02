using Common.Domain;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Users.Application.Commands;
using Users.Application.Commands.Handlers;
using Users.Domain;
using Users.Domain.Exceptions;

namespace Tests.Unit.Users.Application.Commands.Handlers;

public class UnRegisterUserCommandHandlerTests
{
    private readonly IUserRepository userRepository;
    private readonly IUnitOfWork unitOfWork;
    public UnRegisterUserCommandHandlerTests()
    {
        userRepository = Substitute.For<IUserRepository>();
        unitOfWork = Substitute.For<IUnitOfWork>();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task TestUnregisterUser(bool userExists)
    {
        if (!userExists)
            userRepository.Delete(Arg.Any<Guid>()).ThrowsAsync(new UserNotFoundException("id", "id"));

        var command = new UnRegisterUserCommand(Guid.NewGuid());
        var handler = new UnRegisterUserCommandHandler(userRepository, unitOfWork);

        var action = async () =>
        {
            await handler.Handle(command, CancellationToken.None);
            await userRepository.Received(1).Delete(Arg.Any<Guid>(),Arg.Any<CancellationToken>());            
            await unitOfWork.Received(1).Commit(Arg.Any<CancellationToken>());
        };

        if (!userExists)
            await action.Should().ThrowAsync<UserNotFoundException>();
        else
            await action.Should().NotThrowAsync();

    }
}