using System;
using System.Diagnostics;
using System.Threading;
using System.IO;
using System.Text;
namespace ConsoleMonitor
{
    class Program
    {
       static void Main(string[] args)
        {
            if (args.Length < 3)
            {
                Console.WriteLine("Необходимо вести 3 аргумента:");
                Console.WriteLine("1: Имя процесса, например \"notepad\"");
                Console.WriteLine("2: Время жизни процесса в минутах");
                Console.WriteLine("3: Время опроса в минутах");
                Console.WriteLine("4: Можно задать имя файла лога (Опционально)");
                Console.WriteLine("Пример1: ConsoleMonitor notepad 5 1");
                Console.WriteLine("Пример2: ConsoleMonitor notepad 5 1 CustomLogFile.txt");
            }
            else if (args.Length == 3 || args.Length == 4)
            {

                String nameProcess = args[0].ToString();
                int lifeTimeProcess = 0;
                int touchTimeProcess = 0;
                var logFileName = "ConsoleMonitor.log";
                if (args.Length == 4)
                {
                    logFileName = args[3];
                }

                try
                {
                    lifeTimeProcess = Convert.ToInt32(args[1]);
                    touchTimeProcess = Convert.ToInt32(args[2]);
                }
                catch (FormatException e)
                {
                    Console.WriteLine("Ошибка в параметрах времени: " + e.Message.ToString());
                }

                Console.WriteLine("Отслеживаем процесс: " + nameProcess + " на время выполнения " + lifeTimeProcess + " мин. каждые " + touchTimeProcess + " мин.");

                if (lifeTimeProcess > 0 & touchTimeProcess > 0)
                {
                    var LongProgramTime = DateTime.Now;
                    WriteLog(DateTime.Now + " Начало отслеживания " + nameProcess, logFileName);
                    ConsoleKeyInfo cki;
                    while (true)
                    {
                        if(LongProgramTime < DateTime.Now)
                        {
                            var processes = GetProcess(nameProcess);
                            if (processes.Length > 0)
                            {
                                foreach (Process prc in processes)
                                {
                                    // Console.WriteLine("Процесс: " + prc.ProcessName + " ID " + prc.Id + " зпущен в " + prc.StartTime + " сейчас " + DateTime.Now);
                                    if (prc.StartTime < DateTime.Now.AddMinutes(-lifeTimeProcess)) {
                                        KillProcessById(prc.Id);
                                        WriteLog(DateTime.Now + " Процесс " + nameProcess + " был остановлен", logFileName);
                                    }
                                }
                            }
                            else
                            {
                                WriteLog(DateTime.Now + " Процесс " + nameProcess + " пока не запущен", logFileName);
                            }
                            LongProgramTime = DateTime.Now.AddMinutes(touchTimeProcess);
                        }
                        Thread.Sleep(1000);
                        if (Console.KeyAvailable == true)
                        {
                            cki = Console.ReadKey(true);
                            if (cki.Key == ConsoleKey.Spacebar || cki.Key == ConsoleKey.X || cki.Key == ConsoleKey.Enter)
                            {
                                WriteLog(DateTime.Now + " Работа прекращена " + nameProcess, logFileName);
                                break;
                            }
                        }
                        
                    }
                }
                else
                {
                    Console.WriteLine("Не все параметры введены корректно, или нет процесса с таким именем.");
                }
            }
        }

        public static void KillProcessById(int idToKill)
        {
            foreach (Process process in Process.GetProcesses())
                if (process.Id == idToKill)
                    process.Kill();
        }

        protected static Process[] GetProcess(String nameProcess)
        {
            Process[] prcesses = Process.GetProcessesByName(nameProcess);
            return prcesses;
        }

        public static bool WriteLog(String text, String fileName)
        {
            try
            {
                StreamWriter writer = new System.IO.StreamWriter(fileName);
                writer.WriteLine(text);
                writer.Close();
                return true;
            }
            catch (IOException e)
            {
                Console.WriteLine("Ошибка записи лога: " + e.Message.ToString());
                return false;
            }
            catch (ObjectDisposedException e)
            {
                Console.WriteLine("Ошибка записи лога: " + e.Message.ToString());
                return false;
            }
            catch (EncoderFallbackException e)
            {
                Console.WriteLine("Ошибка записи лога: " + e.Message.ToString());
                return false;
            }
        }
    }
}
