using Logic.Models;

namespace Logic.Simulation.Abilities
{
    internal class Retreat2Ability(AbilityModel model, BattleUnit user, BattleState state, GameModel gameModel)
        : RetreatAbility(model, user, state, gameModel, 2)
    {
        public override string Code => "RETREAT_2";

        public static IBattleAbility Create(AbilityModel model, BattleUnit user, BattleState state, GameModel gameModel)
        {
            return new Retreat2Ability(model, user, state, gameModel);
        }
    }
}
