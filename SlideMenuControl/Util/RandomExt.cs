using System;

namespace SlideMenuControl.Util
{
    /// <summary>
    /// Create Random value
    /// </summary>
    public static class RandomExt
    {
        /// <summary>
        /// Get Random value between 2 value
        /// </summary>
        /// <param name="random">Random</param>
        /// <param name="min">Min Value</param>
        /// <param name="max">Max Value</param>
        /// <returns>Random value</returns>
        public static double GetRandom(this Random random, double min, double max)
        {
            return (random.NextDouble() * (max - min)) + min;
        }
    }
}