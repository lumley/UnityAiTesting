using System;

namespace Lumley.AiTest.Utilities
{
    public static class DateTimeUtilities
    {
        /// <summary>
        /// Gets the total amount of time passed from this moment since <see cref="DateTime.UnixEpoch"/>
        /// </summary>
        /// <returns><see cref="TimeSpan"/> with the amount of time passed</returns>
        public static TimeSpan GetCurrentTimeSinceEpoch()
        {
            return GetTimeSinceEpoch(DateTime.UtcNow);
        }

        /// <summary>
        /// Gets the total amount of time passed from a given point in time since <see cref="DateTime.UnixEpoch"/>
        /// </summary>
        /// <param name="dateTime"><see cref="DateTime"/> of reference</param>
        /// <returns><see cref="TimeSpan"/> with the amount of time passed</returns>
        public static TimeSpan GetTimeSinceEpoch(DateTime dateTime)
        {
            return dateTime - DateTime.UnixEpoch;
        }
    }
}