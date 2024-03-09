using Logic.Models;

namespace Logic.Simulation.Abilities
{
    internal class DarkOrbAbility(AbilityModel model, BattleUnit user, BattleState state, GameModel gameModel)
        : SingleTargetAttackAbility(model, user, state, gameModel, 4, 4)
    {
        public override string Code => "DARK_ORB";

        public static IBattleAbility Create(AbilityModel model, BattleUnit user, BattleState state, GameModel gameModel)
        {
            return new DarkOrbAbility(model, user, state, gameModel);
        }
    }
}
