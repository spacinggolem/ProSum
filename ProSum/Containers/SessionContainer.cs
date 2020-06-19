using Microsoft.AspNetCore.Http;
using ProSum.Containers.Interfaces;
using ProSum.Models;
using ProSum.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProSum.Containers
{
    public class SessionContainer : ISessionContainer
    {
        private List<Session> sessions;
        private int keepAliveMinutes;
        private readonly ProjectService _projectService;

        public SessionContainer(ProjectService projectService)
        {
            _projectService = projectService;
            sessions = new List<Session>();
            keepAliveMinutes = 30;
        }
        public Session CreateSession(User user, IHttpContextAccessor contextAccessor)
        {
            DateTime expiresAt = DateTime.Now;
            expiresAt = expiresAt.AddMinutes(keepAliveMinutes);
            Session newSession = new Session(user, expiresAt);
            sessions.Add(newSession);

            CookieOptions options = new CookieOptions();
            options.Expires = newSession.ExpiresAt;
            options.IsEssential = true;
            contextAccessor.HttpContext.Response.Cookies.Append("session", newSession.Id.ToString(), options);
            return newSession;
        }

        private void DeleteSession(Session session, HttpResponse response)
        {
            sessions.Remove(session);
            response.Cookies.Delete("session");
        }

        public void DeleteSession(IHttpContextAccessor contextAccessor)
        {
            if (!string.IsNullOrEmpty(contextAccessor.HttpContext.Request.Cookies["session"]))
            {
                string sessionId = contextAccessor.HttpContext.Request.Cookies["session"];
                Session session = sessions.FirstOrDefault(s => s.Id == Guid.Parse(sessionId));
                if (session != null)
                {

                    DeleteSession(session, contextAccessor.HttpContext.Response);
                }
            }
        }

        public Session GetSession(IHttpContextAccessor contextAccessor)
        {
            if (!string.IsNullOrEmpty(contextAccessor.HttpContext.Request.Cookies["session"]))
            {
                string sessionId = contextAccessor.HttpContext.Request.Cookies["session"];
                Session session = sessions.FirstOrDefault(s => s.Id == Guid.Parse(sessionId));
                if (session != null)
                {
                    if (session.IsExpired())
                    {
                        DeleteSession(session, contextAccessor.HttpContext.Response);
                        return null;
                    }
                    else
                    {
                        _projectService.GetProjectPermissions(session.User);
                        return session;
                    }
                }
                else
                {
                    contextAccessor.HttpContext.Response.Cookies.Delete("session");
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        public bool IsLoggedIn(IHttpContextAccessor contextAccessor)
        {
            Session session = GetSession(contextAccessor);

            return session != null;
        }

        public bool HasAccess(IHttpContextAccessor contextAccessor, RolesEnum[] roles)
        {
            Session session = GetSession(contextAccessor);

            if (session != null)
            {
                return roles.Any(Role => session.HasAccess(Role) || session.HasAccess(RolesEnum.Admin));
            }
            return false;

        }

        public bool HasAccess(IHttpContextAccessor contextAccessor, RolesEnum role)
        {
            Session session = GetSession(contextAccessor);

            if (session != null)
            {
                return session.HasAccess(role);
            }
            return false;

        }

        public bool IsProjectEmployee(IHttpContextAccessor contextAccessor, Project project)
        {
            Session session = GetSession(contextAccessor);

            if (session != null)
            {
                Permission permission = new Permission()
                {
                    Name = "ProjectEmployee"
                };

                return session.User.HasPermission(project, permission);
            }
            return false;
        }

        public bool IsProjectManager(IHttpContextAccessor contextAccessor, Project project)
        {
            Session session = GetSession(contextAccessor);

            if (session != null)
            {
                Permission permission = new Permission()
                {
                    Name = "ProjectManager"
                };

                return session.User.HasPermission(project, permission);
            }
            return false;
        }


    }
}

