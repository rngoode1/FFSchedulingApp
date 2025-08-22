using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using FFSchedulingApp.Enums;
using FFSchedulingApp.Model;
using static FFSchedulingApp.Model.Match;

namespace FFSchedulingApp
{
    public class Program
    {
        private static readonly TeamList _teamList = new();
        private static readonly Schedule _schedule = new();
        public static void Main(string[] args)
        {
            Console.WriteLine("\nStarting Scheduling App\n");

            _schedule.AddWeek(_teamList.RivalryWeek());
            _teamList.ShuffleTeams();

            // Divisional Weeks
            Console.WriteLine("Building Divisional Weeks 2-6");
            for (int i = 2; i <= 6; i++)
            {
                Console.WriteLine($"\n---Building Divisional Week {i}---");
                _schedule.AddWeek(BuildWeek(i));
                _teamList.ShuffleTeams();
            }
            Console.WriteLine("\n" + _schedule);
        }

        private static Week BuildWeek(int weekNumber)
        {
            List<Match> matches = [];
            foreach (Team team in _teamList.Teams)
            {
                // compile already scheduled teams
                List<Team> scheduledTeams =
                [
                    .. matches.Select(m => m.HomeTeam).ToList(),
                    .. matches.Select(m => m.AwayTeam).ToList()

                ];

                // If team has no matches scheduled find a matchup
                if (!scheduledTeams.Where(st => st.Id == team.Id).Any())
                {
                    matches.Add(BuildMatchUp(team, scheduledTeams, weekNumber));
                }
                else
                {
                    Console.WriteLine($"HomeTeam {team} already has a matchup scheduled for this week\n");
                }                
            }
            return new Week(matches, weekNumber);            
        }

        private static Match BuildMatchUp(Team team, List<Team> scheduledTeams, int weekNumber)
        {
            Match newMatch = new();
            bool teamScheduled = false;
            List<Team> tempOppList = [.. team.PossibleOpponents];

            Console.WriteLine($"Building Matchup for HomeTeam: {team}");
            // Using a temp list, keep removing opps until you find one thats not scheduled, or if there are no opponents left
            while (!teamScheduled)
            {
                Team opponentTeam = tempOppList.FirstOrDefault() ?? new Team();
                // no opp left
                if (opponentTeam.IsNull())
                {
                    Console.WriteLine($"Opp List is null for Home Team {team}, no matchups can be made \n");
                    teamScheduled = true;
                }
                // check if the next opp is scheduled already
                else if (!scheduledTeams.Where(o => o.Id == opponentTeam.Id).Any())
                {
                    newMatch = new(team, opponentTeam, weekNumber);
                    teamScheduled = true;
                }
                // opp team has already been scheduled
                else
                {
                    Console.WriteLine($"AwayTeam {opponentTeam} already has a matchup scheduled for this week");
                    tempOppList.Remove(opponentTeam);
                }
            }
            return newMatch;
        }

        /// <summary>
        /// Finds next opponent based on matchtype
        /// </summary>
        /// <param name="team">Team whos opponent list to search</param>
        /// <param name="matchType">The matchtype of the opponent you would like to find</param>
        /// <returns>Next opponent of the appropriate matchtype, if there are none returns an empty team</returns>
        private static Team FindNextOpponent(Team team, MatchTypes matchType)
        {
            Divisions opponentDivision = matchType.Equals(MatchTypes.Divisional) ? team.Division : GetOppositeDivision(team.Division);
            Team opponentTeam = team.PossibleOpponents.FirstOrDefault(t => t.Division.Equals(team.Division)) ?? new Team();
            if (opponentTeam.IsNull())
            {
                Console.WriteLine($"Cannot find anymore {matchType} opponents for: {team}. Finding next {GetOppositeMatchType(matchType)} opponent");
            }
            return opponentTeam;
        }

        /// <summary>
        /// Finds the opposite division of the one provided
        /// </summary>
        /// <param name="division">Shirts, Skins or None</param>
        /// <returns>The opposite of Shirts or Skins, None as default if nothing is provided</returns>
        private static Divisions GetOppositeDivision(Divisions division)
        {
            Divisions oppDivision = Divisions.None;
            switch (division)
            {
                case Divisions.Shirts:
                    oppDivision = Divisions.Skins;
                    break;
                case Divisions.Skins:
                    oppDivision = Divisions.Shirts;
                    break;
                case Divisions.None:
                    break;
            }
            return oppDivision;
        }

        /// <summary>
        /// Returns the opposite match type 
        /// </summary>
        /// <param name="matchType">Matchtype to find the opposite of</param>
        /// <returns>Opposite match type</returns>
        private static MatchTypes GetOppositeMatchType(MatchTypes matchType)
        {
            MatchTypes oppType = MatchTypes.None;
            switch (matchType)
            {
                case MatchTypes.Divisional:
                    oppType = MatchTypes.CrossDivisional;
                    break;
                case MatchTypes.CrossDivisional:
                    oppType = MatchTypes.Divisional;
                    break;
            }
            return oppType;
        }    
    }
}