using Logic.Util;

namespace Logic.Models
{
    internal class UnitModel
    {
        public required string Code { get; init; }

        public required string Faction { get; init; }

        public required int MaxHealth { get; init; }

        public List<string> Abilities { get; } = [];
    }
}
