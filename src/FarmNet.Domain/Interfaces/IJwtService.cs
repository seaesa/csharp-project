using FarmNet.Domain.Entities;

namespace FarmNet.Domain.Interfaces;

public interface IJwtService
{
    string GenerateToken(AppUser user, IList<string> roles);
}
