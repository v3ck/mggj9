namespace Logic.Simulation
{
    internal class BattleState
    {
        private readonly IDictionary<int, Unit> _units = new Dictionary<int, Unit>();
        public IDictionary<int, Unit> Units => _units;

        public Queue<int> Initiative = new();
    }
}
