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
            // Console.WriteLine($"*Week {MatchWeek} Match Made: {this}*");
        }

        public bool IsNull()
        {
            return HomeTeam.Id == 0 || AwayTeam.Id == 0 || MatchWeek == 0 || MatchType.Equals(MatchTypes.None);
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