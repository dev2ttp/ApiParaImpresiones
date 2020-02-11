using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TotalPack.Printing.StarIO
{
    internal class StarIOCodePage
    {
        public string GetCodePageCommand(string codePage)
        {
            switch (codePage)
            {
                case "Normal":
                    return "\x1b\x1d\x74\x00";
                case "437":
                    return "\x1b\x1d\x74\x01";
                case "Katakana":
                    return "\x1b\x1d\x74\x02";
                case "858":
                    return "\x1b\x1d\x74\x04";
                case "852":
                    return "\x1b\x1d\x74\x05";
                case "860":
                    return "\x1b\x1d\x74\x06";
                case "861":
                    return "\x1b\x1d\x74\x07";
                case "863":
                    return "\x1b\x1d\x74\x08";
                case "865":
                    return "\x1b\x1d\x74\x09";
                case "866":
                    return "\x1b\x1d\x74\x0a";
                case "855":
                    return "\x1b\x1d\x74\x0b";
                case "857":
                    return "\x1b\x1d\x74\x0c";
                case "862":
                    return "\x1b\x1d\x74\x0d";
                case "864":
                    return "\x1b\x1d\x74\x0e";
                case "737":
                    return "\x1b\x1d\x74\x0f";
                case "851":
                    return "\x1b\x1d\x74\x10";
                case "869":
                    return "\x1b\x1d\x74\x11";
                case "928":
                    return "\x1b\x1d\x74\x12";
                case "772":
                    return "\x1b\x1d\x74\x13";
                case "774":
                    return "\x1b\x1d\x74\x14";
                case "874":
                    return "\x1b\x1d\x74\x15";
                case "1252":
                    return "\x1b\x1d\x74\x20";
                case "1250":
                    return "\x1b\x1d\x74\x21";
                case "1251":
                    return "\x1b\x1d\x74\x22";
                case "3840":
                    return "\x1b\x1d\x74\x40";
                case "3841":
                    return "\x1b\x1d\x74\x41";
                case "3843":
                    return "\x1b\x1d\x74\x42";
                case "3844":
                    return "\x1b\x1d\x74\x43";
                case "3845":
                    return "\x1b\x1d\x74\x44";
                case "3846":
                    return "\x1b\x1d\x74\x45";
                case "3847":
                    return "\x1b\x1d\x74\x46";
                case "3848":
                    return "\x1b\x1d\x74\x47";
                case "1001":
                    return "\x1b\x1d\x74\x48";
                case "2001":
                    return "\x1b\x1d\x74\x49";
                case "3001":
                    return "\x1b\x1d\x74\x4a";
                case "3002":
                    return "\x1b\x1d\x74\x4b";
                case "3011":
                    return "\x1b\x1d\x74\x4c";
                case "3012":
                    return "\x1b\x1d\x74\x4d";
                case "3021":
                    return "\x1b\x1d\x74\x4e";
                case "3041":
                    return "\x1b\x1d\x74\x4f";
                default:
                    return null;
            }
        }
    }
}
