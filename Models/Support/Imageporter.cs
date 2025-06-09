namespace DiplomAPI.Models.Support
{
    public class Imageporter
    {
        private static IConfiguration _configuration;
        public Imageporter(IConfiguration configuration) 
        {
            _configuration = configuration;
        }
        public string porter(string path) 
        {
            if(path == "") return path;
            byte[] imageArray = System.IO.File.ReadAllBytes(path);
            return Convert.ToBase64String(imageArray);
        }

         public async Task<string> UploadFile(IFormFile file, int mode)
         {
            var desktopPath = "";
            if (mode == 1) desktopPath = _configuration["UploadFile:Broker"];
            else if (mode == 2) desktopPath = _configuration["UploadFile:Tool"];
            string targetPath;
            if (file == null || file.Length == 0)
            {
                targetPath = Path.Combine(desktopPath, _configuration["UploadFile:Support"] + "\\NoNPhoto.png");
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
