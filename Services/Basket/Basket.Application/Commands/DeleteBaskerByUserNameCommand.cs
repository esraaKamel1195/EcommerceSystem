using MediatR;

namespace Basket.Application.Commands
{
    public class DeleteBaskerByUserNameCommand: IRequest<Unit>
    {
        public string UserName { get; set; }

        public DeleteBaskerByUserNameCommand(string userName)
        {
            UserName = userName;
        }
    }
}
