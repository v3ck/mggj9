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
            { "SUMMON_FAMILIAR", typeof(SummonFamiliarAbility) },
            { "EXPLOSION", typeof(ExplosionAbility) },
            { "CHAIN_LIGHTNING", typeof(ChainLightningAbility) },
            { "SUMMON_MARMOT", typeof(SummonMarmotAbility) },
            { "DARK_ORB", typeof(DarkOrbAbility) },
            { "TIME_STOP", typeof(TimeStopAbility) },
            { "SHADOW_ORB", typeof(ShadowOrbAbility) },
            { "STOMP", typeof(StompAbility) },
            { "SLIME_BURST", typeof(SlimeBurstAbility) },
            { "LIGHT_BEAM", typeof(LightBeamAbility) },
            { "APPROACH_ENEMIES_3", typeof(ApproachEnemies3Ability) },
            { "BLESSING", typeof(BlessingAbility) },
            { "ACID_ERUPTION", typeof(AcidEruptionAbility) },
            { "ACID_BLAST", typeof(AcidBlastAbility) },
            { "SLIME_RAIN", typeof(SlimeRainAbility) },
            { "RADIANCE", typeof(RadianceAbility) },
            { "WARP_SPACE", typeof(WarpSpaceAbility) },
            { "VOID_ORB", typeof(VoidOrbAbility) },
            { "RIDE_THE_LIGHTNING", typeof(RideTheLightningAbility) },
            { "PROTECT", typeof(ProtectAbility) },
            { "FREEZE", typeof(FreezeAbility) },
            { "APPROACH_ALLIES_2", typeof(ApproachAllies2Ability) }
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
