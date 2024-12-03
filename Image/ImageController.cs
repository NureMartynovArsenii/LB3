using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp.Formats.Jpeg;
using System.Security.Cryptography;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
using System.Drawing;

using SixLabors.ImageSharp.PixelFormats;

[Route("api/images")]
[ApiController]
public class ImageController : ControllerBase
{
    private static readonly List<ImageData> _imageStorage = new();
    private readonly Blockchain _blockchain;
    private readonly JpegCompressor _jpegCompressor;
    public ImageController(Blockchain blockchain)
    {
        _blockchain = blockchain;
        _jpegCompressor = new JpegCompressor();
    }

    // Модель для хранения изображений
    public class ImageData
    {
        public string Id { get; set; }          // Уникальный идентификатор
        public byte[] Data { get; set; }        // Данные изображения
        public string Hash { get; set; }        // Хеш изображения
    }

    [HttpPost("upload")]
    public async Task<IActionResult> UploadImage(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("File is required.");

        using var memoryStream = new MemoryStream();
        await file.CopyToAsync(memoryStream);
        byte[] imageData = memoryStream.ToArray();

        // Определяем MIME-тип или расширение файла
        string fileExtension = Path.GetExtension(file.FileName).ToLower();

        // Словарь доступных алгоритмов сжатия
        var compressors = new Dictionary<string, ICompressor>
    {
        { ".jpg", new JpegCompressor() },
        { ".jpeg", new JpegCompressor() },
        { ".png", new PngCompressor() },
        { ".bmp", new BmpCompressor() },
        { ".gif", new GifCompressor() },
        { ".tiff", new TiffCompressor() },
        { ".tif", new TiffCompressor() },
        { ".webp", new WebPCompressor() }  // Новый компрессор WebP
    };

        // Если формат не поддерживается
        if (!compressors.TryGetValue(fileExtension, out var compressor))
        {
            return BadRequest($"File format '{fileExtension}' is not supported.");
        }

        // Сжимаем изображение
        byte[] compressedImageData = compressor.Compress(imageData);

        // Генерируем хеш
        string hash = ComputeHash(compressedImageData);

        // Сохраняем изображение в хранилище
        var image = new ImageData
        {
            Id = Guid.NewGuid().ToString(),
            Data = compressedImageData,
            Hash = hash
        };
        _imageStorage.Add(image);

        // Добавляем блок в блокчейн с хешем изображения
        _blockchain.AddBlock(hash);

        return Ok(new { ImageId = image.Id, Hash = hash, BlockIndex = _blockchain.Chain.Last().Index });
    }

  

  

    [HttpGet("hash/{hash}")]
    public IActionResult GetImageByHash(string hash)
    {
        var image = _imageStorage.FirstOrDefault(img => img.Hash == hash);

        if (image == null)
            return NotFound("Image not found.");

        // Определение MIME-типа по содержимому изображения
        string mimeType = GetMimeType(image.Data);

        // Проверим правильность MIME-типа
        if (mimeType == "application/octet-stream")
        {
            return BadRequest("Unsupported image format.");
        }

        // Возвращаем изображение с правильным MIME-типом и расширением
        string fileExtension = GetFileExtension(mimeType);
        return File(image.Data, mimeType, $"{image.Id}{fileExtension}");
    }

    [HttpPost("compare-compression")]
    public async Task<IActionResult> CompareCompression(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("File is required.");

        using var memoryStream = new MemoryStream();
        await file.CopyToAsync(memoryStream);

        byte[] originalImageData = memoryStream.ToArray();
        long originalSize = originalImageData.Length;

        // Генерируем хеш для оригинального изображения
        string originalHash = ComputeHash(originalImageData);

        // Добавляем оригинальное изображение в блокчейн
        _blockchain.AddBlock(originalHash);

        // Список всех доступных компрессоров
        var compressors = new List<ICompressor>
    {
        new JpegCompressor(),
        new PngCompressor(),
        new WebPCompressor(),
        new BmpCompressor(),
        new GifCompressor(),
        new TiffCompressor()
    };

        var compressionResults = new List<object>();

        foreach (var compressor in compressors)
        {
            try
            {
                byte[] compressedImageData = compressor.Compress(originalImageData);
                long compressedSize = compressedImageData.Length;

                // Генерируем хеш для сжатого изображения
                string compressedHash = ComputeHash(compressedImageData);

                // Добавляем сжатое изображение в блокчейн
                _blockchain.AddBlock(compressedHash);

                compressionResults.Add(new
                {
                    Compressor = compressor.GetType().Name,
                    OriginalSize = originalSize,
                    CompressedSize = compressedSize,
                    CompressionRate = ((1 - (double)compressedSize / originalSize) * 100).ToString("F2") + "%",
                    OriginalHash = originalHash,
                    CompressedHash = compressedHash
                });
            }
            catch (Exception ex)
            {
                // Обработка ошибок при сжатии, если формат не поддерживается компрессором
                compressionResults.Add(new
                {
                    Compressor = compressor.GetType().Name,
                    OriginalSize = originalSize,
                    CompressedSize = "Error",
                    CompressionRate = "Error",
                    ErrorMessage = ex.Message
                });
            }
        }

        return Ok(compressionResults);
    }




    private string GetFileExtension(string mimeType)
    {
        return mimeType switch
        {
            "image/jpeg" => ".jpg", // Используем .jpg вместо .jpeg
            "image/png" => ".png",
            "image/gif" => ".gif",
            "image/webp" => ".webp",
            _ => ".bin" // Для неизвестных форматов
        };
    }

    private string GetMimeType(byte[] imageData)
    {
        if (imageData.Length > 4 &&
            imageData[0] == 0xFF && imageData[1] == 0xD8) // JPEG
        {
            return "image/jpeg";
        }
        else if (imageData.Length > 8 &&
                 imageData[0] == 0x89 && imageData[1] == 0x50 &&
                 imageData[2] == 0x4E && imageData[3] == 0x47) // PNG
        {
            return "image/png";
        }
        else if (imageData.Length > 4 &&
                 imageData[0] == 0x47 && imageData[1] == 0x49 &&
                 imageData[2] == 0x46) // GIF
        {
            return "image/gif";
        }
        // Проверка на Little-endian TIFF (современные платформы)
        else if (imageData[0] == 0x49 && imageData[1] == 0x49 &&
                 imageData[2] == 0x2A && imageData[3] == 0x00) // TIFF (little-endian)
        {
            return "image/tiff";
        }
        
        else if (imageData.Length > 12 &&
         imageData[0] == 0x52 && imageData[1] == 0x49 &&
         imageData[2] == 0x46 && imageData[3] == 0x46) // RIFF
        {
            if (imageData[8] == 0x57 && imageData[9] == 0x45 &&
                imageData[10] == 0x42 && imageData[11] == 0x50) // WEBP
            {
                return "image/webp";
            }
        }
        else if (imageData.Length > 2 &&
             imageData[0] == 0x42 && imageData[1] == 0x4D) // BMP magic number "BM"
        {
            return "image/bmp";
        }
        
        // По умолчанию возвращаем octet-stream
        return "application/octet-stream";
    }
    [HttpGet("id/{id}")]
    public IActionResult GetImageById(string id)
    {
        // Ищем изображение по ID в хранилище
        var image = _imageStorage.FirstOrDefault(img => img.Id == id);

        // Если изображение не найдено, возвращаем ошибку
        if (image == null)
            return NotFound("Image not found.");

        // Определение MIME-типа по содержимому изображения
        string mimeType = GetMimeType(image.Data);

        // Возвращаем изображение с правильным MIME-типом
        string fileExtension = GetFileExtension(mimeType);
        return File(image.Data, mimeType, $"{image.Id}{fileExtension}");
    }

    

    private string ComputeHash(byte[] data)
    {
        using var sha256 = SHA256.Create();
        byte[] hashBytes = sha256.ComputeHash(data);
        return Convert.ToBase64String(hashBytes);
    }
}
