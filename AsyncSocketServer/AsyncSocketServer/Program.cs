using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using System.IO;
using System.Configuration;
using System.Net;


[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace AsyncSocketServer
{
    class Program
    {
        public static ILog Logger;
        public static AsyncSocketServer AsyncSocketServ;
        public static string FileDirectory;

        static void Main(string[] args)
        {
            DateTime currentTime = DateTime.Now;
            log4net.GlobalContext.Properties["LogDir"] = currentTime.ToString("yyyyMM");
            log4net.GlobalContext.Properties["LogFileName"] = "_SocketAsyncServer" + currentTime.ToString("yyyyMMdd");
            Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            FileDirectory = config.AppSettings.Settings["FileDirectory"].Value;
            if (FileDirectory == "")
                FileDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Files");
            if (!Directory.Exists(FileDirectory))
                Directory.CreateDirectory(FileDirectory);

            int port = 0;
            if (!(int.TryParse(config.AppSettings.Settings["Port"].Value, out port)))
                port = 9999;
            int parallelNum = 0;
            if (!(int.TryParse(config.AppSettings.Settings["ParallelNum"].Value, out parallelNum)))
                parallelNum = 8000;
            int socketTimeOutMS = 0;
            if (!(int.TryParse(config.AppSettings.Settings["SocketTimeOutMS"].Value, out socketTimeOutMS)))
                socketTimeOutMS = 5 * 60 * 1000;

            AsyncSocketServ = new AsyncSocketServer(parallelNum);
            AsyncSocketServ.SocketTimeOutMS = socketTimeOutMS;
            AsyncSocketServ.Init();
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse("0.0.0.0"), port);
            AsyncSocketServ.Start(endPoint);

            Console.WriteLine("Press any key to terminate the server process...");
            Console.ReadKey();
            
        }
    }
}
