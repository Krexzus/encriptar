using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class EncryptionController : ControllerBase
{
    private readonly IEncryptionService _service;

    public EncryptionController(IEncryptionService service)
    {
        _service = service;
    }

    /// <summary>
    /// Encripta un texto plano.
    /// </summary>
    [HttpPost("encrypt")]
    public ActionResult<EncryptResponse> Encrypt([FromBody] EncryptRequest request)
    {
        var result = _service.EncryptWithId(request.PlainText);
        return Ok(new EncryptResponse { CipherText = result.Encrypted, Id = result.Id });
    }

    /// <summary>
    /// Desencripta un texto cifrado.
    /// </summary>
    [HttpPost("decrypt")]
    public ActionResult<DecryptResponse> Decrypt([FromBody] DecryptRequest request)
    {
        try
        {
            var plain = _service.Decrypt(request.CipherText);
            return Ok(new DecryptResponse { PlainText = plain });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Obtiene un texto encriptado desde Firebase por id.
    /// </summary>
    [HttpGet("firebase/{id}")]
    public ActionResult<EncryptResponse> GetFromFirebase(string id)
    {
        var cipher = _service.GetEncryptedFromFirebase(id);
        if (cipher == null)
            return NotFound();
        return Ok(new EncryptResponse { CipherText = cipher, Id = id });
    }
} 