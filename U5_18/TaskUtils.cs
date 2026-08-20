using System;
using System.Collections.Generic;
using System.Linq;

namespace U5_18 {
    /// <summary>
    /// Provides utility methods for analyzing server activity and mail data.
    /// </summary>
    public class TaskUtils {
        /// <summary>
        /// Identifies hours during the day when a server was not processing any letters.
        /// </summary>
        /// <param name="servers">The list of servers containing letter data.</param>
        /// <param name="speeds">The list of server processing speeds.</param>
        /// <returns>A list of InactiveHourServer objects containing the inactive hours for each server.</returns>
        public static List<InactiveHourServer> FindInactiveHours(List<Server> servers, List<ServerSpeed> speeds) {
            return servers
                .Select(server => new {
                    Server = server,
                    SpeedInfo = speeds.FirstOrDefault(serv => serv.ServerName == server.ServerName)
                })
                .Select(data => new {
                    data.Server,
                    ActiveHours = Enumerable.Range(0, data.Server.GetLettersCount())
                        .Select(i => data.Server.GetLetter(i))
                        .SelectMany(letter => {
                            double durationSeconds = (double)letter.BiteSize / data.SpeedInfo.Speed;
                            int startH = letter.Time.Hours;
                            int endH = letter.Time.Add(TimeSpan.FromSeconds(durationSeconds)).Hours;
                            return Enumerable.Range(startH, Math.Min(endH, 23) - startH + 1);
                        })
                        .Distinct()
                        .ToList()
                })
                .Select(data => new {
                    data.Server,
                    InactiveHours = Enumerable.Range(0, 24)
                        .Where(h => !data.ActiveHours.Contains(h))
                        .ToList()
                })
                .Where(data => data.InactiveHours.Any())
                .Select(data => new InactiveHourServer(data.Server.ServerName, data.Server.Date, data.InactiveHours))
                .ToList();
        }

        /// <summary>
        /// Finds the minimum number of letters sent by any single sender on a specific date.
        /// </summary>
        /// <param name="servers">The list of servers to analyze.</param>
        /// <param name="targetDate">The specific date to filter data by.</param>
        /// <param name="minSenders">Output parameter: a list of sender addresses that sent the minimum amount of letters.</param>
        /// <returns>The minimum count of letters sent by the identified senders.</returns>
        public static int FindMinSendersByDate(List<Server> servers, DateTime targetDate, out List<string> minSenders) {
            var counts = servers
                .Where(s => s.Date.Date == targetDate.Date)
                .SelectMany(server => Enumerable.Range(0, server.GetLettersCount())
                    .Select(i => server.GetLetter(i).SenderAddress))
                .GroupBy(address => address)
                .ToDictionary(group => group.Key, group => group.Count());

            int minVal = counts.Values.Min();
            minSenders = counts.Where(kv => kv.Value == minVal)
                               .Select(kv => kv.Key)
                               .ToList();

            return minVal;
        }
    }
}