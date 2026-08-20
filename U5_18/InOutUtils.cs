using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace U5_18 {
    /// <summary>
    /// Static utility class for handling input and output operations
    /// </summary>
    public class InOutUtils {
        /// <summary>
        /// Reads server details and its associated letters from a data stream.
        /// </summary>
        /// <param name="inputStream">The stream containing server data.</param>
        /// <returns>A Server object populated with data from the stream.</returns>
        public static Server ReadServerFromStream(Stream inputStream) {
            using (StreamReader reader = new StreamReader(inputStream)) {
                string firstLine = reader.ReadLine();

                string[] header = firstLine.Split(new string[] { "; " }, StringSplitOptions.None);
                DateTime date = DateTime.Parse(header[0]);
                string serverName = header[1];

                List<Letter> tempLetters = new List<Letter>();

                string line;
                while ((line = reader.ReadLine()) != null) {
                    string[] values = line.Split(new string[] { "; " }, StringSplitOptions.RemoveEmptyEntries);
                    tempLetters.Add(new Letter(TimeSpan.Parse(values[0]), values[1], values[2], int.Parse(values[3])));
                }

                return new Server(date, serverName, tempLetters);
            }
        }

        /// <summary>
        /// Reads a list of server processing speeds from a data stream.
        /// </summary>
        /// <param name="inputStream">The stream containing speed data.</param>
        /// <returns>A list of ServerSpeed objects.</returns>
        public static List<ServerSpeed> ReadSpeedsFromStream(Stream inputStream) {
            List<ServerSpeed> speeds = new List<ServerSpeed>();
            using (StreamReader reader = new StreamReader(inputStream)) {
                string line;
                while ((line = reader.ReadLine()) != null) {
                    string[] values = line.Split(new string[] { "; " }, StringSplitOptions.None);
                    speeds.Add(new ServerSpeed(values[0], int.Parse(values[1])));
                }
            }
            return speeds;
        }

        /// <summary>
        /// Prints the initial data of servers and their speeds to a specified file.
        /// </summary>
        /// <param name="filePath">The path to the output file.</param>
        /// <param name="servers">The list of servers to print.</param>
        /// <param name="speeds">The list of server speeds to print.</param>
        public static void PrintStartingData(string filePath, List<Server> servers, List<ServerSpeed> speeds) {
            using (StreamWriter writer = new StreamWriter(filePath, false, Encoding.UTF8)) {
                writer.WriteLine("\nSERVERIŲ GREIČIAI:");
                writer.WriteLine(new string('-', 44));
                writer.WriteLine("| {0,-20} | {1,-17} |", "Serveris", "Greitis (B/s)");
                writer.WriteLine(new string('-', 44));

                speeds.ForEach(speed => writer.WriteLine(speed.ToString()));
                writer.WriteLine(new string('-', 44));

                writer.WriteLine("\nSERVERIŲ LAIŠKAI:");

                servers.ForEach(server =>
                {
                    writer.WriteLine("\n" + server.ToString());
                    writer.WriteLine(new string('-', 85));
                    writer.WriteLine("| {0,-12} | {1,-25} | {2,-25} | {3,10} |", "Laikas", "Siuntėjas", "Gavėjas", "Dydis (B)");
                    writer.WriteLine(new string('-', 85));

                    Enumerable.Range(0, server.GetLettersCount())
                        .Select(i => server.GetLetter(i))
                        .ToList()
                        .ForEach(letter => writer.WriteLine(letter.ToString()));

                    writer.WriteLine(new string('-', 85));
                });
            }
        }

        /// <summary>
        /// Appends a titled section of result lines to a specified text file.
        /// </summary>
        /// <param name="filePath">The full path of the file where results will be saved.</param>
        /// <param name="title">The heading text to be printed above the results.</param>
        /// <param name="lines">A list of strings representing the data rows to be written.</param>
        public static void PrintResults(string filePath, string title, List<string> lines) {
            using (StreamWriter writer = new StreamWriter(filePath, true, Encoding.UTF8)) {
                writer.WriteLine("\n" + title);
                writer.WriteLine(new string('=', title.Length));

                lines.ForEach(line => writer.WriteLine(line));
            }
        }
    }
}