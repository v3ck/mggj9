using Logic.Models;

namespace Logic.Simulation.Abilities
{
    internal class ApproachEnemies1Ability(AbilityModel model, BattleUnit user, BattleState state, GameModel gameModel)
        : ApproachAbility(model, user, state, gameModel, 1, false, true)
    {
        public override string Code => "APPROACH_ENEMIES_1";

        public static IBattleAbility Create(AbilityModel model, BattleUnit user, BattleState state, GameModel gameModel)
        {
            return new ApproachEnemies1Ability(model, user, state, gameModel);
        }
    }
}
