using DiplomAPI.Models.LogoModels;
using DiplomAPI.Models.Support;
using Microsoft.AspNetCore.Mvc;

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
            
           return await _userLogoService.UserRigistrationAsinc(user.Login, user.Password);
             
        }

        [HttpPost("Autorisation")]
        public async Task<ActionResult<int?>> Post_UserAutorisation([FromBody] StartUserData user)
        {
            return await _userLogoService.UserAutorisationAsync(user.Login, user.Password);
        }
    }
}
