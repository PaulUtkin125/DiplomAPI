using Finansu.Model;

namespace DiplomAPI.Models.Support
{
    public class Imageporter
    {
        public string porter(string path) 
        {
            if(path == "") return path;
            byte[] imageArray = System.IO.File.ReadAllBytes(path);
            return Convert.ToBase64String(imageArray);
        }
    }
}
