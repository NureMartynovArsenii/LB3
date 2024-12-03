public class BlockchainBlock
{
    public int Index { get; set; }              // Номер блока
    public DateTime Timestamp { get; set; }     // Время создания блока
    public string DataHash { get; set; }        // Хеш данных (изображения)
    public string PreviousHash { get; set; }    // Хеш предыдущего блока
    public int Nonce { get; set; }

}
