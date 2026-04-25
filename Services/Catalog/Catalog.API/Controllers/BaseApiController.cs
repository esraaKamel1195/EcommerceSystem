using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.API.Controllers
{
    [ApiController]
    //[Authorize(Policy = "CanRead")]
    public class BaseApiController : ControllerBase
    {

    }
}
