using MediatR;
using Microsoft.AspNetCore.Mvc;
using WeatherForecast.API.ViewModels;

namespace WeatherForecast.API.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class BaseController : ControllerBase
    {
        private ISender _mediator = null!;
        protected ResponseModel response = null!;
        protected ISender Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<ISender>();
        public BaseController()
        {
            response = new ResponseModel();
        }
    }
}
