using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

public class TiffCompressor : ICompressor
{
    public byte[] Compress(byte[] imageData)
    {
        using var inputStream = new MemoryStream(imageData);
        using var outputStream = new MemoryStream();

        // Загрузка изображения из потока
        using var image = Image.FromStream(inputStream);

        // Сохранение изображения в формате TIFF
        image.Save(outputStream, ImageFormat.Tiff);

        return outputStream.ToArray(); // Возвращаем байтовый массив TIFF
    }

    public void SetQuality(int quality)
    {
        // TIFF обычно используется для изображений без сжатия или с минимальным сжатием.
        // Здесь можно оставить метод пустым или предусмотреть специальное сжатие для TIFF (например, LZW).
    }
}
