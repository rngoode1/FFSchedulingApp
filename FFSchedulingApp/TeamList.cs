using FFSchedulingApp.Enums;
using FFSchedulingApp.Model;

namespace FFSchedulingApp
{
    /// <summary>
    /// Creates the object TeamList whos property is the list of teams provided in TeamInfo.cs
    /// </summary>
    public class TeamList
    {
        public List<Team> Teams { get; set; } = [];

        public TeamList()
        {
            Teams.AddRange(Enumerable.Range(1, 10).Select(i => new Team(i)));
            // Adds the opponent list to each team and randomizes it
            Teams.ForEach(team =>
                {
                    List<Team> unorderedTeams =
                    [
                        // Add every team except it self to possible opponents
                        .. Teams.Where(t => t.Id != team.Id),
                        // team.PossibleOpponents.AddRange(Teams.Where(t => t.Id != team.Id));                    

                        // Add every divisional opponent again
                        .. Teams.Where(t => t.Division.Equals(team.Division) && t.Id != team.Id),
                    ];

                    team.PossibleOpponents.AddRange(unorderedTeams.OrderBy(_ => Guid.NewGuid()));
                }
            );
        }

        /// <summary>
        /// Retrives a <c>Team</c> object from the <c>TeamList</c>
        /// </summary>
        /// <param name="id">Team Id</param>
        /// <returns >A <c>Team</c> object</returns>
        public Team GetTeam(int id)
        {
            Team team = Teams.Find(i => i.Id == id) ?? new Team();
            if (team.Id == 0)
            {
                Console.WriteLine($"Team {id} not found");
            }
            return team;
        }

        /// <summary>
        /// Creates a <c>Week</c> object that is always the first match of the season
        /// </summary>
        /// <returns>A <c>Week</c> object with a list of <c>Matches</c> based on last years placement (1v2, 3v4, ..)</returns>
        public Week RivalryWeek()
        {
            Console.WriteLine("---Building Rivalry Week (1) Matchups---");
            Week rivalryWeek = new([.. Enumerable.Range(1, 5)
                .Select(i =>
                    new Match(
                        Teams.Find(t => t.Id == (i*2) - 1) ?? new Team(),
                        Teams.Find(t => t.Id == i*2) ?? new Team(),
                        1
                    )
                )],
                1
            );            
            Teams.ForEach(t => t.CrossDivisionalMatches++);
            // rivalryWeek.Matches.ForEach(m =>
            // {
            //     m.HomeTeam.PossibleOpponents.Remove(m.AwayTeam);
            //     m.AwayTeam.PossibleOpponents.Remove(m.HomeTeam);
            // });
            ToString();
            Console.WriteLine("\n");
            return rivalryWeek;
        }

        public void SetCrossOpponents()
        {
            List<Team> skins =
            [
                .. Teams.Where(t => t.Division.Equals(Divisions.Skins))
            ];
            var error = false;
            foreach (Team shirtTeam in Teams.Where(t => t.Division.Equals(Divisions.Shirts)))
            {
                if (shirtTeam.TotalMatches() < 6)
                {
                    Team skinsTeam = skins.First(s => s.Id != shirtTeam.Id - 1) ?? new Team();
                    if (skinsTeam.IsNull()) error = true;
                    shirtTeam.CrossDivisionalOppId = skinsTeam.Id;
                    skinsTeam.CrossDivisionalOppId = shirtTeam.Id;
                    skins.Remove(skinsTeam);
                }
                else
                {
                    Team skinsTeam = shirtTeam.PossibleOpponents.FirstOrDefault(sh => sh.Division.Equals(Divisions.Skins)) ?? new Team();
                    if (skinsTeam.IsNull()) error = true;
                    shirtTeam.CrossDivisionalOppId = skinsTeam.Id;
                    skinsTeam.CrossDivisionalOppId = shirtTeam.Id;
                }                
            }

            if (error)
            {
                foreach (Team team in Teams)
                {
                    team.CrossDivisionalOppId = 0;
                }
            }

            foreach (Team team in Teams)
            {
                Console.WriteLine($"{team} cross div opp is: {GetTeam(team.CrossDivisionalOppId)}");
            }
        }

        public void CheckCounts()
        {
            Console.WriteLine("--Validating Counts--\n");
            foreach (Team team in this.Teams)
            {
                Console.WriteLine(team);
                Console.WriteLine($"Divisional {team.DivisionalMatches}");
                Console.WriteLine($"CrossDiv {team.CrossDivisionalMatches}\n");
            }   
        }

        public void OrderByPossibleDivOppDesc2()
        {
            Teams =
            [
                .. Teams.OrderBy(t =>
                    t.PossibleOpponents
                        .Where(op => op.Division == t.Division)
                        .Count())
            ];            
        }

        public void OrderByPossibleDivOppDesc()
        {
            Teams =
            [
                .. Teams.OrderBy(t =>
                    t.PossibleOpponents
                        .GroupBy(op => op.Id)
                        .Where(g => g.Count() > 1)
                        .SelectMany(g => g)
                        .Where(op => op.Division == t.Division)
                        .DistinctBy(t => t.Id)
                        .Count())
            ];
        }

        public void ShuffleTeams()
        {
            int n = Teams.Count;
            Random rng = new();
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                (Teams[k], Teams[n]) = (Teams[n], Teams[k]);
            }
        }         
    }


}