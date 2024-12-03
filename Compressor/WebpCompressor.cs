using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;
using System.IO;

public class WebPCompressor : ICompressor
{
    private int _quality = 75;  // Значение качества по умолчанию

    public byte[] Compress(byte[] imageData)
    {
        using var inputStream = new MemoryStream(imageData);
        using var outputStream = new MemoryStream();

        // Загружаем изображение из входного потока
        using var image = Image.Load(inputStream);

        // Кодировщик WebP с заданным качеством
        var encoder = new WebpEncoder
        {
            Quality = _quality
        };

        // Сохранение изображения в формате WebP
        image.Save(outputStream, encoder);

        return outputStream.ToArray();  // Возвращаем сжатое изображение
    }

    public void SetQuality(int quality)
    {
        if (quality < 1 || quality > 100)
            throw new ArgumentOutOfRangeException(nameof(quality), "Quality must be between 1 and 100.");

        _quality = quality;
    }
}
