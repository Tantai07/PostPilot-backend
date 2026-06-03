using Microsoft.EntityFrameworkCore;
using PostPilot.Api.Features.Auth.Dtos;
using PostPilot.Infrastructure.Auth;
using PostPilot.Infrastructure.Database;

namespace PostPilot.Api.Features.Auth;

public sealed class LoginCommand
{
    private readonly AppDbContext _dbContext;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenService _jwtTokenService;

    public LoginCommand(AppDbContext dbContext, IPasswordHasher passwordHasher, IJwtTokenService jwtTokenService)
    {
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<LoginResponseDto?> ExecuteAsync(LoginRequestDto request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
        {
            return null;
        }

        var email = request.Email.Trim().ToLowerInvariant();
        var adminUser = await _dbContext.AdminUsers
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Email == email && x.IsActive, cancellationToken);

        if (adminUser is null || !_passwordHasher.Verify(request.Password, adminUser.PasswordHash))
        {
            return null;
        }

        return _jwtTokenService.CreateLoginResponse(adminUser);
    }
}
