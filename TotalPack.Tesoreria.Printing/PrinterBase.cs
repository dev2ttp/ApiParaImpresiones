using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TotalPack.Tesoreria.Printing
{
    public abstract class PrinterBase
    {
        /// <summary>
        /// Gets or sets the name of the printer.
        /// </summary>
        public string PrinterName { get; set; }
        /// <summary>
        /// Gets or sets the amount of time, in milliseconds, to wait for the communication requests to finish. The default value is 10000.
        /// </summary>
        public int Timeout { get; set; } = 10000;
        /// <summary>
        /// Gets or sets the last status of the printer.
        /// </summary>
        public PrinterStatus LastStatus { get; protected set; }
        /// <summary>
        /// Gets or sets the type of the printer.
        /// </summary>
        public PrinterType PrinterType { get; set; }

        /// <summary>
        /// Returns the status of the printer.
        /// </summary>
        /// <returns>An object that contains the status of the printer.</returns>
        public abstract PrinterStatus GetStatus();
        /// <summary>
        /// Prints the specified string value to the printer.
        /// </summary>
        /// <param name="value">The value to print.</param>
        public abstract void Print(string value);
        /// <summary>
        /// Starts pooling the printer status.
        /// </summary>
        public abstract void StartPooling();

        /// <summary>
        /// The event that is raised whenever the status of the printer changes.
        /// </summary>
        public event EventHandler PrinterStatusUpdate;

        protected virtual void OnPrinterStatusUpdate(EventArgs e)
        {
            PrinterStatusUpdate?.Invoke(this, e);
        }
    }
}
