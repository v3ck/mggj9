namespace Logic.Simulation
{
    internal class BattleState
    {
        private readonly IDictionary<int, BattleUnit> _units = new Dictionary<int, BattleUnit>();
        public IDictionary<int, BattleUnit> Units => _units;

        public Queue<int> Initiative = new();
    }
}
