using Logic.Util;

namespace Logic.Models
{
    internal class UnitModel
    {
        private static int _nextId = 0;

        public int Id { get; } = _nextId++;

        public required UnitCode Code { get; init; }

        public required bool IsEnemy { get; init; }

        public IList<AbilityCode> Abilities { get; } = new List<AbilityCode>();
    }
}
