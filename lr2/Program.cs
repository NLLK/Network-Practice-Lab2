using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace lr2
{
    class Program
    {
        const float noiseLevelQ = 0.05f;//0.05f;
        const int BasicWaitTime = 150;
        const int DelayBetweenThreads = BasicWaitTime / 2;
        const int DelayBetweenPackets = 300;

        static void Main(string[] args)
        {
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;

            //string myName = "Бабанов Артем Леонидович 10.11.2000";
            //int[] myNameCoded = CodeMyName(myName);

            int[] StartSequence = { 1, 0, 1, 0 };

            ThreadedThings tt = new ThreadedThings(BasicWaitTime, StartSequence, 16);

            Thread listenerThread = new Thread(tt.Listener);
            listenerThread.Start();

            Thread.Sleep(DelayBetweenThreads);


            Thread transmitterThread = new Thread(tt.Transmitter);

            int[] message = { 0, 0, 1, 0, 0, 0, 1, 0, 0, 1, 1, 0, 0, 0, 1, 0 };

            transmitterThread.Start(message);

            Thread.Sleep(ThreadedThings.SleepTimeRandomizer((message.Length+10)*BasicWaitTime, 20));

            Console.WriteLine("M: Complited = true");

            tt.Complited = true;

            /*ThreadedThings tt = new ThreadedThings(100, 10, 20);
            Thread listenerThread = new Thread(tt.Listener);
            listenerThread.Start();

            Thread.Sleep(315);

            Thread transmitterThread = new Thread(tt.Transmitter);
            int[] data = new int[] { 1, 0, 0, 1, 0, 0, 0, 1, 1, 1 };//myNameCoded;//new int[] { 1,0,0,1,0,0,0,1,1,1};

            transmitterThread.Start(data);

            Thread.Sleep(4444);//Thread.Sleep(35000);
            tt.COMPLITED = true;

            UltimateWriter("Отправлен массив: ", data);
            int[] received = new int[tt.ReceivedIndex];
            Array.Copy(tt.Received, received, tt.ReceivedIndex);
            UltimateWriter("Получен массив: ", received);*/
            /*
                        Console.WriteLine("Запуск двух потоков передачи: ");

                        tt.ReceivedIndex = 0;

                        transmitterThread = new Thread(tt.Transmitter);
                        data = new int[] { 1,0,0,1,0,0,0,1,1,1};
                        transmitterThread.Start(data);
                        UltimateWriter("Отправлен массив: ", data);

                        Thread.Sleep(300);

                        Thread transmitterThread1 = new Thread(tt.Transmitter);
                        data = new int[] { 0,0,1,0,0,1,0,1,0,0,1 };
                        transmitterThread1.Start(data);
                        UltimateWriter("Отправлен массив: ", data);

                        Thread.Sleep(5000);
                        tt.COMPLITED = true;
                        received = new int[tt.ReceivedIndex];
                        Array.Copy(tt.Received, received, tt.ReceivedIndex);
                        UltimateWriter("Получен массив: ", received);



                        Console.WriteLine("Ускоряем в два раза: \n");

                        ThreadedThings tt2 = new ThreadedThings(50, 5, 10);
                        Thread listenerThread2 = new Thread(tt2.Listener);
                        listenerThread2.Start();

                        Thread.Sleep(315);

                        Thread transmitterThread2 = new Thread(tt2.Transmitter);
                        int[] data2 = new int[] { 1, 0, 0, 1, 0, 0, 0, 1, 1, 1 };//myNameCoded;//new int[] { 1,0,0,1,0,0,0,1,1,1};

                        transmitterThread2.Start(data);

                        Thread.Sleep(4444);//Thread.Sleep(35000);
                        tt2.COMPLITED = true;

                        UltimateWriter("Отправлен массив: ", data);
                        int[] received2 = new int[tt2.ReceivedIndex];
                        Array.Copy(tt2.Received, received2, tt2.ReceivedIndex);
                        UltimateWriter("Получен массив: ", received2);
            */
            Console.ReadKey();

        }

        static void UltimateWriter(string str, int[] array)
        {
            string arrayStr = ArrayFunctions.ArrayToString(array);
            Console.WriteLine($"{str}{arrayStr}");
        }

        static List<int[]> SplitToPackets(int[] array, int volume)
        {
            List<int[]> output = new List<int[]>();

            for (int i = 0; i < array.Length;)
            {
                int[] packet = new int[volume];
                int j = 0;
                for (j = 0; j < volume; j++)
                {
                    if (i < array.Length)
                    {
                        packet[j] = array[i];
                        i++;
                    }
                }
                output.Add(packet);
            }

            return output;
        }

        static int[] CodeMyName(string name)
        {
            int[] array = new int[name.Length];
            int i = 0;
            foreach (char c in name)
            {
                if (c > 1000) array[i] = c - 'А' + 0xC0;
                else array[i] = c;
                i++;
            }

            int[] byteArray = new int[name.Length * 8];

            int j = 0;
            foreach (int integer in array)
            {
                int[] intArray = ArrayFunctions.NumToIntArray(integer, 8);
                for (i = j; i < intArray.Length + j; i++)
                    byteArray[i] = intArray[i - j];
                j = i;
            }

            return byteArray;
        }
        static int[] getArray(int number)
        {
            int[] array = new int[number];
            bool inputed = false;
            while (!inputed)
            {
                Console.WriteLine($"Введите последовательность из {number} бит (только 0 или 1): ");
                string input = Console.ReadLine();
                if (input.Length != number)
                {
                    Console.WriteLine("Введена неверная строка");
                    inputed = false;
                    continue;
                }
                int a = 0;
                try
                {
                    a = Convert.ToInt32(input);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Введена неверная строка");
                    inputed = false;
                    continue;
                }
                inputed = true;
                for (int i = 0; i < input.Length; i++)
                {
                    char c = input[i];
                    int bit = c - '0';
                    if (bit != 0 && bit != 1)
                    {
                        Console.WriteLine("Введена неверная строка");
                        inputed = false;
                        break;
                    }
                    else
                    {
                        array[i] = bit;
                    }
                }
            }

            return array;
        }
        static float CheckRedundancy(int k, int i)
        {
            float redundancy = (float)k / (i + k);
            return redundancy;
        }
        static int[] CodingMessageParityMethod(int[] message)// метод четности
        {
            int[] array = new int[message.Length + 1];
            message.CopyTo(array, 0);

            int controlBit = 0;

            foreach (int i in message)
                if (i == 1) controlBit++;//xor

            array[array.Length - 1] = controlBit % 2;
            return array;
        }
        static bool DecodingMessageParityMethod(int[] message)
        {
            int controlBit = message[0];

            for (int i = 1; i < message.Length; i++)
                controlBit ^= message[i];//xor

            return (controlBit == 0) ? true : false;
        }

        static int[] CodingMessageBergerMethod(int[] message, int codingBits)
        {
            int[] array = new int[message.Length + codingBits];
            message.CopyTo(array, 0);

            int control = 0;
            foreach (int i in message)
                if (i == 1) control++;

            //control = ~control;

            int[] intArray = ArrayFunctions.NumToIntArray(control, codingBits);

            for (int i = 0; i < codingBits; i++)
                intArray[i] = (intArray[i] == 0) ? 1 : 0;

            intArray.CopyTo(array, message.Length);

            return array;
        }
        static bool DecodingMessageBergerMethod(int[] message, int codingBits)
        {
            int control = 0;
            for (int i = 0; i < message.Length - codingBits; i++)
                if (message[i] == 1) control++;

            int[] codingArray = new int[codingBits];
            Array.Copy(message, message.Length - codingBits, codingArray, 0, codingBits);

            int[] codingArrayInv = new int[codingBits];
            for (int i = 0; i < codingBits; i++)
                codingArrayInv[i] = (codingArray[i] == 0) ? 1 : 0;

            int[] intArray = ArrayFunctions.NumToIntArray(control, codingBits);
            bool ans = ArrayFunctions.CompareArrays(intArray, codingArrayInv);

            return ans;
        }

        static int[] CodingMessageHammingMethod(int[] message)
        {
            int power = ArrayFunctions.GetInfoBits(message);

            int[] array = new int[message.Length + power + 1];


            int[] controlIndexs = new int[power + 1];//1,2,4,8,16...
            int num = 1;
            int ind = 1;
            for (ind = 1; ind < power + 1; ind++)
            {
                controlIndexs[ind - 1] = num;
                num *= 2;
            }
            controlIndexs[ind - 1] = num;

            int[] dataIndexs = new int[message.Length];//3,5,6,7,9,10,11,12...

            int dataI = 0;

            for (int i = 0; i < array.Length; i++)
            {
                bool flag = true;
                foreach (int id in controlIndexs)
                {
                    if ((id - 1) == i)
                    {
                        flag = false;
                        break;
                    }
                }
                if (flag)
                {
                    dataIndexs[dataI] = i + 1;
                    dataI++;
                }

            }
            //переписать сообщение в массив для дальнейшей передачи
            int mesI = 0;

            foreach (int dataInd in dataIndexs)
            {
                array[dataInd - 1] = message[mesI];
                mesI++;
            }
            foreach (int conrolIndex in controlIndexs)
            {
                int controlBit = 0;

                for (int i = conrolIndex - 1; i < array.Length;)
                {
                    for (int j = 0; j < conrolIndex; j++)
                    {
                        //check j bits
                        controlBit += array[i];

                        i++;
                        if (i >= array.Length)
                            break;

                    }
                    //skip j bits
                    i += conrolIndex;

                }
                array[conrolIndex - 1] = controlBit % 2;
            }

            return array;
        }

        static bool DecodingMessageHammingMethod(int[] message, int power)
        {
            int[] controlIndexs = new int[power + 1];//1,2,4,8,16...
            int num = 1;
            int ind = 1;
            for (ind = 1; ind < power + 1; ind++)
            {
                controlIndexs[ind - 1] = num;
                num *= 2;
            }
            controlIndexs[ind - 1] = num;

            /*            int[] controlBits = new int[power + 1];
                        int iterator = 0;
                        foreach (int index in controlIndexs)
                        {
                            controlBits[iterator] = message[index - 1];
                            iterator++;
                        }*/

            int[] controlBitsNew = new int[power + 1];
            int iNew = 0;

            foreach (int controlIndex in controlIndexs)
            {
                int controlBit = 0;

                for (int i = controlIndex - 1; i < message.Length;)
                {
                    for (int j = 0; j < controlIndex; j++)
                    {
                        //check j bits
                        controlBit += message[i];

                        i++;
                        if (i >= message.Length)
                            break;

                    }
                    //skip j bits
                    i += controlIndex;

                }
                controlBitsNew[iNew] = controlBit % 2;

                //message[controlIndex - 1] = controlBit % 2;
                iNew++;
            }

            bool ans = true;
            foreach (int bit in controlBitsNew)
            {
                if (bit != 0)
                {
                    ans = false;
                    break;
                }
            }


            return ans;
        }

        static int[] FixingMessageHammingMethod(int[] message, int power)
        {
            int[] controlIndexs = new int[power + 1];//1,2,4,8,16...
            int num = 1;
            int ind = 1;
            for (ind = 1; ind < power + 1; ind++)
            {
                controlIndexs[ind - 1] = num;
                num *= 2;
            }
            controlIndexs[ind - 1] = num;

            int[] controlBits = new int[power + 1];
            int iterator = 0;
            foreach (int index in controlIndexs)
            {
                controlBits[iterator] = message[index - 1];
                iterator++;
            }

            int[] controlBitsNew = new int[power + 1];
            int iNew = 0;
            foreach (int controlIndex in controlIndexs)
            {
                int controlBit = 0;

                for (int i = controlIndex - 1; i < message.Length;)
                {
                    for (int j = 0; j < controlIndex; j++)
                    {
                        //check j bits
                        controlBit += message[i];

                        i++;
                        if (i >= message.Length)
                            break;

                    }
                    //skip j bits
                    i += controlIndex;

                }
                controlBitsNew[iNew] = controlBit % 2;
                iNew++;
            }

            int badBitInd = 0;

            int multiply = 1;
            for (int i = 0; i < controlBitsNew.Length; i++)
            {
                if (controlBitsNew[i] == 1)
                {
                    badBitInd += multiply;
                }
                multiply *= 2;
            }
            badBitInd--;
            if (badBitInd < message.Length && badBitInd >= 0)
                message[badBitInd] = (message[badBitInd] == 0) ? 1 : 0;

            return message;
        }

        static int[] NoiseGeneration(int[] message)
        {
            int[] output = (int[])message.Clone();
            Random random = new Random();
            for (int i = 0; i < message.Length; i++)
            {
                if ((float)random.NextDouble() < noiseLevelQ)
                    output[i] = (message[i] == 1) ? 0 : 1;
            }
            return output;
        }
        static int NoiseGeneration(int bit)
        {
            Random random = new Random();
            int output = 0;
            if ((float)random.NextDouble() < noiseLevelQ)
                output = (bit == 1) ? 0 : 1;
            return output;
        }

    }
}
