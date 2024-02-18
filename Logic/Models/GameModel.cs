namespace Logic.Models
{
    internal class GameModel(GameParameters parameters)
    {
        public HexGrid Grid { get; } = new HexGrid(parameters.GridRadius);

        private readonly IDictionary<string, UnitModel> _units = new Dictionary<string, UnitModel>();
        public IDictionary<string, UnitModel> Units => _units;
    }
}
