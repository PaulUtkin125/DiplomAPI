using DiplomAPI.Data;
using Finansu.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DiplomAPI.Models.UserModels
{
    public class UserKabinetService
    {
        public UserKabinetService() 
        {
            
        }

        public async Task<double> _UpdateMoneu(int id, double sum, int vector)
        {
            using (var context = new dbContact())
            {
                var targetUser = await context.User.FindAsync(id);
                if (vector == 1) targetUser.Maney -= sum;
                else targetUser.Maney += sum;
                context.SaveChanges();
                return targetUser.Maney;
            }
        }
        public async Task<User?> UserMoneyLoadAsync(int id)
        {
            using (var context = new dbContact())
            {
                return await context.User.FindAsync(id);
            }
        }




        public async Task<List<InvestTools>> AllToolsLoadAsync()
        {
            using (var context = new dbContact())
            {
                var allTools = await context.InvestTools.Include(x => x.Brokers).ToListAsync();

                List<InvestTools> list = new List<InvestTools>();
                foreach (var investTool in allTools) 
                {
                    byte[] imageArray = System.IO.File.ReadAllBytes(investTool.ImageSource);
                    investTool.ImageSource = Convert.ToBase64String(imageArray);
                    list.Add(investTool);
                }

                return list;
            }
        }

        public async Task<List<Portfolio>> UserSToolsAsync(int id)
        {
            using (var context = new dbContact())
            {
                var UTools = await context.Portfolio.Include(x => x.InvestTool).Where(x => x.UserId == id).ToListAsync();
                List<Portfolio> list = new List<Portfolio>();
                foreach (var tool in UTools)
                {
                    byte[] imageArray = System.IO.File.ReadAllBytes(tool.InvestTool.ImageSource);
                    tool.InvestTool.ImageSource = Convert.ToBase64String(imageArray);
                    list.Add(tool);
                }
                return list;
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

            using (var conext = new dbContact())
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

        public async Task<List<string[]>?> LoadDataFormDatabaseAsync(int userId)
        {
            List<string[]> strings = new List<string[]>();
            string[] record = new string[2]; // 0 - брокер; 1 - деньги
            using (var context = new dbContact())
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
