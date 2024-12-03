using System.Drawing;
using System.Drawing.Imaging;
//using Test.Compressor;

public class PngCompressor : ICompressor
{
    public byte[] Compress(byte[] imageData)
    {
        using var inputStream = new MemoryStream(imageData);
        using var outputStream = new MemoryStream();

        using var image = Image.FromStream(inputStream);
        image.Save(outputStream, ImageFormat.Png);

        return outputStream.ToArray();
    }
    public void SetQuality(int quality)
    {
        // Для PNG установка качества не поддерживается, метод оставлен пустым.
    }
}
