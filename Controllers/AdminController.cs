using System.Text;
using Azure.Core;
using Diplom_Utkin.Model.DataBase;
using Diplom_Utkin.Model.Support;
using DiplomAPI.Data;
using DiplomAPI.Models.db;
using DiplomAPI.Models.Reports;
using DiplomAPI.Models.Support;
using Finansu.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DiplomAPI.Controllers
{
    [Controller]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly MailSupport _mailSupport;
        private static Imageporter _imageporter;
        private static IConfiguration _configuration;

        public AdminController(IConfiguration configuration) 
        {
            _configuration = configuration;
            _mailSupport = new MailSupport(_configuration);
            _imageporter = new Imageporter(_configuration);
        }

        [HttpGet("NewBrokersList")]
        public async Task<ActionResult<List<Brokers>>> NewBrokersList()
        {
            using (var context = new dbContact(_configuration))
            {
                var Broker = await context.Brokers.Include(x => x.Urisidikciiy).Where(x => x.TypeOfRequestId == 1 && x.isAdmitted == false && x.IsClosing == false).ToListAsync();
                foreach (var item in Broker)
                {
                    if (item.SourseFile != "")
                    {
                        item.SourseFile = _imageporter.porter(item.SourseFile);
                    }
                    
                }
                return Broker;
            }
        }

        [HttpGet("NotNewBrokersList")]
        public async Task<ActionResult<List<Brokers>>> NotNewBrokersList()
        {
            using (var context = new dbContact(_configuration))
            {
                var Broker = await context.Brokers.Include(x => x.Urisidikciiy).Include(x => x.TypeOfRequest).ToListAsync();
                foreach (var item in Broker)
                {
                    if (item.SourseFile != "")
                    {
                        item.SourseFile = _imageporter.porter(item.SourseFile);
                    }

                }
                return Broker;
            }
        }

        [HttpPost("targetBroker")]
        public async Task<ActionResult> targetBroker([FromBody]ToolRequest Request)
        {
            using (var context = new dbContact(_configuration))
            {
                var data = await context.Brokers.Include(x => x.Urisidikciiy).FirstAsync(x => x.Id == Request.id);
                if (data == null) return BadRequest();

                data.SourseFile = _imageporter.porter(data.SourseFile);
                return Ok(data);
            }
        }

        [HttpPost("ModefiteRequest")]
        public async Task<ActionResult> ModefiteRequest([FromBody]ModefiteRequestSupport modefite)
        {
            try
            {
                using (var context = new dbContact(_configuration))
                {
                    var broker_Exist = await context.Brokers.FindAsync(modefite.brokerId);
                    if (modefite.mode == 0) // одобрить
                    {
                        string password = PasswordGenerator();

                        broker_Exist.TypeOfRequestId = 2;
                        broker_Exist.Password = password;

                        _mailSupport.SendConfirmation(broker_Exist.Email, password);
                    }
                    if (modefite.mode == 1) // отклонён
                    {
                        broker_Exist.TypeOfRequestId = 3;
                    }
                    context.Entry(broker_Exist).State = EntityState.Modified;

                    ApplicationHistory applicationHistory = new ApplicationHistory()
                    {
                        mainId = broker_Exist.Id,
                        UrisidikciiyId = broker_Exist.UrisidikciiyId,
                        NameBroker = broker_Exist.NameBroker,
                        SourseFile = broker_Exist.SourseFile,
                        IsClosing = broker_Exist.IsClosing,
                        FullNameOfTheDirector = broker_Exist.FullNameOfTheDirector,
                        INN = broker_Exist.INN,
                        KPP = broker_Exist.KPP,
                        OKTMO = broker_Exist.OKTMO,
                        BusinessAddress = broker_Exist.BusinessAddress,
                        Phone = broker_Exist.Phone,
                        Email = broker_Exist.Email,
                        TypeOfRequestId = broker_Exist.TypeOfRequestId,
                        Password = broker_Exist.Password
                    };
                    context.ApplicationHistory.Add(applicationHistory);

                    context.SaveChanges();
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        static private string PasswordGenerator()
        {
            const int length = 10;

            const string lowerChars = "abcdefghijklmnopqrstuvwxyz";
            const string upperChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string digits = "0123456789";
            const string specialChars = "!@#$%^&*()-_=+[]{};:,.<>?";

            Random random = new Random();
            StringBuilder passwordBuilder = new StringBuilder();
            passwordBuilder.Append(lowerChars[random.Next(lowerChars.Length)]);
            passwordBuilder.Append(upperChars[random.Next(upperChars.Length)]);
            passwordBuilder.Append(digits[random.Next(digits.Length)]);
            passwordBuilder.Append(specialChars[random.Next(specialChars.Length)]);

            string allChars = lowerChars + upperChars + digits + specialChars;
            for (int i = 4; i < length; i++)
            {
                passwordBuilder.Append(allChars[random.Next(allChars.Length)]);
            }

            return new string(passwordBuilder.ToString().OrderBy(c => random.Next()).ToArray());
        }

        [HttpPost("AllUserlist")]
        public async Task<ActionResult<List<User>>> AllUser([FromBody]int userId)
        {
            using (var context = new dbContact(_configuration))
            {
                var Users = await context.User.Include(x => x.TypeOfUser).Where(x => x.Id != userId && x.TypeOfUserId != 4).ToListAsync();
                return Ok(Users);
            }
        }

        [HttpPost("targetUser")]
        public async Task<ActionResult> targetUser([FromBody] ToolRequest Request)
        {
            using (var context = new dbContact(_configuration))
            {
                var data = await context.User.Include(x => x.TypeOfUser).FirstAsync(x => x.Id == Request.id);
                if (data == null) return BadRequest();

                return Ok(data);
            }
        }

        [HttpGet("AllUserType")]
        public async Task<ActionResult<List<TypeOfUser>>> AllUserType()
        {
            try
            {
                using (var context = new dbContact(_configuration))
                {
                    var resalt = await context.typeOfUser.ToListAsync();
                    return Ok(resalt);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPatch("deleteUser")]
        public async Task<ActionResult> DeleteUser([FromBody]int id)
        {
            using (var context = new dbContact(_configuration))
            {
                try
                {
                    var targetUser = await context.User.FindAsync(id);
                    targetUser.TypeOfUserId = 4;

                    context.Entry(targetUser).State = EntityState.Modified;
                    context.SaveChanges();

                    return Ok();
                }
                catch (Exception ex)
                {
                    return BadRequest(ex);
                }
            }
        }

        [HttpPatch("updateUser")]
        public async Task<ActionResult> updateUser([FromBody]User request)
        {
            try
            {
                using (var context = new dbContact(_configuration))
                {
                    var user_Exist = await context.User.FindAsync(request.Id);

                    if (request.TypeOfUserId != 0 &&
                        request.TypeOfUserId != user_Exist.TypeOfUserId) user_Exist.TypeOfUserId = request.TypeOfUserId;

                    if (request.Loggin != null && request.Loggin != user_Exist.Loggin) user_Exist.Loggin = request.Loggin;
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


        [HttpPost("ReportObiemOperationofBroker")]
        public async Task<ActionResult<List<Repport1Model>>> ReportObiemOperationofBroker([FromBody]Report1Reauest request)
        {
            try
            {

                using (var context = new dbContact(_configuration))
                {
                    var brokerSupport = await context.Brokers.ToListAsync();
                    var allOparatoins = context.dvizhenieSredstvs.Include(x => x.InvestTools.Brokers).Where(x => x.Date >= request.startDate && x.Date <= request.endDate);
                    if (allOparatoins is null) return BadRequest(1);

                    var usedBrokerId = allOparatoins.Select(x => x.InvestTools.BrokersId).ToList();

                    double sumPokupki = 0;
                    double summProdazhi = 0;

                    List<Repport1Model> report1Reauests = new List<Repport1Model>();

                    foreach (var item in brokerSupport)
                    {
                        if (!usedBrokerId.Contains(item.Id)) continue;

                        summProdazhi = Math.Round(allOparatoins.Where(x => x.InvestTools.BrokersId == item.Id && x.Money < 0).Sum(x => x.Quentity * x.Money), 2);
                        sumPokupki = Math.Round(allOparatoins.Where(x => x.InvestTools.BrokersId == item.Id && x.Money > 0).Sum(x => x.Quentity * x.Money), 2);

                        Repport1Model report1Reauest = new Repport1Model()
                        {
                            BrokerName = item.NameBroker,
                            sumPokupokClientov = sumPokupki,
                            sumProdazhClientov = summProdazhi
                        };
                        report1Reauests.Add(report1Reauest);
                    }
                    return Ok(report1Reauests);

                }

            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
        [HttpPost("ReportStatysBroker")]
        public async Task<ActionResult<List<AltBrokers>>> ReportStatysBroker([FromBody] Report1Reauest request)
        {
            try
            {

                using (var context = new dbContact(_configuration))
                {
                    var brokers = await context.ApplicationHistory
                            .Where(x => x.dateEdit >= DateOnly.FromDateTime(request.startDate) && x.dateEdit <= DateOnly.FromDateTime(request.endDate))
                            .OrderByDescending(x => x.dateEdit).ToListAsync();

                    var typeIds = brokers.Select(b => b.TypeOfRequestId).Distinct().ToList();

                    // Загрузим названия типов заявок по их Id
                    var typesDict = await context.TypeRequest
                        .Where(t => typeIds.Contains(t.Id))
                        .ToDictionaryAsync(t => t.Id, t => t.Name);


                    TypeOfRequest typeOfRequest;
                    List<AltBrokers> brokersList = new List<AltBrokers>();
                    foreach (var item in brokers)
                    {
                        if (brokersList.FirstOrDefault(x => x.Id == item.mainId) == null)
                        {

                            AltBrokers brokers1 = new AltBrokers()
                            {
                                Id = item.mainId,
                                NameBroker = item.NameBroker,
                                SourseFile = item.SourseFile,
                                IsClosing = item.IsClosing,
                                FullNameOfTheDirector = item.FullNameOfTheDirector,
                                INN = item.INN,
                                KPP = item.KPP,
                                OKTMO = item.OKTMO,
                                Phone = item.Phone,
                                BusinessAddress = item.BusinessAddress,
                                Email = item.Email,
                                isAdmitted = item.isAdmitted,
                                dateSubmitted = item.dateSubmitted,
                                TypeOfRequest = typesDict.ContainsKey(item.TypeOfRequestId) ? typesDict[item.TypeOfRequestId] : "Неизвестно",
                                Password = item.Password
                            };
                            brokersList.Add(brokers1);
                        }
                    }

                    return Ok(brokersList);

                }

            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}
