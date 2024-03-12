using Logic.Models;

namespace Logic.Simulation.Abilities
{
    internal class AcidBlastAbility(AbilityModel model, BattleUnit user, BattleState state, GameModel gameModel)
        : SingleTargetAttackAbility(model, user, state, gameModel, 3, 8)
    {
        public override string Code => "ACID_BLAST";

        public static IBattleAbility Create(AbilityModel model, BattleUnit user, BattleState state, GameModel gameModel)
        {
            return new AcidBlastAbility(model, user, state, gameModel);
        }
    }
}
