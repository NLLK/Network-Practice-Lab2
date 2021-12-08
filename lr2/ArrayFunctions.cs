using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lr2
{
    static class ArrayFunctions
    {
        public static int GetInfoBits(int[] message)
        {
            float log = (float)Math.Log2(message.Length);
            if (log > (int)log) log = (int)log + 1;
            return (int)log;
        }

        public static int[] NumToIntArray(int num, int volume)
        {
            bool[] bitArray = ToBits(num, volume);
            int[] intArray = new int[volume];
            Array.Reverse(bitArray);

            for (int i = 0; i < volume; i++)
                if (bitArray[i]) intArray[i] = 1;

            return intArray;
        }

        public static bool[] ToBits(int input, int numberOfBits)
        {
            return Enumerable.Range(0, numberOfBits)
            .Select(bitIndex => 1 << bitIndex)
            .Select(bitMask => (input & bitMask) == bitMask)
            .ToArray();
        }
        public static string ArrayToString(int[] array)
        {
            char[] output = new char[array.Length + 1];
            int index = 0;
            foreach (int i in array)
            {
                output[index] = (char)(i + '0');
                index++;
            }
            //output[array.Length] = '\n';
            return new string(output);
        }
        public static int[] LeftShiftArray(int[] array)
        {
            int[] output = new int[array.Length];
            for (int i = 0; i < array.Length - 1; i++)
            {
                output[i] = array[i + 1];
            }
            output[array.Length - 1] = 0;
            return output;
        }
        public static bool CompareArrays(int[] array1, int[] array2)
        {
            for (int i = 0; i < array1.Length; i++)
            {
                if (array1[i] != array2[i]) return false;
            }
            return true;
        }
    }
}
