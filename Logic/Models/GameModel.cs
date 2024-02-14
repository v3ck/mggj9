namespace Logic.Models
{
    internal class GameModel(GameParameters parameters)
    {
        public HexGrid Grid { get; } = new HexGrid(parameters.GridRadius);
    }
}
