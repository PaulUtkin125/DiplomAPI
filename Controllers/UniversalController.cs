using System.Text;
using DiplomAPI.Data;
using DiplomAPI.Models.Support;
using Finansu.Model;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DiplomAPI.Controllers
{
    [Route("api/[controller]")]
    [Controller]
    public class UniversalController : ControllerBase
    {
        private readonly MailSupport _mailSupport;
        private static readonly Imageporter _imageporter = new Imageporter();
        private static IConfiguration _configuration;

        public UniversalController(IConfiguration configuration)
        {
            _configuration = configuration;
            _mailSupport = new MailSupport(_configuration);
        }

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

        [HttpPost("ValidationINN")]
        public async Task<ActionResult<bool>> ValidationINN([FromBody] MailRequest request)
        {
            using (var context = new dbContact(_configuration))
            {
                var testData = await context.Brokers.FirstOrDefaultAsync(x => x.INN.ToString() == request.ToMail);
                if (testData != null) return Ok(1);
                return Ok(0);
            }
        }

        [HttpPost("targetToolInformation")]
        public async Task<ActionResult<InvestTools>> TargetToolInformation([FromBody]ToolRequest idTool)
        {
            try
            {
                using (var context = new dbContact(_configuration))
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

        [HttpPost("CreateBroker")]
        public async Task<ActionResult> CreateBroker([FromForm]CreateBrokerRequest request)
        {
            try
            {
                
                using (var context = new dbContact(_configuration))
                {
                    var testData = await context.Brokers.FirstOrDefaultAsync(x=> x.INN.ToString() == request.INN);
                    if (testData != null) return Ok(1);

                    

                    Brokers brokers = new Brokers()
                    {
                        NameBroker = request.NameBroker,
                        FullNameOfTheDirector = request.FullNameOfTheDirector,
                        UrisidikciiyId = request.UrisidikciiyId,
                        SourseFile = await UploadFile(request.file),
                        INN = long.Parse(request.INN),
                        KPP = long.Parse(request.KPP),
                        OKTMO = long.Parse(request.OKTMO),
                        BusinessAddress = request.BusinessAddress,
                        Phone = request.Phone,
                        Email = request.Email
                    };
                    context.Brokers.Add(brokers);
                    context.SaveChanges();
                }
                return Ok(0);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
            
        }
        

        static private async Task<string> UploadFile(IFormFile file)
        {
            var desktopPath = _configuration["UploadFile:Broker"];
            string targetPath;
            if (file == null || file.Length == 0)
            {
                targetPath = Path.Combine(desktopPath, _configuration["UploadFile:Support"]+ "\\NoNPhoto.png"); 
            }
            else 
            {
                targetPath = Path.Combine(desktopPath, file.FileName);
                using (var stream = new FileStream(targetPath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
            }

            

            
            return targetPath;
        }
    }
}