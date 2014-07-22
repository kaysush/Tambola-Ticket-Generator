using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TambolaTicketGenerator
{
    public class TicketGenerator
    {
        private static int GetLenthOfColumn(int column, int[,] ticket)
        {

            int count = 0;
            int rows = ticket.GetLength(0);
            for (int i = 0; i < rows; i++)
            {
                if (ticket[i, column] != 0)
                    count++;
            }
            return count;
        }

        
        /*
        * Generates a Random number which is not already generated.
        * It actually works by generating a random index, value of that index
        * is used as the random number.
        * In this way this function ensures that no repeatitions are generated.
        */

        private static int GetNextRandom(int[] allNums)
        {
            Random generator = new Random();
            int index, num;
            do
            {
                index = generator.Next(0, allNums.Length);
                num = allNums[index];
            }
            while (num == 0);
            return num;
        }


        /*
        * Removes numbers of a specific range.
        * Once the values for all three rows in a column is generated all the remaining 
        * numbers becomes useless. Hence to lessen the noise those numbers are 
        * removed from the array altogether so that aren't generated in 
        * coming iterations.
        */
        private static void RemoveSeries(int num, ref int[] allNums)
        {
            int index = GetColumnFromNum(num);
            int lowerIndex = (index * 10) + 1;
            int upperIndex = (index + 1) * 10;
            for (int i = lowerIndex; i <= upperIndex; i++)
            {
                allNums[i - 1] = 0;
            }
        }

        /*
        * Given a number find out to which column it belongs.
        * The trick is number divisible by 10 belongs to the range one less than actual.
        */

        private static int GetColumnFromNum(int num)
        {
            if (num % 10 == 0)
            {
                return (num / 10) - 1;
            }
            else
            {
                return num / 10;
            }
        }

        /*
        * Places the number on the ticket at respective location.
        * Once the number is generated, its placed on the ticket if the respective column has any space left.
        * It returns true if number is successfully placed or false if there was no space left to place that number.
        */

        private static bool PlaceOnTicket(int num, ref int[,] ticket, ref int[] allNums, ref int[] rangeLengths)
        {
            int column = GetColumnFromNum(num);
            int length = GetLenthOfColumn(column, ticket);
            if (length < 3)
            {
                ticket[length, column] = num;
                allNums[num - 1] = 0;
                rangeLengths[column]++;
                return true;
            }
            else
            {
                RemoveSeries(num, ref allNums);
                return false;
            }

        }

        /*
        * Returns column number which has given number of elements are present.
        */
        private static int FindColumnForRangeLength(int length, ref int[] rangeLengths)
        {
            for (int i = 0; i < rangeLengths.Length; i++)
            {
                if (rangeLengths[i] == length)
                {
                    rangeLengths[i] = 0;
                    return i;
                }
            }
            return -1;
        }


        /*
        * Shuffle the columns with 2 elements in them.
        * It permutes the columns of the ticket which have 2 values filled.
        */
        private static void Permute2Numbers(ref int[,] ticket, int column, ref int row1Count, ref int row2Count, ref int row3Count)
        {
            Random generator = new Random();
            int choice = generator.Next(0, 3);
            int num1 = ticket[0, column];
            int num2 = ticket[1, column];
            ticket[0, column] = 0;
            ticket[1, column] = 0;

            switch (choice)
            {
                case 0:
                    ticket[0, column] = num1;
                    ticket[1, column] = num2;
                    row1Count++;
                    row2Count++;
                    break;
                case 1:
                    ticket[1, column] = num1;
                    ticket[2, column] = num2;
                    row2Count++;
                    row3Count++;
                    break;
                case 2:
                    ticket[0, column] = num1;
                    ticket[2, column] = num2;
                    row1Count++;
                    row3Count++;
                    break;
            }

        }

        /*
        * Find out which number out of three is the smallest.
        * Instead of returning the smallest number it returns the argument position 
        * which is minimum.
        */
        private static int MinOfThree(int num1, int num2, int num3)
        {
            if (num1 < num2)
            {
                if (num1 < num3)
                {
                    return 1;
                }
                else
                {
                    return 3;
                }
            }
            else
            {
                if (num2 < num3)
                {
                    return 2;
                }
                else
                {
                    return 3;

                }
            }
        }


        /*
        * Shuffle the column which have 1 element in them.
        * It places the elemnt in the row which has lowest number count.
        */
        private static void Place1Numer(ref int[,] ticket, int column, ref int row1Count, ref int row2Count, ref int row3Count)
        {
            int num = ticket[0, column];
            ticket[0, column] = 0;
            switch (MinOfThree(row1Count, row2Count, row3Count))
            {
                case 1:
                    ticket[0, column] = num;
                    row1Count++;
                    break;
                case 2:
                    ticket[1, column] = num;
                    row2Count++;
                    break;
                case 3:
                    ticket[2, column] = num;
                    row3Count++;
                    break;

            }
        }

        /*
        * Re-arranges the ticket to follow all the rules.
        * Once the random numbers are generated this function re-arranges the entire
        * ticket to follow tambola ticket rules.
        */

        private static void RearrangeTicket(ref int[,] ticket, int[] rangeLengths)
        {
            int row1Count = 0, row2Count = 0, row3Count = 0;
            int column;

            #region Arrange All three counts
            while ((column = FindColumnForRangeLength(3, ref rangeLengths)) != -1)
            {
                row1Count++;
                row2Count++;
                row3Count++;
            }
            #endregion


            #region Arrange All two counts
            while ((column = FindColumnForRangeLength(2, ref rangeLengths)) != -1)
            {
                Permute2Numbers(ref ticket, column, ref row1Count, ref row2Count, ref row3Count);
            }
            #endregion

            #region Arrange All one counts
            while ((column = FindColumnForRangeLength(1, ref rangeLengths)) != -1)
            {
                Place1Numer(ref ticket, column, ref row1Count, ref row2Count, ref row3Count);
            }
            #endregion


        }

        /*
        * Pretty print the ticket.
        * Just for the purpose of debugging.
        * Can only be used in console environment.
        */

        public static void PrintTicket(int[,] ticket)
        {

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (ticket[i, j] != 0)
                        Console.Write("{0}\t", ticket[i, j]);
                    else
                        Console.Write("{0}\t", "--");
                }
                Console.WriteLine();
            }

        }

        /*
        * Use the above function to generated the final ticket.
        * This is the public interface to this aseembly. 
        * Other projects can use this functinality.
        */
        public static void GenerateTicket(ref int[,] ticket)
        {
            int[] allNums = new int[90];
            //int[,] ticket = new int[3,9];
            int[] rangeLengths = new int[9];

            #region Fill Initial All Num array
            for (int i = 0; i < 90; i++)
            {
                allNums[i] = i + 1;
            }
            #endregion

            #region Generate 9 initial nos.
            Random generator = new Random();
            for (int i = 1; i <= 9; i++)
            {
                int lowerBound = ((i - 1) * 10) + 1;
                int upperBound = (i * 10) + 1;
                int num = generator.Next(lowerBound, upperBound);
                allNums[num - 1] = 0; // Removing that number from the all numbers array
                int column = i - 1;
                int row = 0;
                ticket[row, column] = num;
                rangeLengths[column]++;

            }
            #endregion

            #region Generate 6 other nos.
            int numbersGenerated = 0;
            while (numbersGenerated != 6)
            {
                int num = GetNextRandom(allNums);
                bool inserted = PlaceOnTicket(num, ref ticket, ref allNums, ref rangeLengths);
                if (inserted)
                {
                    numbersGenerated++;
                }
            }
            #endregion

            RearrangeTicket(ref ticket, rangeLengths);



        }
    }
}
