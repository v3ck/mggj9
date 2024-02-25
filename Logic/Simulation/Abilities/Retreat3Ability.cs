using Logic.Models;

namespace Logic.Simulation.Abilities
{
    internal class Retreat3Ability(AbilityModel model, BattleUnit user, BattleState state, GameModel gameModel)
        : RetreatAbility(model, user, state, gameModel, 3)
    {
        public override string Code => "RETREAT_3";

        public static IBattleAbility Create(AbilityModel model, BattleUnit user, BattleState state, GameModel gameModel)
        {
            return new Retreat3Ability(model, user, state, gameModel);
        }
    }
}
