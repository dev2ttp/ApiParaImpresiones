using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TotalPack.Tesoreria.Printing.Nipon;
using TotalPack.Tesoreria.Printing.StarIO;

namespace TotalPack.Tesoreria.Printing
{
    /// <summary>
    /// This class is used to request 
    /// 
    /// 
    /// instances.
    /// </summary>
    public static class PrinterManager
    {
        private static PrinterBase printer = null;

        /// <summary>
        /// Retrieves or creates a printer instance.
        /// </summary>
        /// <param name="printerName">The name of the printer.</param>
        /// <returns>The printer instance.</returns>
        public static PrinterBase GetPrinter(string printerName)
        {
            if (printerName == null)
            {
                throw new ArgumentNullException(nameof(printerName));
            }

            if (printer != null)
            {
                return printer;
            }

            if (printerName.Contains("NII") || printerName.Contains("NP"))
            {
                printer = new NiiPrinter();
                printer.PrinterName = printerName;
                printer.PrinterType = PrinterType.Nii;
            }
            else if (printerName.Contains("TUP900"))
            {
                printer = new StarIOPrinter();
                printer.PrinterName = printerName;
                printer.PrinterType = PrinterType.TUP900;
            }
            else
            {
                throw new Exception($"The printer type could not be guessed --> '{printerName}'");
            }

            return printer;
        }
    }
}
