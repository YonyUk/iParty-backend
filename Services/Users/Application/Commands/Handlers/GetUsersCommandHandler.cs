using AutoMapper;
using MediatR;
using Users.Application.DTOs;
using Users.Domain;

namespace Users.Application.Commands.Handlers;

public class GetUsersCommandHandler : IRequestHandler<GetUsersCommand, IEnumerable<UserDTO>>
{
    private readonly IUserRepository userRepository;
    private readonly IMapper mapper;
    public GetUsersCommandHandler(IUserRepository userRepository, IMapper mapper)
    {
        this.userRepository = userRepository;
        this.mapper = mapper;
    }
    public async Task<IEnumerable<UserDTO>> Handle(GetUsersCommand request, CancellationToken cancellationToken)
    {
        var users = await userRepository.GetUsers();
        return mapper.Map<IEnumerable<UserDTO>>(users);
    }
}