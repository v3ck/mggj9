using Logic.Simulation.Actions;

namespace Logic.Simulation.Abilities
{
    internal interface IBattleAbility
    {
        public static string Code { get; } = string.Empty;

        public int CurrentCharge { get; set; }

        public int MaxCharge { get; }

        public void TryCharge(IBattleAction action);

        public bool CanUse(BattleUnit user);

        public IEnumerable<IBattleAction> Use(BattleUnit user);
    }
}
