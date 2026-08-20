using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace U5_18 {
    /// <summary>
    /// Represents the data for a server's processing speed.
    /// </summary>
    public class ServerSpeed {
        public string ServerName { get; set; }
        public int Speed { get; set; }

        /// <summary>
        /// Initializes a new instance of the ServerSpeed class.
        /// </summary>
        /// <param name="serverName">The name of the server.</param>
        /// <param name="speed">The speed of the server in bytes per second.</param>
        public ServerSpeed(string serverName, int speed) {
            ServerName = serverName;
            Speed = speed;
        }

        /// <summary>
        /// Formats the server speed data into a table row string.
        /// </summary>
        /// <returns>A formatted string containing the server name and speed.</returns>
        public override string ToString() {
            return string.Format("| {0,-20} | {1,17} |", ServerName, Speed);
        }
    }
}