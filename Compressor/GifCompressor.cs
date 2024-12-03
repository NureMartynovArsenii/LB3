using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

public class GifCompressor : ICompressor
{
    public byte[] Compress(byte[] imageData)
    {
        using var inputStream = new MemoryStream(imageData);
        using var outputStream = new MemoryStream();

        // Загрузка изображения из потока
        using var image = Image.FromStream(inputStream);

        // Сохранение изображения в формате GIF
        image.Save(outputStream, ImageFormat.Gif);

        return outputStream.ToArray(); // Возвращаем байтовый массив GIF
    }

    public void SetQuality(int quality)
    {
        // GIF не поддерживает изменение качества сжатия напрямую.
    }
}
