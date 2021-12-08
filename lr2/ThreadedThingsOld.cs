using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace lr2
{
    class ThreadedThingsOld
    {
        int LINE = 0;
        bool RE_SYNC = false;
        public bool COMPLITED = false;
        public int[] Received = new int[1];
        public int ReceivedIndex = 0;

        static float noiseLevelQ = 0.01f;

        int sleepTimeTransmitter = 0; int sleepTimeReceiver = 0; int sleepTimeListener = 0;
        public ThreadedThingsOld(int _sleepTimeTransmitter, int _sleepTimeReceiver, int _sleepTimeListener)
        {
            sleepTimeTransmitter = _sleepTimeTransmitter;
            sleepTimeReceiver = _sleepTimeReceiver;
            sleepTimeListener = _sleepTimeListener;
        }
        public void Transmitter(object _message)
        {
            int[] message = (int[])_message;
            Received = new int[message.Length * 10];
            int sleepTime = sleepTimeTransmitter;
            int ampLevel1 = 2;
            int ampLevel0 = 1;
            int[] array = new int[message.Length];
            message.CopyTo(array, 0);
            for (int i = 0; i < message.Length; i++)
            {
                LINE = (message[i] == 0) ? ampLevel0 : ampLevel1;
                Console.WriteLine("T: " + $"Передал {LINE - 1}");
                Thread.Sleep(sleepTime);
            }
            LINE = 0;
            Console.WriteLine("T: " + "Закончил передачу");
        }

        public void Receiver()
        {
            Console.WriteLine("R: " + "Приемник запущен...");
            int sleepTime = (int)(sleepTimeReceiver * 1.0);
            while (!RE_SYNC)
            {
                Thread.Sleep(sleepTime);
                if (LINE != 0)
                {
                    Console.WriteLine("R: " + $"Получен бит: {LINE - 1}");
                    Received[ReceivedIndex] = LINE - 1;
                    ReceivedIndex++;
                }

                bool breaking = false;
                for (int i = 0; i < 8; i++)
                {
                    Thread.Sleep(sleepTime);
                    if (RE_SYNC)
                    {
                        breaking = true;
                        break;
                    }
                }
                if (breaking) break;
                Random random = new Random();
                Thread.Sleep(random.Next(0, 10));
            }
            Console.WriteLine("R: " + "Приемник перезагрузился...");
        }

        public void Listener()
        {
            int sleepTime = sleepTimeListener;
            int buf = 1;
            while (!COMPLITED)
            {
                buf = LINE;
                Thread.Sleep(sleepTime);

                if (buf != LINE) //(Math.Abs(buf - LINE) > 0)
                {
                    Console.WriteLine("L: " + $"{Math.Abs(buf - LINE)};");
                    Console.WriteLine("L: " + "Cинхронизация...");
                    RE_SYNC = true;
                    Thread.Sleep(sleepTime / 2);
                    RE_SYNC = false;
                    Console.WriteLine("L: " + "Cинхронизация завершена");
                    //запуск потока приемника
                    Thread receiverThread = new Thread(this.Receiver);
                    receiverThread.Start();

                }
            }
            Console.WriteLine("L: " + "Окончание выполнения программы.");
        }
        public void NoiseGenerator()
        {
            Thread.Sleep(100);
            Random random = new Random();

            if ((float)random.NextDouble() < noiseLevelQ)
                LINE = (LINE == 1) ? 0 : 1;
        }
    }
}
