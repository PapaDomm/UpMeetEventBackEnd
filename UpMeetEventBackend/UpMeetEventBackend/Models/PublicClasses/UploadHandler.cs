namespace UpMeetEventBackend.Models.PublicClasses
{
    public class UploadHandler
    {
        private UpMeetDbContext dbContext = new UpMeetDbContext();
        public Image Upload(IFormFile img)
        {

            //Setup in another Method/class
            List<string> validExtensions = new List<string>() { ".jpg", ".png", ".gif" };
            string extension = Path.GetExtension(img.FileName);

            if (!validExtensions.Contains(extension))
            {
                return null;
                //$"Extension is not valid, please try ({string.Join(',',validExtensions)}) ";
            }

            long size = img.Length;
            if (size > (5 * 1024 * 1024))
            {
                return null;
                //"Maximum file size is 5mb";
            }

            string fileName = Guid.NewGuid().ToString() + extension;
            string path = Path.Combine(Directory.GetCurrentDirectory(), "Images");

            using FileStream stream = new FileStream(Path.Combine(path, fileName), FileMode.Create);
            img.CopyTo(stream);

            Image newImage = new Image()
            {
                ImageId = 0,
                Path = Path.Combine("Images", fileName)
            };

            dbContext.Images.Add(newImage);
            dbContext.SaveChanges();

            return newImage;
        }
    }
}
