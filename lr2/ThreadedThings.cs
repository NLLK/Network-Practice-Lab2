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

		int receivingThreadsNumber = 0;
		bool receivingPhase = false;
		Thread receiverThread;


		public ThreadedThings(int _basicWaitTime, int[] _startSequence, int _packetSize)
		{
			BasicWaitTime = _basicWaitTime;
			StartSequence = _startSequence;
			PacketSize = _packetSize;
			Received = new int[PacketSize];
			receiverThread = new Thread(this.Receiver);
			//BufferSize = PacketSize + StartSequence.Length;
		}
		public void Transmitter(object _message)
		{
			receivingPhase = false;
			int[] message = (int[])_message;
			int[] messageWithSS = AddStartSequence(StartSequence, message);

			int ampLevel1 = 2;
			int ampLevel0 = 1;

			ConsoleColor color = ConsoleColor.DarkBlue;

			ConsoleWriteWithColor("T: запуск передачи", color);

			for (int i = 0; i < messageWithSS.Length; i++)
			{
				LINE = (messageWithSS[i] == 0) ? ampLevel0 : ampLevel1;
				ConsoleWriteWithColor($" . T: передал бит {LINE-1}", color);
				Thread.Sleep(SleepTimeRandomizer(BasicWaitTime, 10));
			}
			LINE = 0;
			ConsoleWriteWithColor($"T: передача закончена", color);

		}

		public void Receiver()
		{
			ConsoleColor color = ConsoleColor.Magenta;
			//receivingThreadsNumber++;

			/*while (receivingThreadsNumber != 1)
			{
				
			}*/
			while (!Complited)
			{
				RE_SYNC = false;

				ConsoleWriteWithColor("R: " + $"Приемник запущен... Поток {receivingThreadsNumber}", color);
				while (!RE_SYNC && !Complited)
				{
					if (RE_SYNC) break;
					int buf = LINE;
					if (buf != 0)
					{
						ConsoleWriteWithColor(" . . R: " + $"Получен бит: {buf - 1}", color);

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
								ConsoleWriteWithColor("R: обнаружена стартовая последовательность", color);
								//receivingPhase = true;
							}
						}
					}
					else
						ConsoleWriteWithColor("R: " + $"На линии 0", color);
					bool breaking = false;
					int sleepTime = 10;
					ConsoleWriteWithColor("R: " + $"Спим {BasicWaitTime} мс", color);
					/*for (int i = 0; i < BasicWaitTime; i+=sleepTime)//35-50 ms
					{
						if (RE_SYNC)
						{
							breaking = true;
							break;
						}
						ConsoleWriteWithColor("R: " + $"Спим", color);
						Thread.Sleep(SleepTimeRandomizer(sleepTime, 3));
					}*/

					DateTime startedWaiting = DateTime.Now;
					while (DateTime.Now.Subtract(startedWaiting).TotalMilliseconds < BasicWaitTime)
					{
						if (RE_SYNC)
						{
							breaking = true;
							break;
						}
						//ConsoleWriteWithColor("R: " + $"Спим", color);
						Thread.Sleep(SleepTimeRandomizer(sleepTime, 3));
					}

					ConsoleWriteWithColor("R: " + $"Проспались", color);
					if (breaking) break;
				}
				ConsoleWriteWithColor("R: " + "Приемник выключен", color);
				//receivingThreadsNumber--;
			}
		}

		public void Listener()
		{
			ConsoleColor color = ConsoleColor.Red;
			ConsoleWriteWithColor("L: Запуск прослушивания", color);

			int buf = LINE;
			while (buf == 0)
			{
				Thread.Sleep(1);
				buf = LINE;
			}
			ConsoleWriteWithColor("L: Зафиксировано начало передачи", color);

			receiverThread.Start();

			buf = 0;
			
			while (!Complited)
			{
				buf = LINE;
				ConsoleWriteWithColor($"L: Было {buf}", color);
				Thread.Sleep(SleepTimeRandomizer(BasicWaitTime, 10));
				ConsoleWriteWithColor($"L: Стало {LINE}", color);
				if (buf != LINE)
				{
					ConsoleWriteWithColor("L: Пересинхронизация", color);
					RE_SYNC = true;
					
					//запуск потока приемника
					//receiverThread = new Thread(this.Receiver);
					//receiverThread.Start();
					ConsoleWriteWithColor("L: Запуск потока приемника", color);
				}
				
			}		
		}
		static public int SleepTimeRandomizer(int baseTime, int approximation)
		{
			//Random random = new Random();
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
		static void ConsoleWriteWithColor(string str, ConsoleColor color)
		{
			Console.ForegroundColor = color;
			DateTime now = DateTime.Now;
			Console.WriteLine($"{now.Millisecond:D3} . . . . . {str}");
			//Console.ForegroundColor = ConsoleColor.Black;
		}

	}
}
