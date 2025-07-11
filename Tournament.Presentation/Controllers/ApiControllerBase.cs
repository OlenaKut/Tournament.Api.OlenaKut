using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tournament.Core.Responses;

namespace Tournament.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiControllerBase : ControllerBase
    {
        [NonAction]
        public ActionResult ProcessError(ApiBaseResponse baseResponse)
        {
            return baseResponse switch
            {
                // Microsoft.AspNetCore.Mvc.NewtonsoftJson
                ApiNotFoundResponse => NotFound(Results.Problem
                (
                    detail: ((ApiNotFoundResponse)baseResponse).Message,
                    statusCode: StatusCodes.Status404NotFound,
                    title: "Not found",
                    instance: HttpContext.Request.Path
                    )),
                _ => throw new NotImplementedException()
            };
        }
    }
}
