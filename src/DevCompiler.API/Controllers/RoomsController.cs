using DevCompiler.Application.DTOs;
using DevCompiler.Application.Entities;
using DevCompiler.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DevCompiler.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RoomsController : ControllerBase
{
    private readonly IRoomService _roomService;
    public RoomsController(IRoomService roomService)
    {
        _roomService = roomService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetActiveRooms(CancellationToken cancellationToken)
    {
       var rooms = await _roomService.GetActiveRoomsAsync(cancellationToken);
        return Ok(rooms);
    }

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetRoom(Guid id, CancellationToken cancellationToken)
    {
        var room = await _roomService.GetRoomByIdAsync(id, cancellationToken);
        
        if (room is null)
        {
            return NotFound(new {error = "Room not found"});
        }

        return Ok(room);
    }

    [HttpPost]
    public async Task<IActionResult> CreateRoom([FromBody] CreateRoomRequest request, CancellationToken cancellationToken)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Unknown";
        var nickname = User.FindFirst("nickname")?.Value ?? "Anonymous";

        var room = await _roomService.CreateRoomAsync(request, userId, nickname, cancellationToken);
    
        return CreatedAtAction(nameof(GetRoom), new {id = room.Id }, room);
    }

    [HttpPut("{id:guid}/code")]
    public async Task<IActionResult> UpdateCode(Guid id, [FromBody] UpdateCodeRequest request, CancellationToken cancellationToken)
    {
        try
        {
            await _roomService.UpdateRoomCodeAsync(id, request.Code, cancellationToken);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {

            return NotFound(new { error = ex.Message });
        }
    }


}