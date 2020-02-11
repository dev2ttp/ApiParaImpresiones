using KellermanSoftware.CompareNetObjects;
using NiiPrinterCLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace TotalPack.Printing.Nipon
{
    /// <summary>
    /// Defines the methods to communicate with a Nipon printer.
    /// </summary>
    public sealed class NiiPrinter : PrinterBase
    {
        private readonly NIIClassLib niiPrinter;
        private readonly CompareLogic compareLogic;
        private long jobId;
        private Timer timer;

        /// <summary>
        /// Gets the last status exception.
        /// </summary>
        public NiiStatusException LastException { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="NiiPrinter"/> class.
        /// </summary>
        public NiiPrinter()
        {
            niiPrinter = new NIIClassLib();

            compareLogic = new CompareLogic();

            timer = new Timer() { AutoReset = false };
            timer.Interval = 2000;
            timer.Elapsed += Timer_Elapsed;
        }

        /// <summary>
        /// Prints the specified string value to the printer.
        /// </summary>
        /// <param name="value">The value to print.</param>
        public override void Print(string value)
        {
            int resultCode;
            string aux = "";

            value += "\x1B\x64\x04";   // 4LF
            value += "\x1B" + "i";     // Corte Total

            foreach (char c in value)
            {
                aux += string.Format("{0:X2}", Convert.ToUInt32(c));
            }

            resultCode = niiPrinter.NiiStartDoc(PrinterName, out jobId);
            if (resultCode != 0)
            {
                throw new NiiException(resultCode);
            }

            resultCode = niiPrinter.NiiPrint(PrinterName, aux, aux.Length, out jobId);
            if (resultCode != 0)
            {
                throw new NiiException(resultCode);
            }

            resultCode = niiPrinter.NiiEndDoc(PrinterName);
            if (resultCode != 0)
            {
                throw new NiiException(resultCode);
            }
        }

        /// <summary>
        /// Returns the status of the printer.
        /// </summary>
        /// <returns>An object that contains the status of the printer.</returns>
        public override PrinterStatus GetStatus()
        {
            int resultCode;
            long statusCode;
            var printerStatus = new PrinterStatus();

            resultCode = niiPrinter.NiiGetStatus(PrinterName, out statusCode);
            if (resultCode != 0)
            {
                LastException = new NiiStatusException(resultCode);
                throw LastException;
            }

            statusCode = statusCode & 0x1F;
            printerStatus.ReceiptPaperNearEmpty = (statusCode & 0x01) > 0;
            printerStatus.CoverOpen = (statusCode & 0x02) > 0;
            printerStatus.ReceiptPaperEmpty = (statusCode & 0x04) > 0;
            printerStatus.OverTemp = (statusCode & 0x08) > 0;
            printerStatus.CutterError = (statusCode & 0x10) > 0;
            printerStatus.IsOffline = (statusCode & 0x80) > 0;

            if (LastStatus == null)
            {
                LastStatus = printerStatus;
            }

            return printerStatus;
        }
        
        /// <summary>
        /// Starts pooling the printer status.
        /// </summary>
        public override void StartPooling()
        {
            timer.Enabled = true;
        }

        protected override void OnPrinterStatusUpdate(EventArgs e)
        {
            base.OnPrinterStatusUpdate(e);
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                var status = GetStatus();
                var comparisonResult = compareLogic.Compare(status, LastStatus);

                LastException = null;
                LastStatus = status;

                if (!comparisonResult.AreEqual)
                {
                    OnPrinterStatusUpdate(EventArgs.Empty);
                }
            }
            catch (NiiStatusException ex)
            {
                var comparisonResult = compareLogic.Compare(ex, LastException);
                LastException = ex;
                if (!comparisonResult.AreEqual)
                {
                    OnPrinterStatusUpdate(EventArgs.Empty);
                }
            }
            finally
            {
                timer.Enabled = true;
            }
        }
    }
}
