public interface ICompressor
{
    byte[] Compress(byte[] imageData);            // Базовый метод для сжатия
    void SetQuality(int quality);                 // Метод для настройки качества сжатия
}
