using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

[ApiController]
[Route("api/[controller]")]
public class BlockchainController : ControllerBase
{
    private readonly Blockchain _blockchain;

    public BlockchainController(Blockchain blockchain)
    {
        _blockchain = blockchain;
    }

    // GET: api/blockchain
    [HttpGet]
    public IActionResult GetBlockchain()
    {
        return Ok(_blockchain.Chain);
    }

    // POST: api/blockchain/add-block
    [HttpPost("add-block")]
    public IActionResult AddBlock([FromBody] AddBlockRequest request)
    {
        if (string.IsNullOrWhiteSpace(request?.DataHash))
        {
            return BadRequest("Data hash is required.");
        }

        _blockchain.AddBlock(request.DataHash);

        return Ok(new
        {
            Message = "Block added successfully.",
            Block = _blockchain.Chain.Last()
        });
    }

    // GET: api/blockchain/validate-chain
    [HttpGet("validate-chain")]
    public IActionResult ValidateChain()
    {
        bool isValid = _blockchain.Validate();

        if (isValid)
        {
            return Ok(new
            {
                Message = "Blockchain is valid.",
                IsValid = true
            });
        }
        else
        {
            return BadRequest(new
            {
                Message = "Blockchain is invalid.",
                IsValid = false
            });
        }
    }

    // GET: api/blockchain/last-block
    [HttpGet("last-block")]
    public IActionResult GetLastBlock()
    {
        var lastBlock = _blockchain.Chain.LastOrDefault();

        if (lastBlock == null)
        {
            return NotFound("Blockchain is empty.");
        }

        return Ok(lastBlock);
    }
}

