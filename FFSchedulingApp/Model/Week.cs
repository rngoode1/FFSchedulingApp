using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FFSchedulingApp.Model
{
    /// <summary>
    /// Creates a Week object that holds the 5 matchups for that week
    /// </summary>
    public class Week
    {
        public int WeekNumber { get; set; }
        public List<Match> Matches { get; set; } = [];

        /// <summary>
        /// Basic constructer for a Week object
        /// </summary>
        /// <returns>A Week object with an empty Match list and WeekNumber of 0</returns>
        public Week()
        {
            Matches = [];
            WeekNumber = 0;
        }

        /// <summary>
        /// Creates a Week object with the matches and week number provided
        /// </summary>
        /// <param name="matches">List of matches</param>
        /// <param name="weekNumber">Week of matchups</param>
        public Week(List<Match> matches, int weekNumber)
        {
            Matches = matches;
            WeekNumber = weekNumber;
        }

        public override string ToString()
        {
            string returnStr = $"--Week {WeekNumber}--\n";
            List<string> weekStr = [];
            string n = "";
            foreach (Match match in Matches)
            {
                weekStr.Add($"{n}{match.HomeTeam} vs. {match.AwayTeam}: {match.MatchType}");
                n = "\n";
            }
            string weekStrAll = string.Join("\t", weekStr);
            return string.Concat(returnStr, weekStrAll);
        }
    }
}