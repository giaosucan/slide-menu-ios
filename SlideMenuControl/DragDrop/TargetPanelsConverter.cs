using System;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;

namespace SlideMenuControl.DragDrop
{
    /// <summary>
    /// Target Stack Panel
    /// </summary>
    internal class TargetPanelsConverter : IMultiValueConverter
    {
        /// <summary>
        /// Get the stack panel of the Slide Menu
        /// </summary>
        /// <param name="values">Stack Panel</param>
        /// <param name="targetType">Target type</param>
        /// <param name="parameter">Parameters</param>
        /// <param name="culture">Culture Info</param>
        /// <returns>Object</returns>
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return values
                .Where(value => value is Panel)
                .Cast<Panel>()
                .ToList();
        }

        /// <summary>
        /// Convert Back
        /// </summary>
        /// <param name="value">Object value</param>
        /// <param name="targetTypes">Target Type</param>
        /// <param name="parameter">Parameter</param>
        /// <param name="culture">Style Format</param>
        /// <returns>Array of Object</returns>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}