using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace EComm.API.Controllers
{
    [ApiController]
    public class ErrorController : ControllerBase
    {
        [Route("servererror")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public Problem ServerError()
        {
            var eh = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            var problem = new Problem();

            if (eh?.Error is ApplicationException) {
                problem.Description = eh.Error.Message;
            }
            else {
                problem.Description = "An unexpected error has occurred";
            }
            return problem;
        }
    }

    public class Problem
    {
        public string Description { get; set; } = String.Empty;
    }
}
