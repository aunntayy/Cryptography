/*
 * Cryptography Program
 * ---------------------
 * This program demonstrates basic cryptographic operations using the Hill cipher with 2x2 matrices.
 * It supports encryption and decryption of messages using matrices with different moduli (26 and 29).
 */
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
            { 6,11 },
            { 25,15 }
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

            string message2 = "TRYTOBREAKTHISCODE";
            string encry = Encrypt(message2, mod29, key29);

            Console.WriteLine(Decrypt(encry, mod29, key29));
        }

        /// <summary>
        /// Initializes the dictionary for character-to-value mapping.
        /// Maps letters A-Z to 0-25, space to 26, question mark to 27, and exclamation mark to 28.
        /// </summary>
        static void InitializeDictionary()
        {
            for (char c = 'A'; c <= 'Z'; c++)
            {
                value[c] = c - 'A';
            }
            value[' '] = 26;
            value['?'] = 27;
            value['!'] = 28;
        }

        /// <summary>
        /// Converts a character to its corresponding numeric value based on the dictionary.
        /// Throws an exception if the character is not in the dictionary.
        /// </summary>
        /// <param name="alphabet">The character to convert.</param>
        /// <returns>The numeric value of the character.</returns>
        static int ValueOfCharacter(char alphabet)
        {
            if (value.TryGetValue(alphabet, out int val))
            {
                return val;
            }
            throw new ArgumentException($"Character '{alphabet}' not in dictionary.");
        }

        /// <summary>
        /// Converts a numeric value back to its corresponding character.
        /// Handles special characters and ensures the value is within the valid range for the given modulus.
        /// </summary>
        /// <param name="value">The numeric value to convert.</param>
        /// <param name="mod">The modulus used for conversion.</param>
        /// <returns>The corresponding character.</returns>
        static char CharacterOfValue(int value, int mod)
        {
            value = (value % mod + mod) % mod; // Ensure value is within the valid range

            // Handle values that map to special characters
            if (mod == 29)
            {
                if (value == 26) return ' ';
                if (value == 27) return '?';
                if (value == 28) return '!';
            }

            // For letters A-Z
            return (char)(value + 'A');
        }

        /// <summary>
        /// Encrypts a message using the Hill cipher with the given matrix and modulus.
        /// Processes the input message in pairs of characters and applies matrix multiplication.
        /// </summary>
        /// <param name="input">The plaintext message to encrypt.</param>
        /// <param name="mod">The modulus to use for encryption.</param>
        /// <param name="key">The encryption matrix.</param>
        /// <returns>The encrypted message.</returns>
        static string Encrypt(string input, int mod, Matrix<double> key)
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
                encryptedChars.Add(CharacterOfValue((int)result[0, 0] % mod, mod));
                encryptedChars.Add(CharacterOfValue((int)result[1, 0] % mod, mod));
            }

            // Return the encrypted string
            return new string(encryptedChars.ToArray());
        }

        /// <summary>
        /// Decrypts a message using the Hill cipher with the given matrix and modulus.
        /// Processes the encrypted message in pairs of characters and applies the inverse matrix multiplication.
        /// </summary>
        /// <param name="input">The ciphertext message to decrypt.</param>
        /// <param name="mod">The modulus to use for decryption.</param>
        /// <param name="key">The decryption matrix (inverse of encryption matrix).</param>
        /// <returns>The decrypted message.</returns>
        static string Decrypt(string input, int mod, Matrix<double> key)
        {
            List<char> decryptedChars = new List<char>();
            Matrix<double> inverseMatrix = FindInverseMatrix(key, mod);
            for (int i = 0; i < input.Length; i += 2)
            {
                int num1 = ValueOfCharacter(input[i]);
                int num2 = ValueOfCharacter(input[i + 1]);

                // Create the column vector for the pair of characters
                Matrix<double> K = DenseMatrix.OfArray(new double[,]
                {
                    { num1 },
                    { num2 }
                });

                // Multiply the inverse matrix with the character vector
                Matrix<double> result = inverseMatrix.Multiply(K);

                // Convert the resulting numbers back to characters
                decryptedChars.Add(CharacterOfValue((int)result[0, 0] % mod, mod));
                decryptedChars.Add(CharacterOfValue((int)result[1, 0] % mod, mod));
            }
            return new string(decryptedChars.ToArray());
        }

        /// <summary>
        /// Finds the modular inverse of a matrix using the determinant and adjugate matrix.
        /// </summary>
        /// <param name="matrix">The matrix to invert.</param>
        /// <param name="mod">The modulus for the inversion.</param>
        /// <returns>The modular inverse of the matrix.</returns>
        static Matrix<double> FindInverseMatrix(Matrix<double> matrix, int mod)
        {
            double det = matrix.Determinant();
            int detMod = (int)(det % mod);
            detMod = FindModularInverse(detMod, mod);
            Matrix<double> adjugate = FindAdjugateMatrix(matrix, mod);

            Matrix<double> result = adjugate.Multiply(detMod);
            result = result.Map(x => (x % mod + mod) % mod);

            return result;
        }

        /// <summary>
        /// Computes the adjugate matrix of a given 2x2 matrix.
        /// The adjugate is used to find the inverse matrix.
        /// </summary>
        /// <param name="matrix">The matrix to compute the adjugate for.</param>
        /// <param name="mod">The modulus for the adjugate calculation.</param>
        /// <returns>The adjugate matrix.</returns>
        public static Matrix<double> FindAdjugateMatrix(Matrix<double> matrix, int mod)
        {
            Matrix<double> adjugate = DenseMatrix.OfArray(new double[2, 2]
            {
                { matrix[1, 1], -matrix[0, 1] },
                { -matrix[1, 0], matrix[0, 0] }
            });

            adjugate = adjugate.Map(x => (x % mod + mod) % mod);
            return adjugate;
        }

        /// <summary>
        /// Computes the modular inverse of an integer.
        /// Uses the Extended Euclidean Algorithm to find the inverse.
        /// </summary>
        /// <param name="a">The integer to find the inverse of.</param>
        /// <param name="mod">The modulus for the inversion.</param>
        /// <returns>The modular inverse of the integer.</returns>
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
