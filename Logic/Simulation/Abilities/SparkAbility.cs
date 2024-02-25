using Logic.Models;

namespace Logic.Simulation.Abilities
{
    internal class SparkAbility(AbilityModel model, BattleUnit user, BattleState state, GameModel gameModel)
        : SingleTargetAttackAbility(model, user, state, gameModel, 2, 2)
    {
        public override string Code => "SPARK";

        public static IBattleAbility Create(AbilityModel model, BattleUnit user, BattleState state, GameModel gameModel)
        {
            return new SparkAbility(model, user, state, gameModel);
        }
    }
}
