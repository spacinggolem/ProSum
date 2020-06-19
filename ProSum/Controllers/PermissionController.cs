using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProSum.Containers.Interfaces;
using ProSum.DTO;

namespace ProSum.Controllers
{
    public class PermissionController : Controller
    {
        IPermissionContainer container;
        public PermissionController(IPermissionContainer container)
        {
            this.container = container;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public IActionResult Create([FromForm] PermissionCreateDTO permissionInfo)
        {
            container.CreatePermission(permissionInfo.Name);
            return Created(nameof(Get), permissionInfo.Name);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult Get()
        {
            return Ok(container.Permissions);
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}