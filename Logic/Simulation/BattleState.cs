namespace Logic.Simulation
{
    internal class BattleState
    {
        private readonly Dictionary<int, BattleUnit> _units = [];
        public Dictionary<int, BattleUnit> Units => _units;

        private readonly List<BattleSpawn> _spawns = [];
        public List<BattleSpawn> Spawns => _spawns;

        public int Round { get; set; }

        public int Score { get; set; } = 0;
    }
}
