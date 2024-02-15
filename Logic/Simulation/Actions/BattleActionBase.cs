namespace Logic.Simulation.Actions
{
    internal abstract class BattleActionBase(BattleActionBase.ActionType type)
    {
        public enum ActionType
        {
            Move,
            Ability,
            Health,
            Status
        }

        public required ActionType Type { get; init; } = type;
    }
}
