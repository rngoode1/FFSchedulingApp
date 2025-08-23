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
            _teamList.SetCrossOpponents();

            // Divisional Weeks
            Console.WriteLine("\nBuilding Divisional Weeks 2-6");
            for (int i = 2; i <= 6; i++)
            {
                Console.WriteLine($"\n---Building Divisional Week {i}---");
                _teamList.OrderByPossibleDivOppDesc();
                Week iWeek = new();
                int count = 0;
                var stop = false;
                while (iWeek.Matches.Where(m => m.HomeTeam.Id != 0).Count() < 5 && !stop)
                {
                    if (count > 100)
                    {
                        stop = true;
                    }
                    if (iWeek.Matches.Count > 0)
                    {
                        Console.WriteLine($"Week {i} failed, shuffling teams and retry");
                        _teamList.ShuffleTeams();
                    }
                    iWeek.RollBack();
                    iWeek = BuildWeek(i, MatchTypes.Divisional);
                    count++;
                }
                _schedule.AddWeek(iWeek);
                _teamList.ShuffleTeams();                
            }

            // Cross
            Console.WriteLine("\nBuilding CrossDivisional Weeks 7-9");
            for (int i = 7; i <= 9; i++)
            {
                Console.WriteLine($"\n---Building CrossDivisional Week {i}---");
                _teamList.OrderByPossibleDivOppDesc();
                Week iWeek = new();
                int count = 0;
                var stop = false;
                while (iWeek.Matches.Where(m => m.HomeTeam.Id != 0).Count() < 5 && !stop)
                {
                    if (count > 100)
                    {
                        stop = true;
                    }
                    if (iWeek.Matches.Count > 0)
                    {
                        // Console.WriteLine($"Week {i} failed, shuffling teams and retry");
                        _teamList.ShuffleTeams();
                    }
                    iWeek.RollBack();
                    iWeek = BuildWeek(i, MatchTypes.CrossDivisional);
                    count++;
                }
                _schedule.AddWeek(iWeek);
                _teamList.ShuffleTeams();                
            }

            // Divisional Weeks 2
            _teamList.SetCrossOpponents();
            Console.WriteLine("\nBuilding Divisional Weeks 10-14");
            for (int i = 10; i <= 14; i++)
            {
                Console.WriteLine($"\n---Building Divisional Week {i}---");
                _teamList.OrderByPossibleDivOppDesc2();
                Week iWeek = new();
                int count = 0;
                var stop = false;
                while (iWeek.Matches.Where(m => m.HomeTeam.Id != 0).Count() < 5 && !stop)
                {
                    if (count > 100)
                    {
                        stop = true;
                    }
                    if (iWeek.Matches.Count > 0)
                    {
                        // Console.WriteLine($"Week {i} failed, shuffling teams and retry");
                        _teamList.ShuffleTeams();
                    }
                    iWeek.RollBack();
                    iWeek = BuildWeek(i, MatchTypes.Divisional);
                    count++;
                }
                _schedule.AddWeek(iWeek);
                _teamList.ShuffleTeams();                
            }                        
            Console.WriteLine("\n" + _schedule + "\n");
            _teamList.CheckCounts();
        }

        private static Week BuildWeek(int weekNumber, MatchTypes matchTypes)
        {
            List<Match> matches = [];
            bool hasCrossMatch = false;
            foreach (Team team in _teamList.Teams)
            {
                // compile already scheduled teams
                List<Team> scheduledTeams =
                [
                    .. matches.Select(m => m.HomeTeam).ToList(),
                    .. matches.Select(m => m.AwayTeam).ToList()
                ];

                if (!scheduledTeams.Where(st => st.Equals(team)).Any())
                {
                    if (matchTypes.Equals(MatchTypes.Divisional))
                    {
                        // Check if neither week or team has had its cross match
                        if (team.CrossDivisionalOppId >= 1 && !hasCrossMatch)
                        {
                            // If team has no cross div matches scheduled find the cross div matchup
                            Match crossDivMatch = BuildCrossDivisionalMatchUp(team, scheduledTeams, weekNumber);
                            matches.Add(crossDivMatch);
                            hasCrossMatch = true;
                        }
                        // if team has already had their cross match or if cross match is scheduled, make div games
                        else if (team.CrossDivisionalOppId == 0 || hasCrossMatch)
                        {
                            Match divMatch = BuildDivisionalMatchUp(team, scheduledTeams, weekNumber);
                            matches.Add(divMatch);
                        }
                        else
                        {
                            Match crossDivMatch = BuildCrossDivisionalMatchUp(team, scheduledTeams, weekNumber);
                            matches.Add(crossDivMatch);
                            // Console.WriteLine($"Something has gone wrong checking crossoppid: {team.CrossDivisionalOppId}");
                        }
                    }
                    else if (matchTypes.Equals(MatchTypes.CrossDivisional))
                    {
                        Match crossDivMatch = BuildCrossAllMatchup(team, scheduledTeams, weekNumber);
                        matches.Add(crossDivMatch);
                        // Console.WriteLine($"Shouldnt have cross match here {team}");
                    }
                    else
                    {
                        Console.WriteLine($"Something has gone wrong checking matchtype: {matchTypes} {team}");
                    }
                }
                else
                {
                    // Console.WriteLine($"HomeTeam {team} already has a matchup scheduled for this week\n");
                }
            }            
            return new Week(matches, weekNumber);            
        }

        private static Match BuildDivisionalMatchUp(Team team, List<Team> scheduledTeams, int weekNumber)
        {

            List<Team> tempOppList;
            if (weekNumber < 9)
            {
                tempOppList = [
                    .. team.PossibleOpponents
                    .GroupBy(op => op.Id)
                    .Where(g => g.Count() > 1)
                    .SelectMany(g => g)
                    .Where(op => op.Division == team.Division)
                    .DistinctBy(t => t.Id)
                ];
            }
            else
            {
                tempOppList = [.. team.PossibleOpponents.Where(op => op.Division.Equals(team.Division))];
            }
            // Console.WriteLine($"{tempOppList.Count} Div Opps Found, Building Divisional Matchup for HomeTeam: {team}");
                return BuildMatchUp(team, tempOppList, scheduledTeams, weekNumber);
        }

        private static Match BuildCrossDivisionalMatchUp(Team team, List<Team> scheduledTeams, int weekNumber)
        {
            Team crossTeam = _teamList.GetTeam(team.CrossDivisionalOppId);
            List<Team> tempOppList = [ crossTeam ];            
            // Console.WriteLine($"{tempOppList.Count} CrossDiv Opps Found ({crossTeam}), Building CrossDivisional Matchup for HomeTeam: {team}");
            Match crossDivMatch = BuildMatchUp(team, tempOppList, scheduledTeams, weekNumber);
            // set cross id to 0 to flag its had its cross match
            crossDivMatch.HomeTeam.CrossDivisionalOppId = 0;
            crossDivMatch.AwayTeam.CrossDivisionalOppId = 0;
            return crossDivMatch;
        }

        private static Match BuildCrossAllMatchup(Team team, List<Team> scheduledTeams, int weekNumber)
        {
            List<Team> tempOppList = [.. team.PossibleOpponents.Where(op => team.Division.Equals(GetOppositeDivision(op.Division)))];
            Match crossDivMatch = BuildMatchUp(team, tempOppList, scheduledTeams, weekNumber);
            // set cross id to 0 to flag its had its cross match
            crossDivMatch.HomeTeam.CrossDivisionalOppId = 0;
            crossDivMatch.AwayTeam.CrossDivisionalOppId = 0;
            return crossDivMatch;
        }

        private static Match BuildMatchUp(Team team, List<Team> tempOppList, List<Team> scheduledTeams, int weekNumber)
        {
            Match newMatch = new();
            bool teamScheduled = false;
            while (!teamScheduled)
            {
                Team opponentTeam = tempOppList.FirstOrDefault() ?? new Team();
                // no opp left
                if (opponentTeam.IsNull())
                {
                    // Console.WriteLine($"Opp List is null for Home Team {team}, no matchups can be made \n");
                    teamScheduled = true;
                }
                // check if the next opp is scheduled already
                else if (!scheduledTeams.Where(o => o.Equals(opponentTeam)).Any())
                {
                    // Console.WriteLine($"Opponent Found: {opponentTeam}");
                    newMatch = new(team, opponentTeam, weekNumber);
                    // remove opps from main opp list on each team
                    // Console.WriteLine($"New possible opponents: {team}: {team.PossibleOpponents.Count}, {opponentTeam}: {opponentTeam.PossibleOpponents.Count}");
                    team.PossibleOpponents.Remove(opponentTeam);
                    team.UpdateMatchTypes(newMatch);

                    opponentTeam.PossibleOpponents.Remove(team);
                    opponentTeam.UpdateMatchTypes(newMatch);
                    // Console.WriteLine($"New possible opponents: {team}: {team.PossibleOpponents.Count}, {opponentTeam}: {opponentTeam.PossibleOpponents.Count} \n");

                    teamScheduled = true;
                }
                // opp team has already been scheduled
                else
                {
                    // Console.WriteLine($"AwayTeam {opponentTeam} already has a matchup scheduled for this week");
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
        private static Team FindNextOpponent(Team team, MatchTypes matchType, List<Team> tempOppList)
        {
            Divisions opponentDivision = matchType.Equals(MatchTypes.Divisional) ? team.Division : GetOppositeDivision(team.Division);
            Team opponentTeam = tempOppList.FirstOrDefault(po => po.Division.Equals(team.Division)) ?? new Team();
            if (opponentTeam.IsNull())
            {
                // Console.WriteLine($"Cannot find anymore {matchType} opponents for: {team}");
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