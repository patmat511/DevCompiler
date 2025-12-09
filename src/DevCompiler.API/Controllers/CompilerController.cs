using DevCompiler.Application.DTOs;
using DevCompiler.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevCompiler.API.Controllers;


[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CompilerController : ControllerBase
{
    private readonly ICompilerService _compilerService;
    private readonly ILogger<CompilerController> _logger;

    public CompilerController(ICompilerService compilerService, ILogger<CompilerController> logger)
    {
        _compilerService = compilerService;
        _logger = logger;
    }

    [HttpPost("compile")]
    public async Task<IActionResult> Compile([FromBody] CompileRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Code))
            return BadRequest(new { error = "Code cannot be empty" });

        try
        {
            var result = await _compilerService.CompileAndExecuteAsync(request.Code);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,"An error occurred during compilation or execution.");
            return StatusCode(500, new { error = "Compilation failed", message = ex.Message });

        }
    }


}


