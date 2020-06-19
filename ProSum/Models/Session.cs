using System;

namespace ProSum.Models
{
    public class Session
    {
        public Guid Id { get; set; }
        public User User { get; set; }
        public DateTime ExpiresAt { get; private set; }
        public Session(User user, DateTime expiresAt)
        {
            Id = Guid.NewGuid();
            User = user;
            ExpiresAt = expiresAt;
        }

        public bool IsExpired()
        {
            return ExpiresAt <= DateTime.Now;
        }

        public bool HasAccess(RolesEnum role)
        {
            return User.Role.Name == role.ToString();
        }
    }
}
