using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace U5_18 {
    /// <summary>
    /// Represents the results of an inactivity analysis for a specific server on a specific date.
    /// </summary>
    public class InactiveHourServer {
        public string ServerName { get; set; }
        public DateTime Date { get; set; }
        private List<int> Hours;

        /// <summary>
        /// Initializes a new instance of the InactiveHourServer class.
        /// </summary>
        /// <param name="serverName">The name of the analyzed server.</param>
        /// <param name="date">The date of the analysis.</param>
        /// <param name="hours">A list of integers representing the hours (0-23) when the server was inactive.</param>
        public InactiveHourServer(string serverName, DateTime date, List<int> hours) {
            ServerName = serverName;
            Date = date;
            Hours = new List<int>();
            for (int i = 0; i < hours.Count; i++) {
                Hours.Add(hours[i]);
            }
        }

        /// <summary>
        /// Gets a specific inactive hour from the collection by its index.
        /// </summary>
        /// <param name="index">The zero-based index of the hour.</param>
        /// <returns>An integer representing the hour of the day.</returns>
        public int GetHour(int index) {
            return Hours[index];
        }

        /// <summary>
        /// Gets the total number of inactive hours recorded for this server and date.
        /// </summary>
        /// <returns>The count of inactive hours.</returns>
        public int GetHoursCount() {
            return Hours.Count;
        }

        /// <summary>
        /// Formats the inactivity data into a readable string for file output or display.
        /// </summary>
        /// <returns>A string containing the server name, date, and a comma-separated list of inactive hours.</returns>
        public override string ToString() {
            string hoursStr = string.Join(", ", Enumerable.Range(0, GetHoursCount())
                                                   .Select(i => GetHour(i) + ":00"));
            return string.Format("Serveris: {0,-15} Data: {1.ToShortDateString(),-10} Valandos: {2}",
                ServerName, Date, hoursStr);
        }
    }
}