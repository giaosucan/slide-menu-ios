namespace SlideMenuControl.Util
{
    /// <summary>
    /// Define the Error Status
    /// </summary>
    public class ErrorStatus
    {
        /// <summary>
        /// Error Status constructor
        /// </summary>
        public ErrorStatus()
        {
            IsSizeError = false;
            IsMarginError = false;
            IsTextLengthError = false;
            IsOutOfItemError = false;
        }

        /// <summary>
        /// Check Item Property Size Error
        /// </summary>
        public static bool IsSizeError { get; set; }

        /// <summary>
        /// Check Margin Property Error
        /// </summary>
        public static bool IsMarginError { get; set; }

        /// <summary>
        /// Check Title Text Length Error
        /// </summary>
        public static bool IsTextLengthError { get; set; }

        /// <summary>
        /// Check number of item exceed Error
        /// </summary>
        public static bool IsOutOfItemError { get; set; }
    }
}