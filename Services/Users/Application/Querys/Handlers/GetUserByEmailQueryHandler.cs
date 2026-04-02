using AutoMapper;
using MediatR;
using Users.Application.DTOs;
using Users.Domain;
using Users.Domain.ValueObjects;

namespace Users.Application.Querys.Handlers;

public class GetUserByEmailQueryHandler : IRequestHandler<GetUserByEmailQuery, UserDTO>
{
    private readonly IUserRepository userRepository;
    private readonly IMapper mapper;
    public GetUserByEmailQueryHandler(IUserRepository userRepository,IMapper mapper)
    {
        this.userRepository = userRepository;
        this.mapper = mapper;
    }
    public async Task<UserDTO> Handle(GetUserByEmailQuery request, CancellationToken cancellationToken)
    {
        var email = new Email(request.Email);
        var user = await userRepository.GetByEmail(email,cancellationToken);
        return mapper.Map<UserDTO>(user);
    }
}