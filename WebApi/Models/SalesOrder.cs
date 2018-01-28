using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi.Models
{
    public class SalesOrder
    {
        public int DocEntry { get; set; }
        public DateTime DocDate { get; set; }
        public string CardCode { get; set; }
        public string CardName { get; set; }
    }

    public class SalesOrderList
    {
        public static List<SalesOrder> list
        {
            get
            {
                var sos = new List<SalesOrder>();
                sos.Add(new SalesOrder
                {
                    DocEntry = 1,
                    DocDate = Convert.ToDateTime("2018-01-03"),
                    CardCode = "C0001",
                    CardName = "北京第一家有限公司"
                });

                sos.Add(new SalesOrder
                {
                    DocEntry = 2,
                    DocDate = Convert.ToDateTime("2018-01-04"),
                    CardCode = "C0002",
                    CardName = "北京第二家有限公司"
                });

                sos.Add(new SalesOrder
                {
                    DocEntry = 3,
                    DocDate = Convert.ToDateTime("2018-01-05"),
                    CardCode = "C0003",
                    CardName = "北京第三家有限公司"
                });

                return sos;
            }
        }

    }
}