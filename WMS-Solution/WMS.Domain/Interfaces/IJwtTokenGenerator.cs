using WMS.Domain.Entities;

namespace WMS.Domain.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateToken(UserLogin user, string roleName);
}
