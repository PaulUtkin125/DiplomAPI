using System.IO;
using DiplomAPI.Data;
using DiplomAPI.Models.db;
using DiplomAPI.Models.Support;
using Finansu.Model;
using Microsoft.EntityFrameworkCore;

namespace DiplomAPI.Models.UserModels
{
    public class UserKabinetService
    {
        private readonly IConfiguration _configuration;
        public UserKabinetService(IConfiguration configuration) 
        {
            _configuration = configuration;
        }

        public async Task<double> _UpdateMoneu(int id, double sum, int vector)
        {
            using (var context = new dbContact(_configuration))
            {
                if (vector == 1) sum = -sum;

                var targetUser = await context.User.FindAsync(id);
                targetUser.Maney += sum;


                // vector = 1 => -sum
                BalanceHistory balanceHistory = new BalanceHistory()
                {
                    UserId = id,
                    Money = sum,
                };
                context.BalanceHistory.Add(balanceHistory);

                context.SaveChanges();
                return targetUser.Maney;
            }
        }
        public async Task<User?> UserMoneyLoadAsync(int id)
        {
            using (var context = new dbContact(_configuration))
            {
                return await context.User.FindAsync(id);
            }
        }




        public async Task<List<InvestTools>> AllToolsLoadAsync()
        {
            using (var context = new dbContact(_configuration))
            {
                var allTools = await context.InvestTools.Include(x => x.Brokers).Where(x => x.isClosed != true && x.isFrozen != true).ToListAsync();

                List<InvestTools> list = new List<InvestTools>();
                var desktopPath = "";

                foreach (var investTool in allTools) 
                {
                    if (investTool.ImageSource == "NoNPhoto.png") desktopPath = _configuration["UploadFile:Support"];
                    else desktopPath = _configuration["UploadFile:Tool"];

                    string fullPath = desktopPath + investTool.ImageSource;

                    byte[] imageArray = System.IO.File.ReadAllBytes(desktopPath + investTool.ImageSource);
                    investTool.ImageSource = Convert.ToBase64String(imageArray);
                    list.Add(investTool);
                }

                return list;
            }
        }

        public async Task<List<InvestToolDop>> UserSToolsAsync(int id)
        {
            using (var context = new dbContact(_configuration))
            {
                DateTime curentDateStart = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                curentDateStart.AddMonths(-1).AddDays(-1);

                DateTime curentDateEnd = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));
                curentDateEnd = curentDateEnd.AddMonths(-1);

                List<InvestToolDop> investToolDops = new List<InvestToolDop>();

                var UserToolMove = context.dvizhenieSredstvs.Where(x => x.UserId == id &&  x.Money > 0).OrderByDescending(x => x.Date).ToList();
                var UTools = await context.Portfolio.Include(x => x.InvestTool).Where(x => x.UserId == id).ToListAsync();
                foreach (var tool in UTools)
                {
                    byte[] imageArray = System.IO.File.ReadAllBytes(_configuration["UploadFile:Tool"] + tool.InvestTool.ImageSource);
                    tool.InvestTool.ImageSource = Convert.ToBase64String(imageArray);

                    var pribl = await Calculate(id, curentDateStart, curentDateEnd, tool.InvestToolId);
                    var targetEndDate = UserToolMove.First(x => x.InvestToolsId == tool.InvestToolId);

                    InvestToolDop investToolDop = new InvestToolDop()
                    {
                        portfolio = tool,
                        Pribl = pribl,
                        DateTime = targetEndDate.Date

                    };
                    investToolDops.Add(investToolDop);

                }

                return investToolDops;
            }
        }

        private int startNumberOfMonth = 0;
        private int numberOfMonth = 12;

        public async Task<double> Calculate(int id, DateTime? dateStart, DateTime? dateFinish)
        {
            DateTime dateSt = dateStart.Value;
            DateTime dateFin = dateFinish.Value;
            bool svStart = false;
            bool svEnd = false;
            string[] mas = new string[2];  // 1 - название брокера; 2 - рамер прибыли
            List<string[]> statisticList = new List<string[]>();
            List<Portfolio> ports = new List<Portfolio>();

            using (var conext = new dbContact(_configuration))
            {

                // рассчет доходности по всему портфелю

                var moveStartInvestToolExist = conext.InvestToolHistory
                               .Where(x => x.DataIzmrneniiy.Date == dateSt.Date && x.UserId == id).ToList();

                var moveFinishInvestToolExist = conext.InvestToolHistory
                               .Where(x => x.DataIzmrneniiy.Date == dateFin.Date && x.UserId == id).ToList();


                for (int i = 0; i < 5; i++)
                {
                    if (moveStartInvestToolExist.Count == 0)
                    {
                        dateSt = dateSt.AddDays(1);
                        moveStartInvestToolExist = conext.InvestToolHistory
                               .Where(x => x.DataIzmrneniiy.Date == dateSt.Date && x.UserId == id).ToList();
                    }
                    else
                    {
                        svStart = true;
                        break;
                    }
                }
                if (!svStart)
                {
                    //MessageBox.Show("Нет данных на начало выбранного отчетного периода!", "Ошибка");           // !!!!
                    return 0.0;
                }


                for (int i = 0; i < 5; i++)
                {
                    if (moveFinishInvestToolExist.Count == 0)
                    {
                        dateFin = dateFin.AddDays(1);
                        moveFinishInvestToolExist = conext.InvestToolHistory
                               .Where(x => x.DataIzmrneniiy.Date == dateFin.Date && x.UserId == id).ToList();
                    }
                    else
                    {
                        svEnd = true;
                        break;
                    }
                }
                if (!svEnd)
                {
                    //MessageBox.Show("Нет данных на конец выбранного отчетного периода!", "Ошибка");          // !!!!
                    return 0.0;
                }


                dateSt = dateStart.Value.AddDays(1);
                dateFin = dateFinish.Value;



                double vvodSum = conext.dvizhenieSredstvs.Where(x => x.Money > 0 && x.Date >= dateSt && x.Date <= dateFin && x.UserId == id).Select(x => x.Money * x.Quentity).Sum();
                double vuvodSum = Math.Abs(conext.dvizhenieSredstvs.Where(x => x.Money < 0 && x.Date >= dateSt && x.Date <= dateFin && x.UserId == id).Select(x => x.Money * x.Quentity).Sum());

                double startManey = moveStartInvestToolExist.Select(x => x.Price).Sum();
                double endManey = moveFinishInvestToolExist.Select(x => x.Price).Sum();

                double resalt = Math.Round((endManey - startManey + vuvodSum - vvodSum), 2);


                return resalt;
            }
        }
        public async Task<double> Calculate(int id, DateTime? dateStart, DateTime? dateFinish, int toolId)
        {
            DateTime dateSt = dateStart.Value;
            DateTime dateFin = dateFinish.Value;
            bool svStart = false;
            bool svEnd = false;
            string[] mas = new string[2];  // 1 - название брокера; 2 - рамер прибыли
            List<string[]> statisticList = new List<string[]>();
            List<Portfolio> ports = new List<Portfolio>();

            using (var conext = new dbContact(_configuration))
            {

                var moveStartInvestToolExist = conext.InvestToolHistory
                               .Where(x => x.DataIzmrneniiy.Date == dateSt.Date && x.UserId == id && x.InvestToolsId == toolId).ToList();

                var moveFinishInvestToolExist = conext.InvestToolHistory
                               .Where(x => x.DataIzmrneniiy.Date == dateFin.Date && x.UserId == id && x.InvestToolsId == toolId).ToList();


                for (int i = 0; i < 5; i++)
                {
                    if (moveStartInvestToolExist.Count == 0)
                    {
                        dateSt = dateSt.AddDays(1);
                        moveStartInvestToolExist = conext.InvestToolHistory
                               .Where(x => x.DataIzmrneniiy.Date == dateSt.Date && x.UserId == id && x.InvestToolsId == toolId).ToList();
                    }
                    else
                    {
                        svStart = true;
                        break;
                    }
                }


                for (int i = 0; i < 5; i++)
                {
                    if (moveFinishInvestToolExist.Count == 0)
                    {
                        dateFin = dateFin.AddDays(1);
                        moveFinishInvestToolExist = conext.InvestToolHistory
                               .Where(x => x.DataIzmrneniiy.Date == dateFin.Date && x.UserId == id && x.InvestToolsId == toolId).ToList();
                    }
                    else
                    {
                        svEnd = true;
                        break;
                    }
                }
                if (!svEnd)
                {
                    return 0.0;
                }


                dateSt = dateStart.Value.AddDays(1);
                dateFin = dateFinish.Value;



                double vvodSum = conext.dvizhenieSredstvs.Where(x => x.Money > 0 && x.Date >= dateSt && x.Date <= dateFin && x.UserId == id && x.InvestToolsId == toolId).Select(x => x.Money * x.Quentity).Sum();
                double vuvodSum = Math.Abs(conext.dvizhenieSredstvs.Where(x => x.Money < 0 && x.Date >= dateSt && x.Date <= dateFin && x.UserId == id && x.InvestToolsId == toolId).Select(x => x.Money * x.Quentity).Sum());

                double startManey;
                if (!svStart)
                {
                    startManey = 0;
                }
                else
                {
                    startManey = moveStartInvestToolExist.Select(x => x.Price).Sum();
                }
               
                double endManey = moveFinishInvestToolExist.Select(x => x.Price).Sum();

                double resalt = Math.Round((endManey - startManey + vuvodSum - vvodSum), 2);


                return resalt;
            }
        }

        public async Task<List<string[]>?> LoadDataFormDatabaseAsync(int userId)
        {
            List<string[]> strings = new List<string[]>();
            string[] record = new string[2]; // 0 - брокер; 1 - деньги
            using (var context = new dbContact(_configuration))
            {
                var allBrokers = await context.Brokers.ToListAsync();

                bool svEnd = false;

                List<Portfolio> preSegments = null;

                preSegments = context.Portfolio.Include(x => x.InvestTool).Where(x => x.UserId == userId).ToList();
               


                foreach (var Broker in allBrokers)
                {
                    bool sv = false;
                    double sum = 0;
                    foreach (var myBrokers in preSegments)
                    {
                        if (myBrokers.InvestTool.BrokersId == Broker.Id)
                        {
                            if (sv == false)
                            {
                                record = new string[2];
                                record[0] = myBrokers.InvestTool.Brokers.NameBroker;
                                sv = true;

                                sum += myBrokers.AllManey;

                                record[1] = sum.ToString();
                                strings.Add(record);
                            }
                        }

                    }
                    sv = false;
                }
            }

            return strings;
        }
    }
}
