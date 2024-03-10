namespace Logic.Models
{
    internal class GameModel(GameParameters parameters)
    {
        public HexGrid Grid { get; } = new HexGrid(parameters.GridRadius);

        public Dictionary<string, UnitModel> Units { get; } = [];

        public List<SpawnModel> Spawns { get; } = [];

        public Dictionary<string, AbilityModel> Abilities { get; } = [];

        public List<string> CodexAbilities { get; } = [];

        public int RewardInterval { get; } = parameters.RewardInterval;
    }
}
