namespace Logic.Models
{
    internal class GameModel(GameParameters parameters)
    {
        public HexGrid Grid { get; } = new HexGrid(parameters.GridRadius);

        private readonly IDictionary<int, UnitModel> _units = new Dictionary<int, UnitModel>();
        public IDictionary<int, UnitModel> Units => _units;
    }
}
