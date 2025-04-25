using DiplomAPI.Data;
using DiplomAPI.Models.Support;
using DiplomAPI.Models.UserModels;
using Finansu.Model;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace DiplomAPI.Controllers
{

    [Controller]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly UserKabinetService _userKabinetService;

        public UserController()
        {
            _userKabinetService = new UserKabinetService();
        }

        [HttpPost]
        public async Task<ActionResult<double>> Post_UserDataAsync([FromBody]int id) 
        {
            var resalt = await _userKabinetService.UserMoneyLoadAsync(id);
            if (resalt == null) return BadRequest(resalt.Maney);
            else return Ok(resalt.Maney);
        }

        [HttpGet("allInvestTool")]
        public async Task<ActionResult<List<InvestTools>>> Get_allInvestToolAsync()
        {
            return Ok(await _userKabinetService.AllToolsLoadAsync());
        }

        [HttpPost("UserSTools")]
        public async Task<ActionResult<List<Portfolio>>> Post_UserSToolsAsync([FromBody]int id)
        {
            return Ok(await _userKabinetService.UserSToolsAsync(id));
        }

        [HttpGet("Calculate")]
        public async Task<ActionResult<double?>> Get_CalculatedAsync([FromBody] int id, DateTime? dateStart, DateTime? dateFinish)
        {
            double? resalt = await _userKabinetService.Calculate(id, dateStart, dateFinish);
            if (resalt == null) return BadRequest(resalt);
            return Ok(resalt);
        }

        [HttpPost("loadCart")]
        public async Task<ActionResult<List<String[]>?>> Post_LoadCart([FromBody]int id)
        {
            var resalt = await _userKabinetService.LoadDataFormDatabaseAsync(id);
            if (resalt == null) return BadRequest(resalt);
            return Ok(resalt);
        }


        [HttpPost("updateMoneu")]
        public async Task<ActionResult<User>> UpdateMoneu([FromBody]MoneuUpdate moneuUpdate)
        {
            var resalt = await _userKabinetService._UpdateMoneu(moneuUpdate.id, moneuUpdate.sum, moneuUpdate.vector);
            return Ok(resalt);
        }
    }
}
