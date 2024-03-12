using Logic.Extensions;
using Logic.Models;
using Logic.Simulation.Actions;

namespace Logic.Simulation.Abilities
{
    internal class SlimeRainAbility(AbilityModel model, BattleUnit user, BattleState state, GameModel gameModel)
        : BattleAbilityBase(model, user, state, gameModel)
    {
        public override string Code => "SLIME_RAIN";

        private readonly Random rng = new();

        public static IBattleAbility Create(AbilityModel model, BattleUnit user, BattleState state, GameModel gameModel)
        {
            return new SlimeRainAbility(model, user, state, gameModel);
        }

        protected override bool CanUseSpecific()
        {
            if (_user?.Location is null)
            {
                return false;
            }

            return _state.Units.Values.Any(IsTargetValid);
        }

        public override void TryCharge(IBattleAction action)
        {
            if (action is not RoundAction)
            {
                return;
            }

            if (0 < rng.Next(4))
            {
                return;
            }

            _currentCharge++;
        }

        protected override List<IBattleAction> UseSpecific()
        {
            if (_user?.Location is null)
            {
                return [];
            }

            var target = _state.Units.Values
                .Where(IsTargetValid)
                .Random();
            if (target?.Location is null)
            {
                return [];
            }

            _gameModel.Units.TryGetValue("ANGRY_SLIME", out var unitModel);
            if (unitModel is null)
            {
                return [];
            }

            List<IBattleAction> actions = [];
            var locations = _gameModel.Grid
                .AtDistance(target.Location, 1)
                .Where(hex => !_state.Units.Values.Any(unit => hex == unit.Location));
            foreach (var location in locations)
            {
                actions.AddRange(Summon(location, unitModel));
            }

            return actions;
        }

        private bool IsTargetValid(BattleUnit unit)
        {
            return _user.Location is not null &&
                unit.Location is not null &&
                (unit.Location - _user.Location).Magnitude <= 4 &&
                unit.Model.Faction != _user.Model.Faction;
        }

        private List<IBattleAction> Summon(Hex location, UnitModel unitModel)
        {
            var unit = new BattleUnit(unitModel, _state, _gameModel)
            {
                Location = location
            };

            _state.Units[unit.Id] = unit;
            return [new ExistenceAction()
            {
                UnitId = unit.Id,
                UnitCode = "ANGRY_SLIME",
                Exists = true,
                Location = unit.Location
            }];
        }
    }
}
