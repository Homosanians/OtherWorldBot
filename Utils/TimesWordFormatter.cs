using System;

namespace OtherWorldBot.Utils
{
    public class TimesWordFormatter : IFormatProvider, ICustomFormatter
    {
        static readonly string[] timesWords = { "раз", "раза" };

        public object GetFormat(Type formatType)
        {
            return formatType == typeof(ICustomFormatter) ? this : null;
        }

        public string Format(string format, object arg, IFormatProvider formatProvider)
        {
            if (arg == null)
                throw new ArgumentNullException("Argument arg was null.");

            string timesWord;

            uint times = (uint)Math.Abs((int) arg);

            uint oneDigitEnding = times % 10;
            uint twoDigitsEnding = times % 100;

            if (twoDigitsEnding == 12 || twoDigitsEnding == 13 || twoDigitsEnding == 14)
                timesWord = timesWords[0];
            else if (oneDigitEnding == 2 || oneDigitEnding == 3 || oneDigitEnding == 4)
                timesWord = timesWords[1];
            else
                timesWord = timesWords[0];

            return string.Format("{0} {1}", times, timesWord);
        }
    }
}
