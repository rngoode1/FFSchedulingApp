using System.Runtime.CompilerServices;
using FFSchedulingApp.Enums;


namespace FFSchedulingApp.Model
{
    /// <summary>
    /// An Object that holds a matchup of two teams and its associated week
    /// </summary>
    public class Match
    {

        /// <summary>
        /// Used only to help facilitate scheduling of Divisional Games
        /// </summary>
        public Team HomeTeam { get; set; }

        /// <summary>
        /// Used only to help facilitate scheduling of Divisional Games
        /// </summary>    
        public Team AwayTeam { get; set; }

        /// <summary>
        /// The week this match takes place
        /// </summary>
        public int MatchWeek { get; set; }

        /// <summary>
        /// Whether the matchup is divisional or not
        /// </summary>
        public MatchTypes MatchType { get; set; }

        public Match()
        {
            HomeTeam = new Team();
            AwayTeam = new Team();
            MatchWeek = 0;
            MatchType = MatchTypes.None;
        }

        public Match(Team team1, Team team2, int matchWeek)
        {
            HomeTeam = team1;
            AwayTeam = team2;
            MatchWeek = matchWeek;
            MatchType = team1.Division == team2.Division ? MatchTypes.Divisional : MatchTypes.CrossDivisional;
            Console.WriteLine($"*Week {MatchWeek} Match Made: {this}*");
            UpdateTeamProperties();
        }

        private void UpdateTeamProperties()
        {            
            HomeTeam.UpdateMatchTypes(this);        
            AwayTeam.UpdateMatchTypes(this);

            // remove available opps from main opp list on each team
            Console.WriteLine($"Original possible opponents for {HomeTeam}: " + HomeTeam.PossibleOpponents.Count);
            HomeTeam.PossibleOpponents.Remove(AwayTeam);
            Console.WriteLine($"New possible opponents for {HomeTeam}: " + HomeTeam.PossibleOpponents.Count);

            Console.WriteLine($"Original possible opponents for {AwayTeam}: " + AwayTeam.PossibleOpponents.Count);
            AwayTeam.PossibleOpponents.Remove(HomeTeam);
            Console.WriteLine($"New possible opponents for {AwayTeam}: " + AwayTeam.PossibleOpponents.Count + "\n");
        }

        public override string ToString()
        {
            return $"{HomeTeam} vs. {AwayTeam}: {MatchType}";
        }

        public enum MatchTypes
        {
            CrossDivisional,
            Divisional,
            None
        }
    }


}