using System;
using System.Collections.Generic;
using System.Xml;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace Cryptography
{
    internal class Program
    {
        static readonly Matrix<double> key = DenseMatrix.OfArray(new double[,] {
            { 3, 3 },
            { 2, 5 }
        });

        static  Matrix<double> keyIn = DenseMatrix.OfArray(new double[,] {
            { 5, 23 },
            { 24, 3 }
        });


        static void Main(string[] args)
        {
            string input = "TRYTOBREAKTHISCODE";
            string encrypted = Encrypt(input);
            Console.WriteLine("Encrypted Text: " + encrypted);
        
           
            string decry = Decrypt(encrypted);
            Console.WriteLine(decry);
       
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

        static string Decrypt(string input) { 
            List<char> decryptedChars = new List<char>();
            Matrix<double> inverseMatrix = FindInverseMatrix(key);
            for(int i = 0; i < input.Length; i+=2)
            {
                int num1 = ValueOfCharacter(input[i]);
                int num2 = ValueOfCharacter(input[i + 1]);

                // Create the column vector for the pair of characters
                Matrix<double> K = DenseMatrix.OfArray(new double[,]
                {
                    { num1 },
                    { num2 }
                });

                Matrix<double> result = inverseMatrix.Multiply(K);

                decryptedChars.Add((char)(((int)result[0, 0]%26) + 'A'));
                decryptedChars.Add((char)(((int)result[1, 0]%26) + 'A'));
            }
            return new string(decryptedChars.ToArray());
        }
        static Matrix<double> FindInverseMatrix(Matrix<double> matrix)
        {
            double det = matrix.Determinant();
           // det = Math.Abs(det);
            int result = (int)det;
            result = FindModularInverse(result, 26);
            keyIn = keyIn.Multiply(result);


            keyIn[0,0] = keyIn[0, 0] % 26;
            keyIn[0,1] = keyIn[0, 1] % 26;
            keyIn[1,0] = keyIn[1, 0] % 26;
            keyIn[1,1] = keyIn[1, 1] % 26;  
           
            return keyIn;
        }
        public static int FindModularInverse(int a, int mod)
        {
            a = (a % mod + mod) % mod;

            int m0 = mod, t, q;
            int x0 = 0, x1 = 1;

            if (mod == 1)
                return 0;

            while (a > 1)
            {
                // q is quotient
                q = a / mod;
                t = mod;

                // m is remainder now, process same as Euclid's algorithm
                mod = a % mod;
                a = t;
                t = x0;
                x0 = x1 - q * x0;
                x1 = t;
            }

            // Make x1 positive
            if (x1 < 0)
                x1 += m0;

            return x1;
        }

    }
}
