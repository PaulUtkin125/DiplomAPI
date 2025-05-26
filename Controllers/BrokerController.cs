using Azure.Core;
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
        private static readonly Imageporter _imageporter = new Imageporter();
        private static IConfiguration _configuration;

        public BrokerController(IConfiguration configuration)
        {
            _configuration = configuration;
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
        public async Task<ActionResult> UpdateDataAsync([FromBody]Brokers request)
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

                    return Ok();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

    }
}
