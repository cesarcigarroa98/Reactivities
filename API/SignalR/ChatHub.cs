using System;
using System.Threading.Tasks;
using Application.Comments;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR
{
    public class ChatHub : Hub
    {
        private readonly IMediator _mediator;
        public ChatHub(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task SendComment(Create.Command command)
        {
            var comment = await _mediator.Send(command);

            //Call method on client
            await Clients.Group(command.ActivityId.ToString())
                .SendAsync("ReceiveComment", comment.Value);
        }

        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            //Get activity id from URl
            var activityId = httpContext.Request.Query["activityId"];
            //Join client to a group which is named as the current activity id
            await Groups.AddToGroupAsync(Context.ConnectionId, activityId);
            //Get list of comments from current activity
            var result = await _mediator.Send(new List.Query{ActivityId = Guid.Parse(activityId)});
            //Call method on client
            await Clients.Caller
                .SendAsync("LoadComments", result.Value);
        }
    }
}