using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

public class BmpCompressor : ICompressor
{
    public byte[] Compress(byte[] imageData)
    {
        using var inputStream = new MemoryStream(imageData);
        using var outputStream = new MemoryStream();

        // Загрузка изображения из потока
        using var image = Image.FromStream(inputStream);

        // Сохранение изображения в формате BMP (Bitmap)
        image.Save(outputStream, ImageFormat.Bmp);
        
        return outputStream.ToArray(); // Возвращаем байтовый массив BMP
    }

    public void SetQuality(int quality)
    {
        // BMP не поддерживает изменение качества сжатия.
    }
}
