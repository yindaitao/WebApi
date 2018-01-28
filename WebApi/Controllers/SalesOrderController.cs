using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebApi.Models;

namespace WebApi.Controllers
{
    [Authorize]
    public class SalesOrderController : ApiController
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="DocEntry"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/SalesOrder/GetSalesOrder")]
        public SalesOrder GetSalesOrders([FromBody]int DocEntry)
        {
            return SalesOrderList.list.Where(key => key.DocEntry == DocEntry).FirstOrDefault();
        }
    }
}
