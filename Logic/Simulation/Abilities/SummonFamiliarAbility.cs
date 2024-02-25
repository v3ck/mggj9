using Logic.Extensions;
using Logic.Models;
using Logic.Simulation.Actions;
using Logic.Util;

namespace Logic.Simulation.Abilities
{
    internal class SummonFamiliarAbility(AbilityModel model, BattleUnit user, BattleState state, GameModel gameModel)
        : BattleAbilityBase(model, user, state, gameModel)
    {
        public override string Code => "SUMMON_FAMILIAR";

        public static IBattleAbility Create(AbilityModel model, BattleUnit user, BattleState state, GameModel gameModel)
        {
            return new SummonFamiliarAbility(model, user, state, gameModel);
        }

        protected override bool CanUseSpecific()
        {
            if (_user?.Location is null)
            {
                return false;
            }

            return _gameModel.Grid.AtDistance(_user.Location, 1)
                .Any((Hex hex) => !_state.Units.Values.Any(unit => unit.Location == hex));
        }

        public override void TryCharge(IBattleAction action)
        {
            if (action is not HealthAction healthAction)
            {
                return;
            }

            if (!_state.Units.TryGetValue(healthAction.UnitId, out var unit))
            {
                return;
            }

            if (unit.Model.Faction != _user.Model.Faction)
            {
                return;
            }

            var damage = Math.Max(0, healthAction.PreviousAmount - healthAction.Amount);
            _currentCharge = Math.Min(_model.MaxCharge, _currentCharge + damage);
        }

        protected override List<IBattleAction> UseSpecific()
        {
            if (_user?.Location is null)
            {
                return [];
            }

            var target = _gameModel.Grid.AtDistance(_user.Location, 1)
                .Where((Hex hex) => !_state.Units.Values.Any(unit => unit.Location == hex))
                .Random();
            if (target is null)
            {
                return [];
            }

            _gameModel.Units.TryGetValue("FAMILIAR", out var unitModel);
            if (unitModel is null)
            {
                return [];
            }

            var unit = new BattleUnit(unitModel, _state, _gameModel)
            {
                Location = target
            };

            _state.Units[unit.Id] = unit;
            return [new ExistenceAction()
            {
                UnitId = unit.Id,
                UnitCode = "FAMILIAR",
                Exists = true,
                Location = unit.Location
            }];
        }
    }
}
