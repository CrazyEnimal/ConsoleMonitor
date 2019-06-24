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
            // Если пришло достаточное количество аргуметов
            if (args.Length == 3 || args.Length == 4)
            {
                // Пытаемся считать все параметры
                String nameProcess = args[0].ToString();
                int lifeTimeProcess = 0;
                int touchTimeProcess = 0;
                var logFileName = "ConsoleMonitor.log";
                if (args.Length == 4)
                {
                    logFileName = args[3];
                }
                // Попробуем сконвертировать параметры времени в Int если не вышло, то выдадим ошибку.
                try
                {
                    lifeTimeProcess = Convert.ToInt32(args[1]);
                    touchTimeProcess = Convert.ToInt32(args[2]);
                }
                catch (FormatException e)
                {
                    Console.WriteLine("Ошибка в параметрах времени: " + e.Message.ToString());
                }
                // Если ввели все параметры верно, то идём отслеживать процессы.
                if (lifeTimeProcess > 0 & touchTimeProcess > 0)
                {
                    Console.WriteLine("Отслеживаем процесс: " + nameProcess + " на время выполнения " + lifeTimeProcess + " мин. каждые " + touchTimeProcess + " мин.");
                    Console.WriteLine("Для остановки мониторинга нажмите Spacebar, Enter или x");
                    var LongProgramTime = DateTime.Now;
                    WriteLog(DateTime.Now + " Начало отслеживания " + nameProcess, logFileName);
                    ConsoleKeyInfo cki;
                    // Заводи бесконечный цикл, в которм будем отследивать нажатие клавиши для выхода и процессы.
                    while (true)
                    {
                        // Проверяем что прошло необходимое время для опроса заданное в третьем аргументе
                        if(LongProgramTime < DateTime.Now)
                        {
                            // Получаем все процессы с заданным именем.
                            var processes = GetProcess(nameProcess);
                            if (processes.Length > 0)
                            {
                                foreach (Process prc in processes)
                                {
                                    // Если превышено время отслеживаемого процесса мы его убиваем.
                                    if (prc.StartTime < DateTime.Now.AddMinutes(-lifeTimeProcess)) {
                                        var processId = prc.Id;
                                        KillProcessById(processId);
                                        WriteLog(DateTime.Now + " Процесс " + nameProcess + " ID (" + processId + ") был остановлен", logFileName);
                                    }
                                }
                            }
                            else
                            {
                                // Можно писать в лог если процесс пока не запущен.
                                // WriteLog(DateTime.Now + " Процесс " + nameProcess + " пока не запущен", logFileName);
                            }
                            // Добавляем количество минут для следующего опроса.
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
                    // Есои не все аргументы введены или их слишком много.
                    Console.WriteLine("Не все параметры введены корректно.");
                    Console.WriteLine("Необходимо вести 3 аргумента:");
                    Console.WriteLine("1: Имя процесса, например \"notepad\"");
                    Console.WriteLine("2: Время жизни процесса в минутах");
                    Console.WriteLine("3: Время опроса в минутах");
                    Console.WriteLine("4: Можно задать имя файла лога (Опционально)");
                    Console.WriteLine("Пример1: ConsoleMonitor notepad 5 1");
                    Console.WriteLine("Пример2: ConsoleMonitor notepad 5 1 CustomLogFile.txt");
                }
            }
        }
        /**
         * @Args int idToKill - Убиваем процесс по ID
         */
        public static void KillProcessById(int idToKill)
        {
            Process process = Process.GetProcessById(idToKill);
            process.Kill();
        }

        /**
         * @Args String nameProcess
         * @Output Array of Process where name is nameProcess
         */
        protected static Process[] GetProcess(String nameProcess)
        {
            Process[] prcesses = Process.GetProcessesByName(nameProcess);
            return prcesses;
        }

        public static bool WriteLog(String text, String fileName)
        {
            try
            {
                StreamWriter writer = new System.IO.StreamWriter(fileName,true);
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
