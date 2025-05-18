using DiplomAPI.Data;
using Finansu.Model;
using Microsoft.EntityFrameworkCore;

namespace DiplomAPI.Models.LogoModels
{
    public class UserLogoService
    {
        public UserLogoService() 
        {

        }

        public async Task<bool> UserRigistrationAsinc(string login, string password)
        {
            using (var context = new dbContact())
            {
                try
                {
                    User newUser = new()
                    {
                        Loggin = login,
                        PaswordHash = password
                    };
                    context.User.Add(newUser);
                    context.SaveChanges();
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        public async Task<int?> UserAutorisationAsync(string login, string password) 
        {
            using (var context = new dbContact())
            {
                var user_Exist = await context.User.FirstOrDefaultAsync(x => x.Loggin == login && x.PaswordHash == password && x.TypeOfUserId == 2);
                if (user_Exist != null) return user_Exist.Id;
                else return null;
            }
        }

        public async Task<int?> AdminAutorisationAsync(string login, string password)
        {
            using (var context = new dbContact())
            {
                var user_Exist = await context.User.FirstOrDefaultAsync(x => x.Loggin == login && x.PaswordHash == password && x.TypeOfUserId == 1);
                if (user_Exist != null) return user_Exist.Id;
                else return null;
            }
        }
    }
}
