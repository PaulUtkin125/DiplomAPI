using DiplomAPI.Data;
using DiplomAPI.Model.Support;
using DiplomAPI.Models.db;
using DiplomAPI.Models.Reports;
using DiplomAPI.Models.Support;
using DiplomAPI.Models.UserModels;
using Finansu.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DiplomAPI.Controllers
{
    [Controller]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly UserKabinetService _userKabinetService;
        private static IConfiguration _configuration;
        public UserController(IConfiguration configuration)
        {
            _configuration = configuration;
            _userKabinetService = new UserKabinetService(_configuration);
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
        public async Task<ActionResult<List<InvestToolDop>>> Post_UserSToolsAsync([FromBody]int id)
        {
            return Ok(await _userKabinetService.UserSToolsAsync(id));
        }

        [HttpPost("Calculate")]
        public async Task<ActionResult<double?>> Get_CalculatedAsync([FromBody] CalculateSupport calculateSupport)
        {
            if (calculateSupport.toolId != 0)
            {
                double? resalt = await _userKabinetService.Calculate(calculateSupport.Id, calculateSupport.dateStart, calculateSupport.dateFinish, (int)calculateSupport.toolId);
                if (resalt == null) return BadRequest(resalt);
                return Ok(resalt);
            }
            else
            {
                double? resalt = await _userKabinetService.Calculate(calculateSupport.Id, calculateSupport.dateStart, calculateSupport.dateFinish);
                if (resalt == null) return BadRequest(resalt);
                return Ok(resalt);
            }
        }

        [HttpPost("loadCart")]
        public async Task<ActionResult<List<String[]>?>> Post_LoadCart([FromBody]int id)
        {
            var resalt = await _userKabinetService.LoadDataFormDatabaseAsync(id);
            if (resalt == null) return BadRequest(resalt);
            return Ok(resalt);
        }

        [HttpGet("loadNews")]
        public async Task<ActionResult<List<News>>> loadNews()
        {
            using (var context = new dbContact(_configuration))
            {
                var data = context.News.OrderByDescending(x => x.date).ToList();
                return Ok(data);
            }
        }

        [HttpPost("LoadBalanceHistory")]
        public async Task<ActionResult<List<DvizhenieSredstv>>> LoadBalanceHistory([FromBody]ToolRequest request)
        {
            try
            {
                using (var context = new dbContact(_configuration))
                {
                    var muveOfMoney = await context.dvizhenieSredstvs.Include(x => x.InvestTools).Where(x => x.UserId == request.id).ToListAsync();
                    var balaceHistory = await context.BalanceHistory.Where(x => x.UserId == request.id).ToListAsync();

                    foreach (var record in balaceHistory)
                    {
                        DvizhenieSredstv dvizhenieSredstv = new()
                        {
                            InvestToolsId = 0,
                            UserId = record.UserId,
                            Money = record.Money,
                            Quentity = 0,
                            Date = record.Date,
                        };
                        muveOfMoney.Add(dvizhenieSredstv);
                        muveOfMoney = muveOfMoney.OrderByDescending(x => x.Date).ToList();
                    }
                    return Ok(muveOfMoney);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }

            
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
            using (var context = new dbContact(_configuration))
            {
                var user_Exist1 = await context.User.FindAsync(buySuppoprt.uId);

                double sum;


                var user_Exist = await context.User.FindAsync(buySuppoprt.uId);
                if (buySuppoprt.Price < 0) user_Exist.Maney = Math.Round(user_Exist.Maney - buySuppoprt.Price * buySuppoprt.Quentity, 2);
                else user_Exist.Maney = Math.Round(user_Exist.Maney - buySuppoprt.Price * buySuppoprt.Quentity, 2);


                if (user_Exist.Maney < 0) { return BadRequest(-1); }

                context.Entry(user_Exist).State = EntityState.Modified;

                double sumOperation = 0;

                var newRecord = new DvizhenieSredstv()
                {
                    InvestToolsId = buySuppoprt.toolId,
                    Money = buySuppoprt.Price,
                    Quentity = buySuppoprt.Quentity,
                    UserId = buySuppoprt.uId
                };
                context.dvizhenieSredstvs.Add(newRecord);
                context.SaveChanges();

                Portfolio portflio_Existing = context.Portfolio.FirstOrDefault(x => x.UserId == buySuppoprt.uId && x.InvestToolId == buySuppoprt.toolId);
                if (portflio_Existing == null)
                {
                    sum = Math.Round((buySuppoprt.Price * buySuppoprt.Quentity), 2);
                    Portfolio portfolio = new Portfolio()
                    {
                        InvestToolId = buySuppoprt.toolId,
                        UserId = buySuppoprt.uId,
                        AllManey = sum,
                        Quentity = buySuppoprt.Quentity
                    };
                    context.Portfolio.Add(portfolio);
                    context.SaveChanges();
                    portflio_Existing = context.Portfolio.FirstOrDefault(x => x.UserId == buySuppoprt.uId && x.InvestToolId == buySuppoprt.toolId);
                }
                else 
                {
                    sumOperation = Math.Round(buySuppoprt.Price * buySuppoprt.Quentity, 2);
                    portflio_Existing.AllManey = Math.Round(portflio_Existing.AllManey + sumOperation, 2);

                    if (buySuppoprt.Price < 0) portflio_Existing.Quentity -= buySuppoprt.Quentity;
                    else portflio_Existing.Quentity += buySuppoprt.Quentity;
                }
                



                if (portflio_Existing.Quentity < 0)
                {
                    return BadRequest(-2);
                }

                if (portflio_Existing.Quentity == 0) context.Entry(portflio_Existing).State = EntityState.Deleted;
                else context.Entry(portflio_Existing).State = EntityState.Modified;

                BalanceHistory balanceHistory = new BalanceHistory()
                {
                    UserId = buySuppoprt.uId,
                    Money = sumOperation
                };
                context.BalanceHistory.Add(balanceHistory);

                context.SaveChanges();
                return Ok(0);
            }
        }

        [HttpPost("Report1")]
        public async Task<ActionResult<List<DvizhenieSredstv>>> Report1([FromBody]Report1Reauest reauest)
        {
            try
            {
                using (var context = new dbContact(_configuration))
                {
                    var dataList = await context.dvizhenieSredstvs
                        .Include(x => x.InvestTools)
                        .Where(x => x.UserId == reauest.mode && x.Date >= reauest.startDate && x.Date <= reauest.endDate)
                        .ToListAsync();
                    return Ok(dataList);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
        [HttpPost("Report2")]
        public async Task<ActionResult<List<Portfolio>>> Report2([FromBody] Report1Reauest reauest)
        {
            try
            {
                using (var context = new dbContact(_configuration))
                {
                    var dataList = await context.Portfolio.Include(x => x.InvestTool).ToListAsync();
                        
                    return Ok(dataList);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}
