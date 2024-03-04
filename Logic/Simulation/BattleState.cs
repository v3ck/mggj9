namespace Logic.Simulation
{
    internal class BattleState
    {
        private readonly IDictionary<int, BattleUnit> _units = new Dictionary<int, BattleUnit>();
        public IDictionary<int, BattleUnit> Units => _units;

        public int Round { get; set; }

        public int Score { get; set; } = 0;
    }
}
