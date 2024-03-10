using Logic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Simulation.Abilities
{
    internal class ApproachEnemies3Ability(AbilityModel model, BattleUnit user, BattleState state, GameModel gameModel)
        : ApproachAbility(model, user, state, gameModel, 3, false, true)
    {
        public override string Code => "APPROACH_ENEMIES_3";

        public static IBattleAbility Create(AbilityModel model, BattleUnit user, BattleState state, GameModel gameModel)
        {
            return new ApproachEnemies3Ability(model, user, state, gameModel);
        }
    }
}
