using AutoMapper;
using MediatR;
using Users.Application.DTOs;
using Users.Domain;
using Users.Domain.ValueObjects;

namespace Users.Application.Commands.Handlers;

public class GetUserByEmailCommandHandler : IRequestHandler<GetUserByEmailCommand, UserDTO>
{
    private readonly IUserRepository userRepository;
    private readonly IMapper mapper;
    public GetUserByEmailCommandHandler(IUserRepository userRepository,IMapper mapper)
    {
        this.userRepository = userRepository;
        this.mapper = mapper;
    }
    public async Task<UserDTO> Handle(GetUserByEmailCommand request, CancellationToken cancellationToken)
    {
        var email = new Email(request.Email);
        var user = await userRepository.GetByEmail(email,cancellationToken);
        return mapper.Map<UserDTO>(user);
    }
}