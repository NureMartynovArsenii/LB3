public class Blockchain
{
    public List<BlockchainBlock> Chain { get; private set; }
    private const int Difficulty = 4;
    public Blockchain()
    {
        Chain = new List<BlockchainBlock>();
        AddGenesisBlock();
    }
    


    private void AddGenesisBlock()
    {
        Chain.Add(new BlockchainBlock
        {
            Index = 0,
            Timestamp = DateTime.UtcNow,
            DataHash = "Genesis",
            PreviousHash = "0",
            Nonce = 0
        });
    }

    public void AddBlock(string dataHash)
    {
        var lastBlock = Chain.Last();
        var newBlock = new BlockchainBlock
        {
            Index = lastBlock.Index + 1,
            Timestamp = DateTime.UtcNow,
            DataHash = dataHash,
            PreviousHash = lastBlock.DataHash
        };

        // Вычисляем Proof of Work для нового блока
        newBlock.Nonce = ProofOfWork(newBlock);

        // Добавляем новый блок в цепочку
        Chain.Add(newBlock);
    }
    private string ComputeHash(BlockchainBlock block)
    {
        string rawData = $"{block.Index}{block.Timestamp}{block.DataHash}{block.PreviousHash}{block.Nonce}";
        using (var sha256 = System.Security.Cryptography.SHA256.Create())
        {
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(rawData);
            byte[] hash = sha256.ComputeHash(bytes);
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }
    }
    public bool Validate()
    {
        for (int i = 1; i < Chain.Count; i++)
        {
            var currentBlock = Chain[i];
            var previousBlock = Chain[i - 1];

            if (currentBlock.Index != previousBlock.Index + 1)
                return false;

            if (currentBlock.PreviousHash != ComputeHash(previousBlock))
                return false;

            if (!ComputeHash(currentBlock).StartsWith(new string('0', Difficulty)))
                return false;
        }

        return true;
    }

    private int ProofOfWork(BlockchainBlock block)
    {
        int nonce = 0;
        string hash;

        do
        {
            block.Nonce = nonce;
            hash = ComputeHash(block);
            nonce++;
        }
        while (!hash.StartsWith(new string('0', Difficulty)));

        return nonce - 1; // Возвращаем корректное значение nonce
    }
}
