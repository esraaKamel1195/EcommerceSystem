using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
