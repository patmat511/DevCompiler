using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.IdentityModel.Tokens.Experimental;
using System.Security.Claims;

namespace DevCompiler.API.Controllers;


[Authorize]
public class CollaborationHub : Hub
{
    private readonly ILogger<CollaborationHub> _logger;

	public CollaborationHub(ILogger<CollaborationHub> logger)
	{
		_logger = logger;	
    }

	public async Task JoinRoom(string roomId, CancellationToken cancellationToken = default)
	{
		if (string.IsNullOrWhiteSpace(roomId))
		{
			_logger.LogWarning("JoinRoom with empty roomId");
			throw new HubException("Room ID cannot be empty.");
        }

		await Groups.AddToGroupAsync(Context.ConnectionId, roomId, cancellationToken);

		var nickname = Context.User?.FindFirst("nickname")?.Value ?? "Anonymous";
		var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

		_logger.LogInformation("User ({userId}) - {nickname} joined room {roomId}", userId, nickname, roomId);

		await Clients.OthersInGroup(roomId).SendAsync("UserJoined",
			new
			{
				userId = Context.ConnectionId,
				nickname,
				timestamp = DateTime.UtcNow,

            }, cancellationToken);
    }

	public async Task LeaveRoom(string roomId, CancellationToken cancellationToken = default)
	{
        if (string.IsNullOrWhiteSpace(roomId))
        {
			_logger.LogWarning("LeaveRoom with empty roomdId");
			throw new HubException("Room ID cannot be empty.");
        }

		await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId, cancellationToken);

		var nickname = Context.User?.FindFirst("nickname")?.Value?? "Anonymous";	
		var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

		_logger.LogInformation("User ({userId}) - {nickname} left room {roomId}", userId, nickname, roomId);

		await Clients.OthersInGroup(roomId).SendAsync("UserLeft",
			new 
			{
				userId = Context.ConnectionId,
				nickname,
				timestamp = DateTime.UtcNow,

            }, cancellationToken);
    }

	public async Task SendMessage(string roomId, string message, CancellationToken cancellationToken = default)
	{
		if (string.IsNullOrWhiteSpace(roomId))
			throw new HubException("Room ID cannot be empty");
        
		if (string.IsNullOrWhiteSpace(message))
			throw new HubException("Message cannot be empty");

		if (message.Length > 1000)
			throw new HubException("Message is too long. Maximum length is 1000 characters");
    
		var nickname = Context.User?.FindFirst("nickname")?.Value ?? "Anonymous";
		var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

		_logger.LogInformation("User ({userId}) - {nickname} sent message in room {roomId}", userId, nickname, roomId);
    
		await Clients.Group(roomId).SendAsync("ReceiveMessage",
			new 
			{
				userId = Context.ConnectionId,
				nickname,
				message,
				timestamp = DateTime.UtcNow,
			}, cancellationToken);
    }

	public async Task UpdateCode(string roomId, string code, CancellationToken cancellationToken = default)
	{
		if (string.IsNullOrWhiteSpace(roomId))
			throw new HubException("Room ID cannot be empty");

		if (code is null)
			throw new HubException("Code cannot be null");

		if (code.Length > 20000)
			throw new HubException("Code is too long. Maximum length is 20000 characters");

		var nickname = Context.User?.FindFirst("nickname")?.Value ?? "Anonymous";
		var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

		_logger.LogDebug("User ({userId}) - {nickname} updated code in room {roomId}: Code length {length}",userId, nickname, roomId, code.Length );

		await Clients.OthersInGroup(roomId).SendAsync("CodeUpdated",
			new
			{
				userId = Context.ConnectionId,
				nickname,
				code,
				timestamp = DateTime.UtcNow,
			}, cancellationToken);
	}

	public async Task NotifyTyping(string roomId, bool isTyping, CancellationToken cancellationToken = default)
	{
		if (string.IsNullOrWhiteSpace(roomId))
			throw new HubException("Room ID cannot be empty");

		var nickname = Context.User?.FindFirst("nickname")?.Value ?? "Anonymous";

		await Clients.OthersInGroup(roomId).SendAsync("UserTyping",
			new 
			{
				userId = Context.ConnectionId,
				nickname,
				isTyping,
				timestamp = DateTime.UtcNow,
            }, cancellationToken);
    }

	public async Task UpdateCursorPosition(string roomId, int line, int column, CancellationToken cancellationToken = default)
	{
		if (string.IsNullOrWhiteSpace(roomId))
			throw new HubException("Room ID cannot be empty");

		var nickname = Context.User?.FindFirst("nickname")?.Value ?? "Anonymous";

		await Clients.OthersInGroup(roomId).SendAsync("CursorPositionUpdated",
			new 
			{
				userId = Context.ConnectionId,
				nickname,
				line,
				column,
				timestamp = DateTime.UtcNow,
			}, cancellationToken);

    }

	public override async Task OnConnectedAsync()
	{
		var nickname = Context.User?.FindFirst("nickname")?.Value ?? "Anonymous";
		var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

		_logger.LogInformation("User ({userId}) - {nickname} connected with Connection ID: {Context.ConnectionId}",
			userId, nickname, Context.ConnectionId);
    
		await base.OnConnectedAsync();
    }

	public override async Task OnDisconnectedAsync(Exception? exception)
	{
		var nickname = Context.User?.FindFirst("nickname")?.Value ?? "Anonymous";
		var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
		
		if (exception != null)
		{
			_logger.LogWarning("User ({userId}) - {nickname} disconnected with error - {exception}", userId, nickname, exception);
		}
		else
		{
			_logger.LogInformation("User ({userId}) - {nickname} disconnected normally.", userId, nickname);
        }

		await base.OnDisconnectedAsync(exception);
    }

}