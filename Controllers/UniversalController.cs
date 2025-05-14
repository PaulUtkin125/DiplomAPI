using DiplomAPI.Models.Support;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace DiplomAPI.Controllers
{
    [Route("api/[controller]")]
    [Controller]
    public class UniversalController : ControllerBase
    {
        private static readonly MailSupport _mailSupport = new MailSupport();
        

        [HttpPost("SendCode")]
        public async Task<ActionResult<string>> SendCode([FromBody] MailRequest request)
        {
            try
            {
                var tr = _mailSupport.StartPoint(request.ToMail);
                return Ok(tr);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}