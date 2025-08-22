using System.Globalization;

namespace VitoriaAirlinesMAUI.Converters
{
    public class CountryCodeToFlagUrlConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var code = (value?.ToString() ?? string.Empty).Trim();
            if (code.Length < 2)
                return null;

            var cc = code[..2].ToLowerInvariant();

            var uri = new Uri($"https://flagcdn.com/w40/{cc}.png");

            return new UriImageSource
            {
                Uri = uri,
                CachingEnabled = true,
                CacheValidity = TimeSpan.FromDays(30)
            };
        }


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
