using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StarMicronics.StarIO;

namespace TotalPack.Tesoreria.Printing.StarIO
{
    /// <summary>
    /// Defines the methods to communicate with a StarIO printer.
    /// </summary>
    public sealed class StarIOPrinter : PrinterBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StarIOPrinter"/> class.
        /// </summary>
        public StarIOPrinter()
        {
        }

        /// <summary>
        /// Prints the specified string value to the printer.
        /// </summary>
        /// <param name="value">The value to print.</param>
        public override void Print(string value)
        {
            bool isOnline = false;
            IPort port = null;

            try
            {
                port = Factory.I.GetPort("USBPRN:" + PrinterName, "", Timeout);
                isOnline = port.GetOnlineStatus();

                if (!isOnline)
                {
                    throw new PortException("The printer is offline.");
                }

                byte[] dataByteArray = Encoding.GetEncoding("Windows-1252").GetBytes(value);
                uint amountWritten = 0;
                uint amountWrittenKeep = 0;
                while (dataByteArray.Length > amountWritten)
                {
                    amountWrittenKeep = amountWritten;
                    amountWritten += port.WritePort(dataByteArray, amountWritten, (uint)dataByteArray.Length - amountWritten);
                    if (amountWrittenKeep == amountWritten)
                    {
                        throw new PortException("Can't send data.");
                    }
                }

                if (amountWritten != dataByteArray.Length)
                {
                    throw new PortException("All data was not sent.");
                }
            }
            finally
            {
                if (port != null)
                {
                    Factory.I.ReleasePort(port);
                }
            }
        }

        /// <summary>
        /// Returns the status of the printer.
        /// </summary>
        /// <returns>An object that contains the status of the printer.</returns>
        public override PrinterStatus GetStatus()
        {
            IPort port = null;
            var printerStatus = new PrinterStatus();

            try
            {
                port = Factory.I.GetPort(PrinterName, "", Timeout);
                var status = port.GetParsedStatus();

                if (status == null)
                {
                    throw new PortException("The printer status is null.");
                }

                printerStatus.ReceiptPaperNearEmpty = status.ReceiptPaperNearEmptyInner || status.ReceiptPaperNearEmptyOuter;
                printerStatus.CoverOpen = status.CoverOpen;
                printerStatus.ReceiptPaperEmpty = status.ReceiptPaperEmpty;
                printerStatus.OverTemp = status.OverTemp;
                printerStatus.CutterError = status.CutterError;
                printerStatus.IsOffline = status.Offline;
                
                return printerStatus;
            }
            finally
            {
                if (port != null)
                {
                    Factory.I.ReleasePort(port);
                }
            }
        }

        /// <summary>
        /// Starts pooling the printer status.
        /// </summary>
        public override void StartPooling()
        {
            throw new NotImplementedException();
        }

        protected override void OnPrinterStatusUpdate(EventArgs e)
        {
            base.OnPrinterStatusUpdate(e);
        }
    }
}
