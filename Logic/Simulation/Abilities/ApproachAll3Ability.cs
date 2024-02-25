using Logic.Models;

namespace Logic.Simulation.Abilities
{
    internal class ApproachAll3Ability(AbilityModel model, BattleUnit user, BattleState state, GameModel gameModel)
        : ApproachAbility(model, user, state, gameModel, 3, true, true)
    {
        public override string Code => "APPROACH_ALL_3";

        public static IBattleAbility Create(AbilityModel model, BattleUnit user, BattleState state, GameModel gameModel)
        {
            return new ApproachAll3Ability(model, user, state, gameModel);
        }
    }
}
