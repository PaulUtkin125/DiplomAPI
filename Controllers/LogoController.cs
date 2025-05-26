using DiplomAPI.Data;
using DiplomAPI.Models.LogoModels;
using DiplomAPI.Models.Support;
using Finansu.Model;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DiplomAPI.Controllers
{
    [Route("api/[controller]")]
    [Controller]
    public class LogoController : ControllerBase
    {
        private readonly UserLogoService _userLogoService;
        private static IConfiguration _configuration;
        public LogoController(IConfiguration configuration)
        {
            _configuration = configuration;
            _userLogoService = new UserLogoService(_configuration);
        }

        [HttpPost("validationMail")]
        public async Task<ActionResult<bool>> Post_validationMail([FromBody] MailRequest request)
        {
            using (var context = new dbContact(_configuration))
            {
                var user_Exist = await context.User.FirstOrDefaultAsync(x => x.Loggin == request.ToMail);
                if (user_Exist != null)
                { // такой пользователь существует
                    return BadRequest(false);
                }
                else return Ok(true);
            }
        }

        [HttpPost("validationBrokerMail")]
        public async Task<ActionResult<bool>> validationBrokerMail([FromBody] MailRequest request)
        {
            using (var context = new dbContact(_configuration))
            {
                var user_Exist = await context.Brokers.FirstOrDefaultAsync(x => x.Email == request.ToMail);
                if (user_Exist != null)
                { // такой пользователь существует
                    return Ok(false);
                }
                else return Ok(true);
            }
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

        [HttpPost("AutorisationAdmin")]
        public async Task<ActionResult<int>> Post_AdminAutorisation([FromBody] StartUserData user)
        {
            int? resalt = await _userLogoService.AdminAutorisationAsync(user.Login, user.Password);
            if (resalt != null) return Ok(resalt);
            else return BadRequest(0);
        }

        [HttpPost("AutorisationBroker")]
        public async Task<ActionResult<int>> AutorisationBroker([FromBody] StartUserData user)
        {
            int? resalt = await _userLogoService.BrokerAutorisationAsync(user.Login, user.Password);
            if (resalt != null) return Ok(resalt);
            else return BadRequest(0);
        }
    }
}
