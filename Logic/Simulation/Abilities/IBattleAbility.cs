using Logic.Simulation.Actions;
using Logic.Util;

namespace Logic.Simulation.Abilities
{
    internal interface IBattleAbility
    {
        public static AbilityCode Code { get; }

        public int CurrentCharge { get; set; }

        public int MaxCharge { get; }

        public void TryCharge(IBattleAction action);

        public bool CanUse(BattleUnit user);

        public IEnumerable<IBattleAction> Use(BattleUnit user);
    }
}
