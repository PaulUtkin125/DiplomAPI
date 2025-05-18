using DiplomAPI.Data;
using DiplomAPI.Models.Support;
using Finansu.Model;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace DiplomAPI.Controllers
{
    [Route("api/[controller]")]
    [Controller]
    public class UniversalController : ControllerBase
    {
        private static readonly MailSupport _mailSupport = new MailSupport();
        private static readonly Imageporter _imageporter = new Imageporter();
        

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

        [HttpPost("targetToolInformation")]
        public async Task<ActionResult<InvestTools>> TargetToolInformation([FromBody]ToolRequest idTool)
        {
            try
            {
                using (var context = new dbContact())
                {
                    var tool = context.InvestTools.Find(idTool.id);
                    tool.ImageSource = _imageporter.porter(tool.ImageSource);
                    return Ok(tool);
                }
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
            
        }
    }
}