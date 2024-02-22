using Logic.Models;
using Logic.Simulation.Actions;

namespace Logic.Simulation.Abilities
{
    internal abstract class BattleAbilityBase(
        AbilityModel model, BattleUnit user, BattleState state, GameModel gameModel) : IBattleAbility
    {
        public abstract string Code { get; }

        protected int _currentCharge = 0;

        protected readonly BattleState _state = state;

        protected readonly GameModel _gameModel = gameModel;

        protected readonly BattleUnit _user = user;

        protected readonly AbilityModel _model = model;

        public abstract void TryCharge(IBattleAction action);

        public bool CanUse()
        {
            if (_currentCharge < _model.MaxCharge)
            {
                return false;
            }

            return CanUseSpecific();
        }

        protected abstract bool CanUseSpecific();

        public bool CanPay()
        {
            return _model.Cost <= _user.AbilityPoints;
        }

        public List<IBattleAction> Use()
        {
            _currentCharge = 0;
            _user.AbilityPoints -= _model.Cost;
            return UseSpecific();
        }

        protected abstract List<IBattleAction> UseSpecific();
    }
}
