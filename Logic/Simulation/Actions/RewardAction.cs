namespace Logic.Simulation.Actions
{
    internal class RewardAction : IBattleAction
    {
        public ActionType Type { get; } = ActionType.Reward;

        public required string[] AbilityCodes { get; init; }
    }
}
