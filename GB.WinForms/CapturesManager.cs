namespace GB.WinForms;

internal class CapturesManager
{
    /// <summary>
    /// Saves a bitmap to a file with auto-generated filename in the "captures" directory
    /// </summary>
    /// <param name="bitmap">The bitmap to save</param>
    /// <returns>The full path to the saved image file</returns>
    public static string SaveScreenCapture(Bitmap bitmap)
    {
        try
        {
            // Generate a random filename with "capture-" prefix and 4 random digits
            Random random = new Random();
            string randomNumbers = random.Next(1000, 10000).ToString(); // Generates a number between 1000-9999
            string fileName = $"capture-{randomNumbers}.png";

            // Ensure the captures directory exists
            string directoryPath = Path.Combine(Environment.CurrentDirectory, "captures");
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            // Save the bitmap to the file
            string filePath = Path.Combine(directoryPath, fileName);
            bitmap.Save(filePath, System.Drawing.Imaging.ImageFormat.Png);

            return filePath;
        }
        catch (Exception ex)
        {
            return string.Empty;
        }
    }

}
