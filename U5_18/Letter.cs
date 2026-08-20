using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace U5_18 {
    /// <summary>
    /// Represents a single email letter entity with its metadata.
    /// </summary>
    public class Letter {
        public TimeSpan Time {  get; set; }
        public string SenderAddress { get; set; }
        public string RecieverAddress { get; set; }
        public int BiteSize { get; set; }

        /// <summary>
        /// Letter class constructor.
        /// </summary>
        /// <param name="time">The timestamp of the letter.</param>
        /// <param name="senderAddress">The sender's email address.</param>
        /// <param name="recieverAddress">The recipient's email address.</param>
        /// <param name="biteSize">The size of the letter in bytes.</param>
        public Letter(TimeSpan time, string senderAddress, string recieverAddress, int biteSize) {
            Time = time;
            SenderAddress = senderAddress;
            RecieverAddress = recieverAddress;
            BiteSize = biteSize;
        }

        /// <summary>
        /// Converts the letter's data into a formatted table row string.
        /// </summary>
        /// <returns>A string containing the timestamp, sender address, receiver address, and size in bytes.</returns>
        public override string ToString() {
            return string.Format("| {0,-12} | {1,-25} | {2,-25} | {3,10} |",
                Time.ToString(@"hh\:mm\:ss"), SenderAddress, RecieverAddress, BiteSize);
        }
    }
}