using Logic.Models;
using Logic.Simulation.Actions;

namespace Logic.Simulation.Abilities
{
    internal interface IBattleAbility
    {
        public string Code { get; }

        public int CurrentCharge { get; set; }

        public int MaxCharge { get; }

        public static IBattleAbility Create(BattleUnit user, BattleState state, GameModel gameModel)
            => throw new NotImplementedException();

        public void TryCharge(IBattleAction action);

        public bool CanUse();

        public IEnumerable<IBattleAction> Use();
    }
}
