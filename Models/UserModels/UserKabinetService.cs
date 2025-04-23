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

        public async Task<List<InvestTools>> AllToolsLoadAsync()
        {
            using (var context = new dbContact())
            {
                return await context.InvestTools.ToListAsync();
            }
        }

        public async Task<List<Portfolio>> UserSToolsAsync(int id)
        {
            using (var context = new dbContact())
            {
                return await context.Portfolio.Include(x => x.InvestTool).Where(x => x.UserId == id).ToListAsync();
            }
        }

        private int startNumberOfMonth = 0;
        private int numberOfMonth = 12;

        public async Task<double?> Calculate(int id, DateTime? dateStart, DateTime? dateFinish)
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
                    return null;
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
                    return null;
                }


                dateSt = dateStart.Value.AddDays(1);
                dateFin = dateFinish.Value;



                double vvodSum = conext.dvizhenieSredstvs.Where(x => x.Money > 0 && x.Date >= dateSt && x.Date <= dateFin && x.UserId == id).Select(x => x.Money).Sum();
                double vuvodSum = Math.Abs(conext.dvizhenieSredstvs.Where(x => x.Money < 0 && x.Date >= dateSt && x.Date <= dateFin && x.UserId == id).Select(x => x.Money).Sum());

                double startManey = moveStartInvestToolExist.Select(x => x.AllManey).Sum();
                double endManey = moveFinishInvestToolExist.Select(x => x.AllManey).Sum();

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

                DateTime newDate = new DateTime(2024, 12, 31);
                DateTime supportDate = new DateTime(2024, numberOfMonth, DateTime.DaysInMonth(2024, numberOfMonth));  /////////
                DateTime dateFinish = supportDate;

                bool svEnd = false;

                List<InvestToolHistory> preSegments = null;


                for (int t = 0; t < 12; t++)
                {
                    preSegments = context.InvestToolHistory
                               .Where(x => x.DataIzmrneniiy.Date == dateFinish.Date && x.UserId == userId)
                               .Include(x => x.InvestTools)
                               .Include(x => x.InvestTools.Brokers)
                               .ToList();

                    for (int i = 0; i < 5; i++)
                    {
                        if (preSegments.Count == 0)
                        {
                            dateFinish = dateFinish.AddDays(1);
                            preSegments = context.InvestToolHistory
                                   .Where(x => x.DataIzmrneniiy.Date == dateFinish.Date && x.UserId == userId)
                                   .Include(x => x.InvestTools)
                                   .Include(x => x.InvestTools.Brokers)
                                   .ToList();
                        }
                        else
                        {
                            if (svEnd == false)
                            {
                                svEnd = true;
                                numberOfMonth = dateFinish.Month;
                                startNumberOfMonth = numberOfMonth;
                                break;
                            }
                        }
                    }
                    if (!svEnd)
                    {
                        if (supportDate.Month == 1)
                        {
                        //    MessageBox.Show("Нет данных на текущий календарный год!", "Ошибка");
                              return null;
                        }

                        supportDate = new DateTime(2024, supportDate.Month, 1).AddDays(-1).AddMonths(-1);
                        dateFinish = supportDate;
                    }
                }


                foreach (var Broker in allBrokers)
                {
                    bool sv = false;
                    double sum = 0;
                    foreach (var myBrokers in preSegments)
                    {
                        if (myBrokers.InvestTools.BrokersId == Broker.Id)
                        {
                            if (sv == false)
                            {
                                record = new string[2];
                                record[0] = myBrokers.InvestTools.Brokers.NameBroker;
                                sv = true;
                            }

                            sum += myBrokers.AllManey;

                            record[1] = sum.ToString();
                        }

                    }
                    strings.Add(record);
                    sv = false;
                }
            }

            return strings;
        }
    }
}
