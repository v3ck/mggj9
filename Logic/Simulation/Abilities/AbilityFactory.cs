using Logic.Models;

namespace Logic.Simulation.Abilities
{
    internal static class AbilityFactory
    {
        private static readonly Dictionary<string, Type?> _types = new()
        {
            { "RANDOM_WALK", typeof(RandomWalkAbility) },
            { "REST", typeof(RestAbility) },
            { "SPARK", typeof(SparkAbility) },
            { "APPROACH_ENEMIES_1", typeof(ApproachEnemies1Ability) },
            { "APPROACH_ENEMIES_2", typeof(ApproachEnemies2Ability) },
            { "APPROACH_ALL_3", typeof(ApproachAll3Ability) },
            { "CENTRE_WALK", typeof(CentreWalkAbility) },
            { "LIGHT_ORB", typeof(LightOrbAbility) },
            { "SLIME_SLAP", typeof(SlimeSlapAbility) },
            { "RETREAT_2", typeof(Retreat2Ability) },
            { "RETREAT_3", typeof(Retreat3Ability) },
            { "ASSASSINATE", typeof(AssassinateAbility) },
            { "FIREBALL", typeof(FireballAbility) },
            { "ACCURATE_FIREBALL", typeof(AccurateFireballAbility) },
            { "SUMMON_FAMILIAR", typeof(SummonFamiliarAbility) }
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
