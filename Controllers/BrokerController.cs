using DiplomAPI.Data;
using DiplomAPI.Models.Support;
using Finansu.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DiplomAPI.Controllers
{
    [Route("api/[controller]")]
    [Controller]
    public class BrokerController : ControllerBase
    {
        private static Imageporter _imageporter;
        private static IConfiguration _configuration;

        public BrokerController(IConfiguration configuration)
        {
            _configuration = configuration;
            _imageporter = new Imageporter(_configuration);
        }

        [HttpPost("BrokerData")]
        public async Task<ActionResult<Brokers>> BrokerDataAsync([FromBody] int id)
        {
            try
            {
                using (var context = new dbContact(_configuration))
                {
                    var result = await context.Brokers.FindAsync(id);
                    result.SourseFile = _imageporter.porter(result.SourseFile);
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
            
        }

        [HttpPost("BrokersTools")]
        public async Task<ActionResult<List<InvestTools>>> BrokersToolsAsync([FromBody] int id)
        {
            try
            {
                using (var context = new dbContact(_configuration))
                {
                    var result = context.InvestTools.Where(x => x.BrokersId == id).ToList();

                    foreach (var item in result)
                    {
                        item.ImageSource = _imageporter.porter(item.ImageSource);
                    }

                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
            

        }

        [HttpPatch("UpdateData")]
        public async Task<ActionResult<int>> UpdateDataAsync([FromBody]Brokers request)
        {
            try
            {
                using (var context = new dbContact(_configuration))
                {
                    var user_Exist = await context.Brokers.FindAsync(request.Id);
                   
                    if(request.NameBroker != null 
                        && request.NameBroker != user_Exist.NameBroker) user_Exist.NameBroker = request.NameBroker;

                    if (request.Email != null && request.Email != user_Exist.Email) user_Exist.Email = request.Email;
                    if (request.Phone != null && request.Phone != user_Exist.Phone) user_Exist.Phone = request.Phone;
                    context.Entry(user_Exist).State = EntityState.Modified;
                    context.SaveChanges();

                    return Ok(1);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPut("CreateTool")]
        public async Task<ActionResult> CreateTool([FromForm]CreateInvestToolSupport request)
        {
            try
            {
                using (var context = new dbContact(_configuration))
                {
                    InvestTools investTools = new()
                    {
                        BrokersId = int.Parse(request.BrokersId),
                        NameInvestTool = request.NameInvestTool,
                        Price = double.Parse(request.Price),
                        ImageSource = await _imageporter.UploadFile(request.file, 2),
                        TypeTool = request.TypeTool
                    };
                    context.InvestTools.Add(investTools);
                    context.SaveChanges();

                    return Ok();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPatch("UpdateTool")]
        public async Task<ActionResult> UpdateToolAsync([FromForm] UpdateToolRequest request)
        {
            try
            {
                using (var context = new dbContact(_configuration))
                {
                    
                    var tool_Exist = await context.InvestTools.FindAsync(request.id);

                    if (!string.IsNullOrWhiteSpace(request.name)
                        && request.name != tool_Exist.NameInvestTool) tool_Exist.NameInvestTool = request.name;

                    if (request.file != null) tool_Exist.ImageSource = await _imageporter.UploadFile(request.file, 2);

                    context.Entry(tool_Exist).State = EntityState.Modified;
                    context.SaveChanges();

                    return Ok();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPatch("DeleteTool")]
        public async Task<ActionResult> DeleteToolAsync([FromBody] ToolRequest request)
        {
            try
            {
                using (var context = new dbContact(_configuration))
                {

                    var tool_Exist = await context.InvestTools.FindAsync(request.id);

                    InvestTools ExitedTool = context.InvestTools.FirstOrDefault(x => x.Id == request.id);

                    ExitedTool.isClosed = true;

                    context.Entry(ExitedTool).State = EntityState.Modified;


                    var PortfolioRecordsToRemove = context.Portfolio
                        .Where(x => x.InvestToolId == ExitedTool.Id)
                        .ToList();

                    foreach (var targetPortfolioRecord in PortfolioRecordsToRemove)
                    {
                        if (targetPortfolioRecord == null) continue;

                        if (targetPortfolioRecord.InvestToolId == ExitedTool.Id)
                        {
                            DvizhenieSredstv newMoveMoney = new()
                            {
                                InvestToolsId = ExitedTool.Id,
                                UserId = targetPortfolioRecord.UserId,
                                Money = -targetPortfolioRecord.AllManey
                            };
                            context.dvizhenieSredstvs.Add(newMoveMoney);


                            var targetUser = context.User.FirstOrDefault(x => x.Id == targetPortfolioRecord.UserId);
                            targetUser.Maney += targetPortfolioRecord.AllManey;
                            context.Entry(targetUser).State = EntityState.Modified;


                            context.Entry(targetPortfolioRecord).State = EntityState.Deleted;
                            
                        }
                    }
                    context.SaveChanges();
                    return Ok();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
        [HttpPost("DeleteToolfast")]
        public async Task<ActionResult> DeleteToolfastAsync([FromBody] int id)
        {
            try
            {
                using (var context = new dbContact(_configuration))
                {

                    var tool_Exist = await context.InvestTools.FindAsync(id);

                    InvestTools ExitedTool = context.InvestTools.FirstOrDefault(x => x.Id == id);

                    ExitedTool.isClosed = true;

                    context.Entry(ExitedTool).State = EntityState.Modified;


                    var PortfolioRecordsToRemove = context.Portfolio
                        .Where(x => x.InvestToolId == ExitedTool.Id)
                        .ToList();

                    foreach (var targetPortfolioRecord in PortfolioRecordsToRemove)
                    {
                        if (targetPortfolioRecord == null) continue;

                        if (targetPortfolioRecord.InvestToolId == ExitedTool.Id)
                        {
                            DvizhenieSredstv newMoveMoney = new()
                            {
                                InvestToolsId = ExitedTool.Id,
                                UserId = targetPortfolioRecord.UserId,
                                Money = -targetPortfolioRecord.AllManey
                            };
                            context.dvizhenieSredstvs.Add(newMoveMoney);


                            var targetUser = context.User.FirstOrDefault(x => x.Id == targetPortfolioRecord.UserId);
                            targetUser.Maney += targetPortfolioRecord.AllManey;
                            context.Entry(targetUser).State = EntityState.Modified;


                            context.Entry(targetPortfolioRecord).State = EntityState.Deleted;

                        }
                    }
                    context.SaveChanges();
                    return Ok();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPost("restoreTool")]
        public async Task restoreTool([FromBody]int id)
        {
            using (var context = new dbContact(_configuration))
            {
                var tool_Exist = await context.InvestTools.FindAsync(id);

                tool_Exist.isClosed = false;

                context.Entry(tool_Exist).State = EntityState.Modified;
                context.SaveChanges();
            }
        }
    }
}
