using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CalcFactorialController : ControllerBase
    {
        [HttpGet]


        public IEnumerable<Models.CalcFactorial> Get(long number = 0)
        {
            long mes = number;
            var api = new ApiMetod();
            var response = api.CalculateAsync(mes);
            return Enumerable.Range(1, 1).Select(index => new Models.CalcFactorial
            {
                UserNomber = mes,
                Factorial = response.Result.ToString(),
            })
            .ToArray();
        }
    }
}
