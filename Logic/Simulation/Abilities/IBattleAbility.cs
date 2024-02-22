using Logic.Models;
using Logic.Simulation.Actions;

namespace Logic.Simulation.Abilities
{
    internal interface IBattleAbility
    {
        public string Code { get; }

        public static IBattleAbility Create(AbilityModel model, BattleUnit user, BattleState state, GameModel gameModel)
            => throw new NotImplementedException();

        public void TryCharge(IBattleAction action);

        public bool CanUse();

        public bool CanPay();

        public List<IBattleAction> Use();
    }
}
