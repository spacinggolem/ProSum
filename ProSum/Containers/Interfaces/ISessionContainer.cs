using Microsoft.AspNetCore.Http;
using ProSum.Models;

namespace ProSum.Containers.Interfaces
{
    public interface ISessionContainer
    {
        public Session CreateSession(User user, IHttpContextAccessor contextAccessor);
        public Session GetSession(IHttpContextAccessor contextAccessor);
        public void DeleteSession(IHttpContextAccessor contextAccessor);
        public bool HasAccess(IHttpContextAccessor contextAccessor, RolesEnum[] roles);
        public bool HasAccess(IHttpContextAccessor contextAccessor, RolesEnum role);
        public bool IsLoggedIn(IHttpContextAccessor contextAccessor);
        public bool IsProjectEmployee(IHttpContextAccessor contextAccessor, Project project);
        public bool IsProjectManager(IHttpContextAccessor contextAccessor, Project project);
    }
}
