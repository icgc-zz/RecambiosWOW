namespace RecambiosWOW.Core.Domain.ValueObjects;

public record MemberIdentity
{
    public string Email { get; }
    public string Username { get; }
    public string PasswordHash { get; }
    public bool IsVerified { get; }

    public MemberIdentity(string email, string username, string passwordHash, bool isVerified = false)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be empty", nameof(email));
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("Username cannot be empty", nameof(username));
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("Password hash cannot be empty", nameof(passwordHash));

        Email = email;
        Username = username;
        PasswordHash = passwordHash;
        IsVerified = isVerified;
    }
}