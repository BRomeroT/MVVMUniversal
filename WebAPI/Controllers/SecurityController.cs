using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebAPI.BL;

namespace WebAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class SecurityController : ControllerBase
    {
        readonly SecurityBL securityBL;
        public SecurityController(SecurityBL securityBL) => this.securityBL = securityBL;

        [HttpPost("Login")]
        public IActionResult Login(SharedAPIModel.Credential credential)
        {
            var (IsValid, Name) = securityBL.ValidateUser(credential.User, credential.Password);
            if (IsValid) return Ok(Name);
            return Unauthorized(); //StatusCode(StatusCodes.Status401Unauthorized);
        }
    }
}
