using Logic.Models;

namespace Logic.Simulation
{
    internal class BattleAction
    {
        public enum ActionType
        {
            Move,
            Teleport,
            Projectile,
            Damage,
            Healing,
            Status
        }

        public required ActionType Type { get; init; }

        public required Hex Source { get; init; }

        public required Hex Target { get; init; }

        public required int AbilityId { get; init; }
    }
}
