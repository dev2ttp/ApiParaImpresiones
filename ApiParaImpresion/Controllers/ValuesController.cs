using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TotalPack.Tesoreria.Printing;

namespace ApiParaImpresion.Controllers
{
    public class ValuesController : ApiController
    {
        private readonly PrinterBase printer = PrinterManager.GetPrinter("Star TUP900 Presenter (TUP992)");

        // GET api/values
        public IEnumerable<string> Get()
        {
            printer.Print("hola");
            printer.Print(Environment.NewLine + Environment.NewLine + Environment.NewLine + Environment.NewLine + Environment.NewLine + Environment.NewLine + Environment.NewLine);
            printer.Print("\x1b\x64\x00");  // Full cut
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}
