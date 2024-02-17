using Logic.Util;

namespace Logic.Models
{
    internal class UnitModel
    {
        private static int _nextId = 0;

        public int Id { get; } = _nextId++;

        public required string Code { get; init; }

        public required string Faction { get; init; }

        public required int MaxHealth { get; init; }

        public IList<string> Abilities { get; } = new List<string>();
    }
}
