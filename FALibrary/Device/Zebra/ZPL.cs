using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FALibrary.Device.Zebra
{
    public sealed class ZPL
    {
        public class PrinterStatus
        {
            public enum EPrintMode
            {
                Rewind = 0,
                PeelOff = 1,
                TearOff = 2,
                Cutter = 3,
                Applicator = 4,
                DelayedCut = 5,
                RFID = 9,
            }

            public string CommunicationSetting { get; set; }
            public bool PaperOut { get; set; }
            public bool Pause { get; set; }
            public int LabelLength { get; set; }
            public int NumberOfFormatsInReceiveBuffer { get; set; }
            public bool BufferFull { get; set; }
            public bool CommunicationDiagnosticMode { get; set; }
            public bool PartialFormat { get; set; }
            public bool CorruptRAM { get; set; }
            public bool UnderTemperature { get; set; }
            public bool OverTemperature { get; set; }
            public string FunctionSetting { get; set; }
            public bool HeadUp { get; set; }
            public bool RibbonOut { get; set; }
            public bool ThermalTransferMode { get; set; }
            public EPrintMode PrintMode { get; set; }
            public string PrintWidthMode { get; set; }
            public bool LabelWaiting { get; set; }
            public string LabelRemainingInBatch { get; set; }
            public bool FormatWhilePrinting { get; set; }
            public int NumberOfGraphicImagesStoredInMemory { get; set; }
        }

        private static readonly char STX = Convert.ToChar(2);
        private static readonly char ETX = Convert.ToChar(3);
        private static readonly char CR = '\r';
        private static readonly char LF = '\n';

        private static byte[] CreateSendData(string command)
        {
            return Encoding.ASCII.GetBytes(STX + command + ETX + CR + LF);
        }

        public static byte[] CommandGetStatus
        {
            get { return CreateSendData("~HS"); }
        }

        public static PrinterStatus ParsingPrinterStatus(byte[] data)
        {
            if (data == null) return null;

            char[] crlf = { CR, LF };
            string[] spliteData = Encoding.ASCII.GetString(data).Split(crlf, StringSplitOptions.RemoveEmptyEntries);
            if (spliteData.Length < 2) return null;

            PrinterStatus printerStatus = new PrinterStatus();

            try
            {
                spliteData[0] = spliteData[0].Replace(STX.ToString(), "");
                spliteData[0] = spliteData[0].Replace(ETX.ToString(), "");
                spliteData[1] = spliteData[1].Replace(STX.ToString(), "");
                spliteData[1] = spliteData[1].Replace(ETX.ToString(), "");

                string[] stringList1 = spliteData[0].Split(',');
                string[] stringList2 = spliteData[1].Split(',');

                printerStatus.CommunicationSetting = stringList1[0];
                if (stringList1[1] == "1") printerStatus.PaperOut = true;
                if (stringList1[2] == "1") printerStatus.Pause = true;
                printerStatus.LabelLength = int.Parse(stringList1[3]);
                printerStatus.NumberOfFormatsInReceiveBuffer = int.Parse(stringList1[4]);
                if (stringList1[5] == "1") printerStatus.BufferFull = true;
                if (stringList1[6] == "1") printerStatus.CommunicationDiagnosticMode = true;
                if (stringList1[7] == "1") printerStatus.PartialFormat = true;
                if (stringList1[8] == "1") printerStatus.CorruptRAM = true;
                if (stringList1[9] == "1") printerStatus.UnderTemperature = true;
                if (stringList1[10] == "1") printerStatus.OverTemperature = true;

                printerStatus.FunctionSetting = stringList2[0];
                if (stringList2[2] == "1") printerStatus.HeadUp = true;
                if (stringList2[3] == "1") printerStatus.RibbonOut = true;
                if (stringList2[4] == "1") printerStatus.ThermalTransferMode = true;
                int printMode = int.Parse(stringList2[5]);
                if (Enum.IsDefined(typeof(PrinterStatus.EPrintMode), printMode))
                    Enum.ToObject(typeof(PrinterStatus.EPrintMode), printMode);
                else
                    throw new Exception("print mode is undefine value(" + printMode.ToString() + ")");

                printerStatus.PrintWidthMode = stringList2[6];
                if (stringList2[7] == "1") printerStatus.LabelWaiting = true;
                printerStatus.LabelRemainingInBatch = stringList2[8];
                if (stringList2[9] == "1") printerStatus.FormatWhilePrinting = true;
                printerStatus.NumberOfGraphicImagesStoredInMemory = int.Parse(stringList2[10]);
            }
            catch (Exception e)
            {
                throw e;
            }

            return printerStatus;
        }
    }
}
