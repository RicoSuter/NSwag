using System;
using Microsoft.AspNetCore.Mvc;

namespace NSwag.Sample.NetCoreAngular.Controllers
{
    [Route("api/[controller]")]
    public class DateController : Controller
    {
        [HttpPost]
        [Route("[action]")]
        public DateTime AddDays(DateTime date, int days)
        {
            return date.AddDays(days);
        }

        [HttpPost]
        [Route("[action]")]
        public int GetDayOfYear(DateTime date)
        {
            return date.DayOfYear;
        }
    }
}