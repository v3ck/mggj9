using Logic.Extensions;
using Logic.Models;
using Logic.Simulation.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic.Simulation.Abilities
{
    internal class SparkAbility(BattleUnit user, BattleState state, HexGrid grid) : IBattleAbility
    {
        public string Code => "SPARK";

        public int CurrentCharge { get; set; } = 0;
        public int MaxCharge { get; } = 0;

        private readonly BattleState _state = state;

        private readonly HexGrid _grid = grid;

        private readonly BattleUnit _user = user;

        public static IBattleAbility Create(BattleUnit user, BattleState state, GameModel gameModel)
        {
            return new SparkAbility(user, state, gameModel.Grid);
        }

        public bool CanUse()
        {
            if (_user?.Location is null)
            {
                return false;
            }

            return _state.Units.Values.Any(IsTargetValid);
        }

        public void TryCharge(IBattleAction action)
        {
            // void
        }

        public IEnumerable<IBattleAction> Use()
        {
            if (_user?.Location is null)
            {
                yield break;
            }

            var target = _state.Units.Values
                .Where(IsTargetValid)
                .Random();

            if (target?.Location is null)
            {
                yield break;
            }

            yield return new AbilityAction()
            {
                BeginLocation = _user.Location,
                EndLocation = target.Location,
                Ability = Code
            };

            yield return new HealthAction()
            {
                UnitId = target.Id,
                Location = target.Location,
                Amount = 2
            };

            target.Health -= 2;
            if (target.Health <= 0)
            {
                yield return new ExistenceAction()
                {
                    UnitId = target.Id,
                    UnitCode = target.Model.Code,
                    Location = target.Location,
                    Exists = false
                };

                _state.Units.Remove(target.Id);
            }
        }

        private bool IsTargetValid(BattleUnit unit)
        {
            return _user.Location is not null &&
                unit.Location is not null &&
                (unit.Location - _user.Location).Magnitude <= 2 &&
                unit.Model.Faction != _user.Model.Faction;
        }
    }
}
