using DiplomAPI.Data;
using DiplomAPI.Models.UserModels;
using Finansu.Model;
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

        [HttpGet("{id}")]
        public IActionResult Get_UserData(int id) 
        {
            using (var context = new dbContact())
            {
                return Ok(context.User.Find(id));
            }
        }

        [HttpGet("allInvestTool")]
        public async Task<ActionResult<List<InvestTools>>> Get_allInvestToolAsync()
        {
            return Ok(await _userKabinetService.AllToolsLoadAsync());
        }

        [HttpGet("UserSTools/{id}")]
        public async Task<ActionResult<List<InvestTools>>> Get_UserSToolsAsync(int id)
        {
            return Ok(await _userKabinetService.UserSToolsAsync(id));
        }

        [HttpGet("Calculate")]
        public async Task<ActionResult<double?>> Get_CalculatedAsync(int id, DateTime? dateStart, DateTime? dateFinish)
        {
            return Ok(await _userKabinetService.Calculate(id, dateStart, dateFinish));
        }
    }
}
