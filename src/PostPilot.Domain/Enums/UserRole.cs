using System.Text.Json.Serialization;

namespace PostPilot.Domain.Enums;

[JsonConverter(typeof(JsonStringEnumConverter<UserRole>))]
public enum UserRole
{
    User = 1,
    Admin = 2
}
