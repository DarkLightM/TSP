using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Semestr
{
    class Program
    {
        public static int[,] AddVertice(int[,] matrix)
        {
            int[,] newMatrix = new int[matrix.GetLength(0) + 1, matrix.GetLength(0) + 1];
            for (int i = 0; i < newMatrix.GetLength(0); i++)
            {
                for (int j = 0; j < newMatrix.GetLength(0); j++)
                {
                    newMatrix[i, j] = matrix[i, j];
                }
            }
            return newMatrix;
        }
        public static int[,] DeleteVertice(int[,] matrix, int vertice)
        {
            int[,] newMatrix = new int[matrix.GetLength(0) - 1, matrix.GetLength(0) - 1];
            for (int i = 0; i < newMatrix.GetLength(0); i++)
            {
                if (i != vertice)
                    for (int j = 0; j < newMatrix.GetLength(0); j++)
                    {
                        if (j != vertice)
                            newMatrix[i, j] = matrix[i, j];
                    }
            }
            return newMatrix;
        }
        public static void AddEdge(int[,] matrix, int i, int j, int value) => matrix[i, j] = value;
        public static void DeleteEdge(int[,] matrix, int i, int j) => matrix[i, j] = Int32.MaxValue;

        public static List<bool> used = new List<bool>();
        public static List<int[]> permutations = new List<int[]>();
        public static List<int> Dynamic(int[,] matrix, int n)
        {
            int[,] dp = new int[1 << n, n];
            for (int i = 0; i < (1 << n); i++)
            {
                for (int j = 0; j < n; j++)
                {
                    dp[i, j] = Int32.MaxValue;
                }
            }
            dp[0, 0] = 0;
            for (int mask = 0; mask < (1 << n); mask++)
            {
                for (int i = 0; i < n; i++)
                {
                    if (dp[mask, i] == Int32.MaxValue) continue;
                    for (int j = 0; j < n; j++)
                    {
                        int a = mask & (1 << j);
                        if (a == 0) // Проверка входит ли город в маску (посещали ли мы уже этот город)
                        {
                            dp[mask | (1 << j), j] = Math.Min(dp[mask | (1 << j), j], dp[mask, i] + matrix[i, j]); //Строим новую маску, включая j город
                        }
                    }
                }
            }
            List<int> result = FindWay(dp, matrix);
            result.Add(dp[(1 << n) - 1, 0]);
            return result;
        }
        public static List<int> FindWay(int[,] dp, int[,] matrix)
        {
            List<int> result = new List<int>();
            int i = 0;
            int n = matrix.GetLength(0);
            int mask = (1 << n) - 1;
            while (true)
            {
                if (i == 0 && mask == 0) break;
                for (int j = 0; j < n; j++)
                {
                    if (dp[mask, i] == matrix[j, i] + dp[mask - (1 << i), j])
                    {
                        result.Add(j);
                        mask -= (1 << i);
                        i = j;

                        break;
                    }
                }
            }
            return result;
        }
        public static void Rec(int idx, int n, int[] array)
        {
            var newArray = new int[array.Length];
            Array.Copy(array, newArray, array.Length);
            if (idx == n - 1)
            {
                permutations.Add(newArray);
                return;
            }
            for (int i = 1; i < n; i++)
            {
                if (used[i])
                    continue;
                newArray[idx] = i;
                used[i] = true;
                Rec(idx + 1, n, newArray);
                used[i] = false;
            }
        }
        public static (int[], int) Permutate(int[,] matrix)
        {
            int minValue = Int32.MaxValue;
            int[] minArray = new int[matrix.GetLength(0)];
            foreach (var array in permutations)
            {
                var newArray = new int[array.Length + 1];
                newArray[0] = 0;
                for (int i = 0; i < array.Length; i++)
                {
                    newArray[i + 1] = array[i];
                }
                int sum = 0;
                for (int i = 0; i < newArray.Length - 1; i++)
                {
                    sum += matrix[newArray[i], newArray[i + 1]];
                    if (sum > minValue) break;
                }
                if (sum < minValue)
                {
                    minValue = sum;
                    minArray = newArray;
                }
            }
            return (minArray, minValue);
        }
        static void Main(string[] args)
        {
            StreamReader reader = new StreamReader("input.txt");
            int size = Convert.ToInt32(reader.ReadLine());
            for (int i = 0; i < size + 1; i++)
                used.Add(false);
            int[,] matrix = new int[size, size];
            for (int i = 0; i < size; i++)
            {
                string[] row = reader.ReadLine().Split(' ');
                for (int j = 0; j < size; j++)
                {
                    if (Int32.TryParse(row[j], out int a))
                        matrix[i, j] = a;
                    else
                        matrix[i, j] = Int32.MaxValue;
                }
            }
            Rec(0, size, new int[size]);
            (int[], int) result = Permutate(matrix);
            int[] array = result.Item1;
            foreach (var elem in array)
            {
                Console.Write(elem + " ");
            }
            Console.Write(result.Item2 + "\n");
            List<int> dresult = Dynamic(matrix, matrix.GetLength(0));
            foreach (var element in dresult)
                Console.Write(element + " ");
        }

    }
}
