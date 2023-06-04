using SuspiciousGames.SellMe.GameEvents;
using SuspiciousGames.SellMe.Utility;

namespace SuspiciousGames.SellMe.Core
{
    public static class GameTime
    {
        #region Constants
        public const int HoursPerDay = 24;
        public const int DaysPerMonth = 30;
        public const int MonthsPerYear = 12;
        public const int HoursPerMonth = DaysPerMonth * HoursPerDay;
        public const int HoursPerYear = DaysPerYear * HoursPerDay;
        public const int DaysPerYear = DaysPerMonth * MonthsPerYear;
        #endregion

        #region Private fields
        private static long _hours = 0;
        #endregion

        #region Properties
        public static long Hours { get => _hours; }
        public static long Days { get => _hours / HoursPerDay; }
        public static long Months { get => _hours / HoursPerMonth; }
        public static long Years { get => _hours / HoursPerYear; }
        public static string SinceStart { get => ConvertToTimeData(_hours); }
        public static long HoursTillNextRent { get => HoursPerMonth - _hours % HoursPerMonth; }
        public static string TimeTillNextRent { get => $"{HoursTillNextRent / HoursPerDay}D{HoursTillNextRent % HoursPerDay}H"; }

        #endregion

        #region Public methods
        /// <summary>
        /// Loads the value from the SaveGame
        /// </summary>
        public static void LoadFromSaveGame()
        {
            if (!SaveGame.LoadData(SaveId.GameTime, out string hours))
            {
                _hours = 0;
                SaveGame.SaveData(SaveId.GameTime, _hours);
            }
            else
            {
                _hours = long.Parse(hours);
            }
        }

        /// <summary>
        /// Adds time according to <paramref name="hours"/>, <paramref name="days"/>, <paramref name="months"/>, <paramref name="years"/> and saves it
        /// </summary>
        /// <param name="hours">Hours to add</param>
        /// <param name="days">Days to add</param>
        /// <param name="months">Months to add</param>
        /// <param name="years">Years to add</param>
        public static void AddTime(int hours = 0, int days = 0, int months = 0, int years = 0)
        {
            if (days < 0)
                days = 0;
            if (hours < 0)
                hours = 0;
            if (months < 0)
                months = 0;
            if (years < 0)
                years = 0;
            if (hours == 0 && days == 0 && months == 0 && years == 0)
                return;
            long monthsBeforeTimeAdd = _hours / HoursPerMonth;
            long hoursToAdd = hours + days * HoursPerDay + months * HoursPerMonth + years * HoursPerYear;
            _hours += hoursToAdd;
            SaveGame.SaveData(SaveId.GameTime, _hours);
            EventManager.TriggerEvent(GameEventID.TimePassed, hoursToAdd, Months > monthsBeforeTimeAdd);
        }

        /// <summary>
        /// Returns a string formated like 0y 0m 0d 0h, 0 values are not printed with the only excpetion being 0y 0m 0d 0h
        /// </summary>
        /// <returns></returns>
        public static string ConvertToTimeData(long hours)
        {
            if (hours == 0)
                return "0Y 0M 0D 0H";
            long years = hours / HoursPerYear;
            long months = hours / HoursPerMonth;
            long days = hours / HoursPerDay;
            long tHours = hours % HoursPerDay;
            string timeString = "";
            if (years != 0)
                timeString += years + "Y ";
            if (months != 0)
                timeString += months + "M ";
            if (days != 0)
                timeString += days + "D ";
            if (tHours != 0)
                timeString += tHours + "H";
            return timeString.TrimEnd(' ');
        }

        /// <summary>
        /// Returns a string formated like 0y 0m 0d 0h, 0 values are not printed with the only excpetion being 0y 0m 0d 0h
        /// </summary>
        /// <returns></returns>
        public static string ConvertToTimeData(int hours)
        {
            if (hours == 0)
                return "0y 0m 0d 0h";
            int years = hours / HoursPerYear;
            int months = hours / HoursPerMonth;
            int days = hours / hours / HoursPerDay;
            int tHours = hours % HoursPerDay;
            string timeString = "";
            if (years != 0)
                timeString += years + "y ";
            if (months != 0)
                timeString += months + "m ";
            if (days != 0)
                timeString += days + "d ";
            if (tHours != 0)
                timeString += tHours + "h";
            return timeString.TrimEnd(' ');
        }

        /// <summary>
        /// Resets the hour value to 0 and saves it
        /// </summary>
        public static void Reset()
        {
            _hours = 0;
            SaveGame.SaveData(SaveId.GameTime, _hours);
        }
        #endregion
    }
}