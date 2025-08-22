using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FFSchedulingApp.Model
{
    /// <summary>
    /// An object that holds a list of weekly matchups
    /// </summary>
    public class Schedule
    {
        /// <summary>
        /// List of weeks in the schedule object
        /// </summary>
        public List<Week> Weeks { get; set; } = [];

        /// <summary>
        /// Adds a week object
        /// </summary>
        /// <param name="week">Populated Week Object</param>
        public void AddWeek(Week week)
        {
            Weeks.Add(week);
        }

        /// <summary>
        /// Retrieves a week
        /// </summary>
        /// <param name="weekNumber">The week that is associated with a Weeks weeknumber</param>
        /// <returns>A fully populated week object</returns>
        public Week GetWeek(int weekNumber)
        {
            Week week = Weeks.Find(w => w.WeekNumber == weekNumber) ?? new Week();
            if (week.WeekNumber == 0)
            {
                Console.WriteLine($"Week {weekNumber} not found");
            }
            return week;

        }

        public override string ToString()
        {
            return string.Join("\n", Weeks);
        }            
    }
}