using System;

namespace OtherWorldBot.Utils
{
    public class TimeWordFormatter : IFormatProvider, ICustomFormatter
    {
        static readonly string[] hours = { "час", "часа", "часов" };
        static readonly string[] minutes = { "минуту", "минуты", "минут" };
        static readonly string[] seconds = { "секунду", "секунды", "секунд" };

        public object GetFormat(Type formatType)
        {
            return formatType == typeof(ICustomFormatter) ? this : null;
        }

        public string Format(string format, object arg, IFormatProvider formatProvider)
        {
            if (arg == null || !(arg is TimeSpan) || format != "W")
            {
                return string.Format("{0:" + format + "}", arg);
            }

            TimeSpan time = (TimeSpan)arg;

            string hh = GetCase(time.Hours, hours);
            string mm = GetCase(time.Minutes, minutes);
            string ss = GetCase(time.Seconds, seconds);

            if (time.Hours == 0 && time.Minutes == 0)
                return string.Format("{0:%s} {1}", time, ss);
            else if (time.Hours == 0)
                return string.Format("{0:%m} {1} {0:%s} {2}", time, mm, ss);
            else
                return string.Format("{0:%h} {1} {0:%m} {2} {0:%s} {3}", time, hh, mm, ss);
        }

        static string GetCase(int value, string[] options)
        {
            value = Math.Abs(value) % 100;

            if (value > 10 && value < 15)
            {
                return options[2];
            }

            value %= 10;
            if (value == 1)
            { 
                return options[0]; 
            }
            if (value > 1 && value < 5) 
            {
                return options[1]; 
            }
            return options[2];
        }
    }
}
