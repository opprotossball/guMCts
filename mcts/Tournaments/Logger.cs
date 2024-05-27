using System;
using System.IO;

namespace mcts.Tournaments
{
    public class Logger
    {
        public readonly bool logToConsole;
        public readonly string filePath;
        private readonly object lockObject = new object();
        public Logger() 
        {
            logToConsole = true;
            filePath = "";
        } 

        public Logger(string filePath)
        {
            logToConsole = false;
            this.filePath = filePath;
        }

        public void Log(string message)
        {
            if (logToConsole)
            {
                Console.WriteLine(message);
            } 
            else
            {
                lock (lockObject)
                {

                    if (!File.Exists(filePath))
                    {
                        using (StreamWriter sw = File.CreateText(filePath))
                        {
                            sw.WriteLine(message);
                        }
                    }
                    else
                    {
                        using (StreamWriter sw = File.AppendText(filePath))
                        {
                            sw.WriteLine(message);
                        }
                    }
                }
            }
        }
    }
}
