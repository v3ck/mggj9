using Logic.Models;

namespace Logic.Simulation.Abilities
{
    internal class ShadowOrbAbility(AbilityModel model, BattleUnit user, BattleState state, GameModel gameModel)
        : SingleTargetAttackAbility(model, user, state, gameModel, 4, 2)
    {
        public override string Code => "SHADOW_ORB";

        public static IBattleAbility Create(AbilityModel model, BattleUnit user, BattleState state, GameModel gameModel)
        {
            return new ShadowOrbAbility(model, user, state, gameModel);
        }
    }
}
