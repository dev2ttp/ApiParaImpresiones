using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApiParaImpresion.Models
{
    public class PrintReq
    {
        public string document { get; set; }
    }
    public class PrintResponse
    {
        public bool status { get; set; }
        public int code { get; set; }
        public string message { get; set; }
    }
    public class PrintBonoReq 
    {
        public string bono { get; set; }
    }
}