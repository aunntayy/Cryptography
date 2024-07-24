using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace Cryptography
{
    internal class Program
    {
        static readonly Matrix<double> key26 = DenseMatrix.OfArray(new double[,] {
            { 6, 11 },
            { 25, 15 }
        });

        static readonly Matrix<double> key29 = DenseMatrix.OfArray(new double[,] {
            {28,7 },
            {19,18 }
        });
        static readonly Dictionary<char, int> value = new Dictionary<char, int>();
        static readonly int mod26 = 26;
        static readonly int mod29 = 29;
        static void Main(string[] args)
        {
            InitializeDictionary();
            string message = "LYNY JRVMQNS JL ! ";
            string decrypt29 = Decrypt(message, mod29, key29);
            string decrypt26 = Decrypt(decrypt29, mod26, key26);
            Console.WriteLine(decrypt26);

            string message2 = "";
        }

        static void InitializeDictionary() {
            for (char c = 'A'; c <= 'Z'; c++) {
                value[c] = c - 'A';
            }
            value[' '] = 26;
            value['?'] = 27;
            value['!'] = 28;
        }

        static int ValueOfCharacter(char alphabet) {
            if (value.TryGetValue(alphabet, out int val)) {
                return val;
            }
            throw new ArgumentException($"Character '{alphabet}' not in dictionary.");
        }

        static char CharacterOfValue(int value) {
            value = (value % 26 + 26) % 26; // For mod 26; adjust if mod is different

            // Handle values that map to special characters
            if (value == 26) return ' ';
            if (value == 27) return '?';
            if (value == 28) return '!';

            // For letters A-Z
            return (char)(value + 'A');
        }

        static string Encrypt(string input ,int mod, Matrix<double> key)
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


                encryptedChars.Add(CharacterOfValue((int)result[0, 0] % mod));
                encryptedChars.Add(CharacterOfValue((int)result[1, 0] % mod));
            }

            // Return the encrypted string
            return new string(encryptedChars.ToArray());
        }

        static string Decrypt(string input, int mod, Matrix<double> key) { 
            List<char> decryptedChars = new List<char>();
            Matrix<double> inverseMatrix = FindInverseMatrix(key,mod);
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

                decryptedChars.Add(CharacterOfValue((int)result[0, 0] % mod));
                decryptedChars.Add(CharacterOfValue((int)result[1, 0] % mod));
            }
            return new string(decryptedChars.ToArray());
        }
        static Matrix<double> FindInverseMatrix(Matrix<double> matrix , int mod)
        {
            double det = matrix.Determinant();
           
            
            det = FindModularInverse((int)det, mod);
            Matrix<double> keyIn = FindModularMatrix(matrix, mod);
            keyIn = keyIn.Multiply(det);

            Matrix<double> result = DenseMatrix.OfArray(new double[,] { {0,0 },{0,0 } });

            result[0,0] = keyIn[0, 0] % mod;
            result[0,1] = keyIn[0, 1] % mod;
            result[1,0] = keyIn[1, 0] % mod;
            result[1,1] = keyIn[1, 1] % mod;  
           
            return result;
        }

        //find k^-1
        public static Matrix<double> FindModularMatrix(Matrix<double> keyMatrix, int mod ) {

            Matrix<double> result = DenseMatrix.OfArray(new double[2,2] { 
                { keyMatrix[1,1], -keyMatrix[0,1] },
                { -keyMatrix[1,0], keyMatrix[0,0] } });
        

            for (int i = 0; i < keyMatrix.ColumnCount; i++) {
                for(int  j = 0; j < keyMatrix.RowCount;j++) {
                    
                    if (result[i,j] < 0) {
                        result[i, j] = (result[i, j] % mod + mod) % mod;
                    } else {
                        result[i,j] = result[i,j] % mod;
                    }
                }
            }

            return result;
        }
        public static int FindModularInverse(int a, int mod)
        {
            a = (a % mod + mod) % mod;
            int m0 = mod, t, q;
            int x0 = 0, x1 = 1;
       
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
