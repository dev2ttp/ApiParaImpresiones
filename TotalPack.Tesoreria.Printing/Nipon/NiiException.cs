using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TotalPack.Tesoreria.Printing.Nipon
{
    /// <summary>
    /// The exception that is thrown when a document error occurs.
    /// </summary>
    public class NiiException : Exception
    {
        /// <summary>
        /// Gets the error code that is associated with this exception.
        /// </summary>
        public int ErrorCode { get; }
        /// <summary>
        /// Gets the error message that is associated with this exception.
        /// </summary>
        public override string Message
        {
            get
            {
                return string.Format("The printer returned an error: ({0}) {1}", ErrorCode, GetErrorDescription());
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NiiException"/> class.
        /// </summary>
        public NiiException()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NiiException"/> class with a specified error code.
        /// </summary>
        /// <param name="errorCode">The error code that is associated with this exception.</param>
        public NiiException(int errorCode)
            : base()
        {
            ErrorCode = errorCode;
        }

        /// <summary>
        /// Returns a friendly message for a given error code.
        /// </summary>
        /// <returns>A string that contains the message.</returns>
        protected virtual string GetErrorDescription()
        {
            switch (ErrorCode)
            {
                case -1:
                    return "Data expansion error";
                case -2:
                    return "Printer open error";
                case -3:
                    return "Document starting error";
                case -4:
                    return "Page starting error";
                case -5:
                    return "File acquisition failure";
                case -6:
                    return "Invalid argument error";
                case -7:
                    return "Temporary open error";
                case -13:
                    return "Printer output error (spooler)";
                case -31:
                    return "Resource shortage";
                case 1:
                    return "Document already started";
                case 2:
                    return "Printer is opened";
                default:
                    return null;
            }
        }
    }
}
