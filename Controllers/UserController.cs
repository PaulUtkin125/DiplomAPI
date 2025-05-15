using DiplomAPI.Data;
using DiplomAPI.Model.Support;
using DiplomAPI.Models.Support;
using DiplomAPI.Models.UserModels;
using Finansu.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
        public async Task<ActionResult<User>> Post_UserDataAsync([FromBody]int id) 
        {
            var resalt = await _userKabinetService.UserMoneyLoadAsync(id);
            if (resalt == null) return BadRequest(resalt);
            else return Ok(resalt);
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

        [HttpPost("Calculate")]
        public async Task<ActionResult<double?>> Get_CalculatedAsync([FromBody] CalculateSupport calculateSupport)
        {
            double? resalt = await _userKabinetService.Calculate(calculateSupport.Id, calculateSupport.dateStart, calculateSupport.dateFinish);
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

        [HttpPost("BueMethod")]
        public async Task<ActionResult<int>> Byu([FromBody]BuySuppoprt buySuppoprt)
        {
            using (var context = new dbContact())
            {
                var user_Exist = await context.User.FindAsync(buySuppoprt.uId);
                if (buySuppoprt.Price < 0) user_Exist.Maney = Math.Round(user_Exist.Maney - buySuppoprt.Price * buySuppoprt.Quentity, 2);
                else user_Exist.Maney = Math.Round(user_Exist.Maney - buySuppoprt.Price * buySuppoprt.Quentity, 2);


                if (user_Exist.Maney < 0) { return BadRequest(-1); }

                context.Entry(user_Exist).State = EntityState.Modified;

                var newRecord = new DvizhenieSredstv()
                {
                    InvestToolsId = buySuppoprt.toolId,
                    Money = buySuppoprt.Price,
                    Quentity = buySuppoprt.Quentity,
                    UserId = buySuppoprt.uId
                };
                context.dvizhenieSredstvs.Add(newRecord);

                Portfolio portflio_Existing = context.Portfolio.FirstOrDefault(x => x.UserId == buySuppoprt.uId && x.InvestToolId == buySuppoprt.toolId);
                portflio_Existing.AllManey = Math.Round(portflio_Existing.AllManey + buySuppoprt.Price, 2);

                if (buySuppoprt.Price < 0) portflio_Existing.Quentity -= buySuppoprt.Quentity;
                else portflio_Existing.Quentity += buySuppoprt.Quentity;



                if (portflio_Existing.Quentity < 0)
                {
                    return BadRequest(-2);
                }

                if (portflio_Existing.Quentity == 0) context.Entry(portflio_Existing).State = EntityState.Deleted;
                else context.Entry(portflio_Existing).State = EntityState.Modified;



                context.SaveChanges();
                return Ok(0);
            }
        }
    }
}
