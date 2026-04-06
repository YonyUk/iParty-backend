using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using NSubstitute;
using Users.Application.ValidationBehaviors;

namespace Tests.Unit.Users.Application.Behaviors;

public record TestRequest(string Name) : IRequest<TestResponse>;
public record TestResponse(string Result);

public class ValidationBehaviorTests
{
    private readonly TestRequest testRequest = new TestRequest("Request");
    private readonly TestResponse testResponse = new TestResponse("Success");

    private readonly RequestHandlerDelegate<TestResponse> next;
    public ValidationBehaviorTests()
    {
        next = Substitute.For<RequestHandlerDelegate<TestResponse>>();
        next().Returns(testResponse);
    }

    [Fact]
    public async Task TestHandleWithoutValidators()
    {
        var validators = Array.Empty<IValidator<TestRequest>>();
        var validationBehavior = new ValidationBehavior<TestRequest, TestResponse>(validators);

        var result = await validationBehavior.Handle(testRequest, next, CancellationToken.None);
        
        await next.Received(1).Invoke();
        result.Should().Be(testResponse);
    }

    [Fact]
    public async Task TestHandleWithValidatorsPassed() // tests the case when all validators pass
    {
        var validator = Substitute.For<IValidator<TestRequest>>();

        validator.ValidateAsync(
            Arg.Any<ValidationContext<TestRequest>>(),
            Arg.Any<CancellationToken>()
        ).Returns(new ValidationResult());

        var validators = new[] { validator };
        var validationBehavior = new ValidationBehavior<TestRequest, TestResponse>(validators);

        var result = await validationBehavior.Handle(testRequest, next, CancellationToken.None);

        await next.Received(1).Invoke();
        result.Should().Be(testResponse);
    }

    [Fact]
    public async Task TestHandleWithValidationFails() // tests case when one validator fails
    {
        var validationFailure = new ValidationFailure("field", "field is required");
        var validationResult = new ValidationResult(new[] { validationFailure });

        var validator = Substitute.For<IValidator<TestRequest>>();
        validator.ValidateAsync(
            Arg.Any<ValidationContext<TestRequest>>(),
            Arg.Any<CancellationToken>()
        ).Returns(validationResult);

        var validators= new[] { validator };
        var validationBehavior = new ValidationBehavior<TestRequest, TestResponse>(validators);

        var action = async () => await validationBehavior.Handle(testRequest, next, CancellationToken.None);

        await action.Should().ThrowAsync<ValidationException>();
        await next.DidNotReceive().Invoke();
    }

    [Fact]
    public async Task TestHandleWithManyValidationFails() // tests case when many validators fails
    {
        var validationFailure1 = new ValidationFailure("field", "field is required");
        var validationFailure2 = new ValidationFailure("field", "required");

        var validator1 = Substitute.For<IValidator<TestRequest>>();
        validator1.ValidateAsync(
            Arg.Any<ValidationContext<TestRequest>>(),
            Arg.Any<CancellationToken>()
        ).Returns(new ValidationResult(new[] { validationFailure1 }));

        var validator2 = Substitute.For<IValidator<TestRequest>>();
        validator2.ValidateAsync(
            Arg.Any<ValidationContext<TestRequest>>(),
            Arg.Any<CancellationToken>()
        ).Returns(new ValidationResult(new[] { validationFailure2 }));

        var validators = new[] { validator1, validator2 };
        var validationBehavior = new ValidationBehavior<TestRequest, TestResponse>(validators);

        var action = async () => await validationBehavior.Handle(testRequest, next, CancellationToken.None);

        var exception = await action.Should().ThrowAsync<ValidationException>();
        exception.Which.Errors.Should().HaveCount(2);
        await next.DidNotReceive().Invoke();
    }
}