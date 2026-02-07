using System;
using System.Globalization;
using System.Windows.Data;

namespace Snack.Converters
{
    public class CellPositionConverter : IMultiValueConverter
    {
        // values[0] = Row or Column (int)
        // values[1] = CellSize (int)
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length < 2)
                return 0d; // double

            if (values[0] is int index && values[1] is int cellSize)
            {
                return (double)(index * cellSize);
            }

            // trường hợp binding ra kiểu string, ta cố parse
            if (int.TryParse(values[0]?.ToString(), out int idx) &&
                int.TryParse(values[1]?.ToString(), out int size))
            {
                return (double)(idx * size);
            }

            return 0d;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}