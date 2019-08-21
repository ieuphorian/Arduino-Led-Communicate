using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RGB_AVGCOLOR
{
    class Program
    {

        #region Global Variables
        static bool _continue;
        static SerialPort _serialPort;
        #endregion

        #region Main Function
        static void Main(string[] args)
        {
            InitializePort();
            while (true)
            {
                var image = ScreenCapture.CaptureDesktop();
                var avgColor = getDominantColor((Bitmap)image);
                SendDataToArduino(avgColor.R, avgColor.G, avgColor.B);
            }
        }
        #endregion

        #region Utils
        static void InitializePort()
        {
            _serialPort = new SerialPort();
            _serialPort.PortName = "COM3";//Set your board COM
            _serialPort.BaudRate = 9600;
            _serialPort.Open();
        }
        static void ClosePort()
        {
            _serialPort.Close();
        }
        static void SendDataToArduino(int r, int g, int b)
        {
            string a = _serialPort.ReadExisting();
            _serialPort.Write($"{r},{g},{b}");
            Thread.Sleep(200);
        }
        static void LogError(Exception ex)
        {
            string filePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Error.txt";

            using (StreamWriter writer = new StreamWriter(filePath, true))
            {
                writer.WriteLine("-----------------------------------------------------------------------------");
                writer.WriteLine("Date : " + DateTime.Now.ToString());
                writer.WriteLine();

                while (ex != null)
                {
                    writer.WriteLine(ex.GetType().FullName);
                    writer.WriteLine("Message : " + ex.Message);
                    writer.WriteLine("StackTrace : " + ex.StackTrace);

                    ex = ex.InnerException;
                }
            }
        }
        static Color getDominantColor(Bitmap bmp)
        {
            int r = 0;
            int g = 0;
            int b = 0;

            int total = 0;

            for (int x = 0; x < bmp.Width; x++)
            {
                for (int y = 0; y < bmp.Height; y++)
                {
                    Color clr = bmp.GetPixel(x, y);

                    r += clr.R;
                    g += clr.G;
                    b += clr.B;

                    total++;
                }
            }

            r /= total;
            g /= total;
            b /= total;

            return Color.FromArgb(r, g, b);
        } 
        #endregion
    }
}
