using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuietTime
{
    class QuietTimeHandler
    {
        private struct TimeOfDay : IComparable
        {
            public TimeOfDay(DateTime time)
            {
                Hour = time.Hour;
                Minute = time.Minute;
                Second = time.Second;
            }

            public TimeOfDay(int hour, int minute, int second)
            {
                Hour = hour;
                Minute = minute;
                Second = second;
            }

            public int Hour { get; private set; }
            public int Minute { get; private set; }
            public int Second { get; private set; }

            public int CompareTo(object obj)
            {
                if (obj == null || !(obj is TimeOfDay))
                    throw new ArgumentException("Object is not a TimeOfDay");

                if (ReferenceEquals(obj, this))
                    return 0;

                TimeOfDay other = (TimeOfDay)obj;

                if (Hour > other.Hour)
                    return 1;
                if (Hour < other.Hour)
                    return -1;
                if (Minute > other.Minute)
                    return 1;
                if (Minute < other.Minute)
                    return -1;
                if (Second > other.Second)
                    return 1;
                if (Second < other.Second)
                    return -1;

                return 0;
            }

            public static bool operator <(TimeOfDay lhs, TimeOfDay rhs) => lhs.CompareTo(rhs) < 0;
            public static bool operator >(TimeOfDay lhs, TimeOfDay rhs) => lhs.CompareTo(rhs) > 0;

            public static bool operator <=(TimeOfDay lhs, TimeOfDay rhs) => lhs.CompareTo(rhs) <= 0;
            public static bool operator >=(TimeOfDay lhs, TimeOfDay rhs) => lhs.CompareTo(rhs) >= 0;
        }

        private struct DateRange
        {
            public TimeOfDay Start;
            public TimeOfDay End;

            public bool Contains(TimeOfDay time)
            {
                return time >= Start && time <= End;
            }
        }

        public static bool IsQuietTime(DateTime time)
        {
            var quietTimes = QuietTimesForDay(time.DayOfWeek);

            var timeOfDay = new TimeOfDay(time);

            foreach (var quietTime in quietTimes)
            {
                if (quietTime.Contains(timeOfDay))
                    return true;
            }

            return false;
        }

        private static List<DateRange> QuietTimesForDay(DayOfWeek day)
        {
            var quietTimes = new List<DateRange>();

            switch (day)
            {
                case DayOfWeek.Monday:
                case DayOfWeek.Tuesday:
                case DayOfWeek.Wednesday:
                case DayOfWeek.Thursday:
                    quietTimes.Add(new DateRange { Start = new TimeOfDay(0, 0, 0), End = new TimeOfDay(6, 59, 59) });
                    quietTimes.Add(new DateRange { Start = new TimeOfDay(22, 0, 0), End = new TimeOfDay(23, 59, 59) });
                    break;
                case DayOfWeek.Friday:
                    quietTimes.Add(new DateRange { Start = new TimeOfDay(0, 0, 0), End = new TimeOfDay(6, 59, 59) });
                    quietTimes.Add(new DateRange { Start = new TimeOfDay(23, 0, 0), End = new TimeOfDay(23, 59, 59) });
                    break;
                case DayOfWeek.Saturday:
                    quietTimes.Add(new DateRange { Start = new TimeOfDay(0, 0, 0), End = new TimeOfDay(8, 59, 59) });
                    quietTimes.Add(new DateRange { Start = new TimeOfDay(23, 0, 0), End = new TimeOfDay(23, 59, 59) });
                    break;
                case DayOfWeek.Sunday:
                    quietTimes.Add(new DateRange { Start = new TimeOfDay(0, 0, 0), End = new TimeOfDay(8, 59, 59) });
                    quietTimes.Add(new DateRange { Start = new TimeOfDay(22, 0, 0), End = new TimeOfDay(23, 59, 59) });
                    break;
            }

            return quietTimes;
        }
    }
}
