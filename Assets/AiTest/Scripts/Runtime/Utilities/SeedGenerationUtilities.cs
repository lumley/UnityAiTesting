using System;
using System.Security.Cryptography;

namespace Lumley.AiTest.Utilities
{
    /// <summary>
    /// Set of utilities for generating or combining seeds
    /// </summary>
    public static class SeedGenerationUtilities
    {
        /// <summary>
        /// Generates a base seed which can be combined with other seeds
        /// </summary>
        /// <returns><see cref="long"/> with the base seed generated</returns>
        public static long GenerateBaseSeed()
        {
            byte[] bytes = new byte[8];
            RandomNumberGenerator.Fill(bytes);
            return BitConverter.ToInt64(bytes, 0) & long.MaxValue;
        }
        
        /// <summary>
        /// Combines a base seed (can be obtained from <see cref="GenerateBaseSeed"/>) with a current amount of days to get a new seed that can be used in Unity (or <see cref="System.Random"/>)
        /// </summary>
        /// <param name="baseSeed"><see cref="long"/> with a base seed</param>
        /// <param name="daySinceEpoch"><see cref="long"/> amount of days since epoch</param>
        /// <returns><see cref="int"/> with the generated seed</returns>
        public static int GetSeedForDay(long baseSeed, long daySinceEpoch)
        {
            ulong combined = (ulong)baseSeed ^ ((ulong)daySinceEpoch * 0x9E3779B97F4A7C15L); // A large prime (golden ratio)
            long longSeed = (long)combined & long.MaxValue; // Ensure positive seed
            return (int)(longSeed ^ (longSeed >> 32));
        }
    }
}