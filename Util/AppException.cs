
using System.Globalization;
using MoviePlatformApi.Models;

namespace MoviePlatformApi.Util
{
    public class AppException : Exception
    {
        public AppException() : base() { }

        public AppException(string message) : base(message) { }

        public AppException(Error message) : base(message.ErrorMsg) { }

        public AppException(string message, params object[] args)
            : base(String.Format(CultureInfo.CurrentCulture, message, args))
        {
        }
    }
}