using FFSchedulingApp.Enums;
using static FFSchedulingApp.Model.Match;

namespace FFSchedulingApp.Model
{
    public class Team
    {
        /// <summary>
        /// Team Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Team Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Team Division
        /// </summary>
        public Divisions Division { get; set; }
        /// <summary>
        /// Should end up being the number of divisional opponents * 2 (8)
        /// </summary>
        public int DivisionalMatches { get; set; }
        /// <summary>
        /// Should end up being the number of cross divisional opponents + 1 (rivalry) (6)
        /// </summary>
        public int CrossDivisionalMatches { get; set; }

        /// <summary>
        /// List of remaining matchups by team id
        /// </summary>
        public List<Team> PossibleOpponents { get; set; }

        /// <summary>
        /// Empty Team object
        /// </summary>
        public Team()
        {
            Id = 0;
            Name = "";
            Division = Divisions.None;
            PossibleOpponents = [];
        }

        /// <summary>
        /// Returns a Team Object
        /// </summary>
        /// <param name="id">Seed of the team</param>
        /// <returns>Team object with Id, Name of team, and divsion the team is in</returns>
        public Team(int id)
        {
            Id = id;
            Name = Enum.GetName(typeof(TeamInfo), id) ?? "";
            Division = (id + 10) % 2 == 0 ? Divisions.Shirts : Divisions.Skins;
            PossibleOpponents = [];
        }

        /// <summary>
        /// Updates the number of match types for this team
        /// </summary>
        /// <param name="newMatch">The match that has just been added to a Week</param>
        public void UpdateMatchTypes(Match newMatch)
        {
            bool isDivisional = newMatch.MatchType == MatchTypes.Divisional;
            DivisionalMatches += isDivisional ? 1 : 0;
            CrossDivisionalMatches += isDivisional ? 0 : 1;
        }

        /// <summary>
        /// Total number of matches
        /// </summary>
        /// <returns>Total of divisional and nondivisional matches</returns>
        public int TotalMatches()
        {
            return DivisionalMatches + CrossDivisionalMatches;
        }

        public override string ToString()
        {
            return $"({Id}) {Name}";
        }

        public string OpponentsToString()
        {        
            return string.Join(", ", PossibleOpponents);
        }

        public bool IsNull()
        {
            return Id == 0;
        }
    }
}