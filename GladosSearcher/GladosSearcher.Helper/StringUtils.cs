using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace GladosSearcher.Helper
{
    public static class StringUtils
    {
        private const string regexPatternDate = @"^(?<dia>(?:[0-2]\d|3[0-1])|\d)(?:[\-\/\.]|(?:\s*[dD][eE]\s*)|\s)(?<mes>(?:0[1-9])|(?:1[012])|(?:[a-zA-Zç]+)|\d)(?:[\-\/\.]|(?:\s*[dD][eE](?:\s*|\\t))|\s)(?<ano>\d{4}|\d{2})(?:(?:(?:(?:\s[aà]s\s)|\s))(?<hora>(?:[0-1][0-9]|2[0-4])):(?<minuto>[0-5][0-9])(?::(?<segundo>[0-5][0-9]))?)?";
        private readonly static Regex _regexDate = new Regex(regexPatternDate, RegexOptions.Compiled);

        public static DateTime ConvertStringToDateTime(this string date) 
        {
            var dateMatch = _regexDate.Match(date);

            if (!dateMatch.Success)
                return new DateTime();

            return FormatDateByMatch(dateMatch);
        }

        private static DateTime FormatDateByMatch(Match date)
        {
            var day = TryConvertStringToInt(date.Groups["dia"].Value);
            var month = TryGetMonth(date.Groups["mes"].Value);
            var year = TryConvertStringToInt(date.Groups["ano"].Value);

            // Normalizes the year to 4 digits (yyyy)
            year = CultureInfo.CurrentCulture.Calendar.ToFourDigitYear(year);

            if (ContainsValidHoursAndMinute(date.Groups["hora"].Value, date.Groups["minuto"].Value)
                && date.Groups.Count > 4)
            {
                var hour = TryConvertStringToInt(date.Groups["hora"].Value);
                var minute = TryConvertStringToInt(date.Groups["minuto"].Value);

                string secondMatch = date.Groups["segundo"].Value;
                var second = !string.IsNullOrEmpty(secondMatch) ? TryConvertStringToInt(secondMatch) : 0;

                return new DateTime(year, month, day, hour, minute, second);
            }
            else
            {
                return new DateTime(year, month, day);
            }
        }

        public static bool ContainsValidHoursAndMinute(string hour, string minute)
        {
            return !string.IsNullOrEmpty(hour) &&
                !string.IsNullOrEmpty(minute);
        }

        public static int TryConvertStringToInt(string number)
        {
            if (int.TryParse(number, out int resultNumber))
                return resultNumber;

            throw new FormatException($"Can't convert {number} to int");
        }

        public static int TryGetMonth(string month)
        {
            if (int.TryParse(month, out int monthNumber))
            {
                return monthNumber;
            }
            else
            {
                return mapMonthNames[month.ToLower()];
            }
        }

        private static Dictionary<string, int> mapMonthNames =
            new Dictionary<string, int>
            {
                { "janeiro", 1 },
                { "jan", 1 },
                { "fevereiro", 2 },
                { "fev", 2 },
                { "março", 3 },
                { "mar", 3 },
                { "abril", 4 },
                { "apr", 4 },
                { "maio", 5 },
                { "mai", 5 },
                { "junho", 6 },
                { "jun", 6 },
                { "julho", 7 },
                { "jul", 7 },
                { "agosto", 8 },
                { "ago", 8 },
                { "setembro", 9 },
                { "set", 9 },
                { "outubro", 10 },
                { "out", 10 },
                { "novembro", 11 },
                { "nov", 11 },
                { "dezembro", 12 },
                { "dez", 12 }
            };
    }
}
