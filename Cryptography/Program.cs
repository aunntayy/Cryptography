using System;
using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace Cryptography
{
    internal class Program
    {
        static readonly Matrix<double> key = DenseMatrix.OfArray(new double[,] {
            { 6, 11 },
            { 25, 15 }
        });

        static void Main(string[] args)
        {
            string input = "TRYTOBREAKTHISCODE";
            string encrypted = Encrypt(input);
            Console.WriteLine("Encrypted Text: " + encrypted);
        }

        /// <summary>
        /// This method returns the value of the character
        /// </summary>
        /// <param name="alphabet"></param>
        /// <returns>Value of character</returns>
        static int ValueOfCharacter(char alphabet)
        {
            Dictionary<char, int> value = new Dictionary<char, int>();
            for (char c = 'A'; c <= 'Z'; c++)
            {
                value[c] = c - 'A';
            }
            value[' '] = 26;
            value['?'] = 27;
            value['!'] = 28;
            return value[alphabet];
        }

        static string Encrypt(string input)
        {
          
            List<char> encryptedChars = new List<char>();

            for (int i = 0; i < input.Length; i += 2)
            {
                // Convert characters to numbers
                int num1 = ValueOfCharacter(input[i]);
                int num2 = ValueOfCharacter(input[i + 1]);

                // Create the column vector for the pair of characters
                Matrix<double> K = DenseMatrix.OfArray(new double[,]
                {
                    { num1 },
                    { num2 }
                });

                // Multiply the key matrix with the character vector
                Matrix<double> result = key.Multiply(K);

                // Convert the resulting numbers back to characters
                encryptedChars.Add((char)(((int)result[0, 0] % 26) + 'A'));
                encryptedChars.Add((char)(((int)result[1, 0] % 26) + 'A'));
            }

            // Return the encrypted string
            return new string(encryptedChars.ToArray());
        }
    }
}
