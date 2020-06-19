using ProSum.Models;
using System;
using System.Collections.Generic;

namespace ProSum.Services.Interfaces
{
    public interface IUserService
    {
        public User Login(User user);

        public List<User> GetAll();

        public User Get(Guid id);
        public User GetByEmail(string email);

        public List<User> GetAllWithRole(RolesEnum role);
        public void Update(User user);
        void Delete(User user);
    }
}
