using Logic.Models;

namespace Logic.Simulation.Abilities
{
    internal static class AbilityFactory
    {
        private static readonly Dictionary<string, Type?> _types = new()
        {
            { "RANDOM_WALK", typeof(RandomWalkAbility) }
        };

        //public static IBattleAbility? Create(string code, BattleUnit user, BattleState battleState, GameModel gameModel)
        //{
        //    var obj = _types.GetValueOrDefault(code)?
        //        .GetConstructor([typeof(BattleUnit), typeof(BattleState), typeof(GameModel)])?
        //        .Invoke([user, battleState, gameModel]);
        //    return obj as IBattleAbility;
        //}

        public static IBattleAbility? Create(string code, BattleUnit user, BattleState battleState, GameModel gameModel)
        {
            var obj = _types.GetValueOrDefault(code)?
                .GetMethod("Create", [typeof(BattleUnit), typeof(BattleState), typeof(GameModel)])?
                .Invoke(null, [user, battleState, gameModel]);
            return obj as IBattleAbility;
        }
    }
}
