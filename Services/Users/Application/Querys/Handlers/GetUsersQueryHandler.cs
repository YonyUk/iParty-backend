using AutoMapper;
using MediatR;
using Users.Application.DTOs;
using Users.Domain;

namespace Users.Application.Querys.Handlers;

public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, IEnumerable<UserDTO>>
{
    private readonly IUserRepository userRepository;
    private readonly IMapper mapper;
    public GetUsersQueryHandler(IUserRepository userRepository, IMapper mapper)
    {
        this.userRepository = userRepository;
        this.mapper = mapper;
    }
    public async Task<IEnumerable<UserDTO>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await userRepository.GetUsers(cancellationToken);
        return mapper.Map<IEnumerable<UserDTO>>(users);
    }
}