using SixLabors.ImageSharp.Formats.Jpeg;
using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp.Formats.Jpeg;
using System.Security.Cryptography;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;

public class JpegCompressor : ICompressor
{
    private int _quality = 75;  // Значение по умолчанию

    // Метод установки качества
    public void SetQuality(int quality)
    {
        if (quality < 1 || quality > 100)
            throw new ArgumentOutOfRangeException(nameof(quality), "Quality must be between 1 and 100.");

        _quality = quality;
    }

  
    public byte[] Compress(byte[] imageData)
    {
        using var image = Image.Load(imageData); // Загрузка изображения из массива байт

        // Попробуем уменьшить качество, если исходное изображение слишком маленькое
        int dynamicQuality = (image.Width * image.Height > 1000 * 1000) ? 50 : _quality;

        var encoder = new JpegEncoder
        {
            Quality = dynamicQuality
        };

        using var ms = new MemoryStream();
        image.Save(ms, encoder);  // Сохранение изображения с использованием выбранного кодека

        return ms.ToArray();
    }

}

