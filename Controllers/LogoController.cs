using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DiplomAPI.Models.LogoModels;
using DiplomAPI.Models.Support;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DiplomAPI.Controllers
{
    [Route("api/[controller]")]
    [Controller]
    public class LogoController : ControllerBase
    {
        private readonly UserLogoService _userLogoService;

        public LogoController()
        {
            _userLogoService = new UserLogoService();
        }

        [HttpPost("UserAdd")]
        public async Task<ActionResult<bool>> Post_userAdd([FromBody] StartUserData user)
        {
            bool resalt = await _userLogoService.UserRigistrationAsinc(user.Login, user.Password);
            if (resalt) return Ok(true);
            else return BadRequest(false);
             
        }

        [HttpPost("Autorisation")]
        public async Task<ActionResult<int>> Post_UserAutorisation([FromBody] StartUserData user)
        {
            int? resalt = await _userLogoService.UserAutorisationAsync(user.Login, user.Password);
            if (resalt != null) return Ok(resalt);
            else return BadRequest(0);
        }
    }
}
