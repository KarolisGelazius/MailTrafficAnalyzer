using System;
using System.Collections.Generic;
using System.Linq;

namespace U5_18 {
    /// <summary>
    /// Represents a server entity that stores mail processing data for a specific date.
    /// </summary>
    public class Server : IComparable<Server> {
        public DateTime Date { get; set; }
        public string ServerName { get; set; }
        private List<Letter> AllLetters;

        /// <summary>
        /// Server class constructor.
        /// </summary>
        /// <param name="date">The date of the operations.</param>
        /// <param name="serverName">The name of the server.</param>
        /// <param name="allLetters">A list of letters to be associated with this server.</param>
        public Server(DateTime date, string serverName, List<Letter> allLetters) {
            Date = date;
            ServerName = serverName;
            AllLetters = new List<Letter>();
            for (int i = 0; i < allLetters.Count; i++) {
                AllLetters.Add(allLetters[i]);
            }
        }

        /// <summary>
        /// Retrieves a specific letter from the server's records by its index.
        /// </summary>
        /// <param name="index">The zero-based index of the letter to retrieve.</param>
        /// <returns>The Letter object at the specified index.</returns>
        public Letter GetLetter(int index) {
            return AllLetters[index];
        }

        /// <summary>
        /// Gets the total number of letters processed by this server.
        /// </summary>
        /// <returns>The count of letters in the internal list.</returns>
        public int GetLettersCount() {
            return AllLetters.Count;
        }

        /// <summary>
        /// Compares this server instance to another.
        /// Sorts by name alphabetically, then by date chronologically.
        /// </summary>
        public int CompareTo(Server other) {
            if (other == null) return 1;

            // Sort by Name
            int nameCompare = string.Compare(this.ServerName, other.ServerName, StringComparison.OrdinalIgnoreCase);
            if (nameCompare != 0) return nameCompare;

            // Sort by Date if names are equal
            return this.Date.CompareTo(other.Date);
        }

        /// <summary>
        /// Returns a formatted string containing the server's name and the operation date.
        /// </summary>
        /// <returns>A string representing the server identification and date.</returns>
        public override string ToString() {
            return string.Format("Serveris: {0} ({1:yyyy-MM-dd})", ServerName, Date);
        }
    }
}