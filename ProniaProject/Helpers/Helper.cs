namespace ProniaProject.Helpers
{
    public class Helper
    {
        public static string GetFileName(string path, string folder, IFormFile image)
        {
            string fileName = image.FileName.Length > 64 ? image.FileName.Substring(image.FileName.Length - 64, 64) : image.FileName;

            fileName = Guid.NewGuid().ToString() + fileName;

            string fullPath = Path.Combine(path, folder, fileName);

            using (FileStream stream = new FileStream(fullPath, FileMode.Create))
            {
                image.CopyTo(stream);
            }

            return fileName;
        }
    }
}
