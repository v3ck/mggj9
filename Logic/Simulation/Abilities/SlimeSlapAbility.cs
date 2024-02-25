using Logic.Models;

namespace Logic.Simulation.Abilities
{
    internal class SlimeSlapAbility(AbilityModel model, BattleUnit user, BattleState state, GameModel gameModel)
        : SingleTargetAttackAbility(model, user, state, gameModel, 1, 1)
    {
        public override string Code => "SLIME_SLAP";

        public static IBattleAbility Create(AbilityModel model, BattleUnit user, BattleState state, GameModel gameModel)
        {
            return new SlimeSlapAbility(model, user, state, gameModel);
        }
    }
}
