using Logic.Models;
using System.Diagnostics;
using System.Reflection;

namespace Logic.Simulation.Abilities
{
    internal static class AbilityFactory
    {
        private static readonly Dictionary<string, Type?> _types = new()
        {
            { "RANDOM_WALK", typeof(RandomWalkAbility) },
            { "SPARK", typeof(SparkAbility) },
            { "APPROACH", typeof(ApproachAbility) }
        };

        public static IBattleAbility? Create(AbilityModel model, BattleUnit user, BattleState battleState, GameModel gameModel)
        {
            var obj = _types.GetValueOrDefault(model.Code)?
                .GetMethod("Create", [typeof(AbilityModel), typeof(BattleUnit), typeof(BattleState), typeof(GameModel)])?
                .Invoke(null, [model, user, battleState, gameModel]);
            return obj as IBattleAbility;
        }
    }
}
