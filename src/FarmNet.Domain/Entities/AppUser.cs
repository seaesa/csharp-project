using Microsoft.AspNetCore.Identity;

namespace FarmNet.Domain.Entities;

public class AppUser : IdentityUser
{
    public string HoTen { get; set; } = string.Empty;
    public Guid? FarmId { get; set; }
    public Farm? Farm { get; set; }
}
