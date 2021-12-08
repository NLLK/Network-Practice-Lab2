using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace lr2
{
	class ThreadedThings
	{
		private int BasicWaitTime = 50;
		static int PacketSize = 16;

		int[] StartBuffer = new int[4];

		int[] StartSequence = { 1, 0, 1, 0 };

		public bool Complited = false;
        private int LINE;
		private bool RE_SYNC;

		public int[] Received = new int[1];

		public ThreadedThings(int _basicWaitTime, int[] _startSequence, int _packetSize)
		{
			BasicWaitTime = _basicWaitTime;
			StartSequence = _startSequence;
			PacketSize = _packetSize;
			Received = new int[PacketSize];
			//BufferSize = PacketSize + StartSequence.Length;
		}
		public void Transmitter(object _message)
		{
			int[] message = (int[])_message;
			int[] messageWithSS = AddStartSequence(StartSequence, message);

			int ampLevel1 = 2;
			int ampLevel0 = 1;

			for (int i = 0; i < messageWithSS.Length; i++)
			{
				LINE = (messageWithSS[i] == 0) ? ampLevel0 : ampLevel1;
				Console.ForegroundColor = ConsoleColor.Black;
				Console.WriteLine($"T: передал бит {LINE-1}");
				Thread.Sleep(SleepTimeRandomizer(BasicWaitTime, 10));
			}
			LINE = 0;
			Console.ForegroundColor = ConsoleColor.DarkBlue;
			Console.WriteLine($"T: передача закончена");

		}

		public void Receiver()
		{
			Console.ForegroundColor = ConsoleColor.DarkBlue;
			Console.WriteLine("R: " + "Приемник запущен...");
			int sleepTime = 10;
			bool receivingPhase = false; 
			while (!RE_SYNC && !Complited)
			{
				int buf = LINE;
				if (buf != 0)
				{
					Console.ForegroundColor = ConsoleColor.Magenta;
					Console.WriteLine("R: " + $"Получен бит: {buf - 1}");

					if (receivingPhase)
					{
						Received = ArrayFunctions.LeftShiftArray(Received);
						Received[Received.Length - 1] = buf - 1;
					}
					else
					{
						//считать линию-1
						int currentBit = buf - 1;

						//сдвинуть вправо и записать в конец
						int[] shiftedBuffer = ArrayFunctions.LeftShiftArray(StartBuffer);

						if (currentBit < 0) currentBit = 0;
						shiftedBuffer[shiftedBuffer.Length - 1] = currentBit;

						StartBuffer = shiftedBuffer;

						//сравнить буфер и стартовую последовательность
						if (ArrayFunctions.CompareArrays(StartBuffer, StartSequence))
						{
							Console.ForegroundColor = ConsoleColor.Blue;
							Console.WriteLine("R: обнаружена стартовая последовательность");
							receivingPhase = true;
						}
					}
				}

				bool breaking = false;
				for (int i = 0; i < 5; i++)//35-50 ms
				{
					Thread.Sleep(SleepTimeRandomizer(sleepTime, 3));
					if (RE_SYNC)
					{
						breaking = true;
						break;
					}
				}
				if (breaking) break;
			}
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine("R: " + "Приемник перезагрузился...");
		}

		public void Listener()
		{
			Console.ForegroundColor = ConsoleColor.DarkBlue;
			Console.WriteLine("L: Запуск прослушивания");
			int buf = 0;
			while (!Complited)
			{
				buf = LINE;

				Thread.Sleep(SleepTimeRandomizer(BasicWaitTime, 10));
				if (buf != LINE)
				{
					Console.ForegroundColor = ConsoleColor.Red;
					Console.WriteLine("L: Пересинхронизация");
					RE_SYNC = true;
					Thread.Sleep(10);
					RE_SYNC = false;
					//запуск потока приемника
					Thread receiverThread = new Thread(this.Receiver);
					receiverThread.Start();
				}
				
			}		
		}
		static public int SleepTimeRandomizer(int baseTime, int approximation)
		{
			Random random = new Random();
			return baseTime;
			//return baseTime - random.Next(0, approximation);
		}

		static int[] AddStartSequence(int[] startSequence, int[] message)
		{
			int[] output = new int[message.Length + startSequence.Length];

			int i = 0;
			for (i = 0; i < startSequence.Length; i++)
				output[i] = startSequence[i];

			for (; i < output.Length - 1; i++)
				output[i] = message[i - startSequence.Length];

			return output;
		}

	}
}
