namespace Logic.Models
{
    internal class SpawnModel
    {
        public required string UnitCode { get; init; }

        public required int BeginRound { get; init; }

        public required int EndRound { get; init; }

        public required double Rate { get; init; }

        public required double Volatility { get; init; }
    }
}
