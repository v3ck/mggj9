using Logic.Models;

namespace Logic.Simulation.Abilities
{
    internal class ApproachEnemies2Ability(AbilityModel model, BattleUnit user, BattleState state, GameModel gameModel)
        : ApproachAbility(model, user, state, gameModel, 2, false, true)
    {
        public override string Code => "APPROACH_ENEMIES_2";

        public static IBattleAbility Create(AbilityModel model, BattleUnit user, BattleState state, GameModel gameModel)
        {
            return new ApproachEnemies2Ability(model, user, state, gameModel);
        }
    }
}
