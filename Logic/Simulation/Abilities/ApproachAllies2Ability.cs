using Logic.Models;

namespace Logic.Simulation.Abilities
{
    internal class ApproachAllies2Ability(AbilityModel model, BattleUnit user, BattleState state, GameModel gameModel)
        : ApproachAbility(model, user, state, gameModel, 2, true, false)
    {
        public override string Code => "APPROACH_ALLIES_2";

        public static IBattleAbility Create(AbilityModel model, BattleUnit user, BattleState state, GameModel gameModel)
        {
            return new ApproachAllies2Ability(model, user, state, gameModel);
        }
    }
}
