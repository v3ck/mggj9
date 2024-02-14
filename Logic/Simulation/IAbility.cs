using Logic.Models;
using System.ComponentModel.DataAnnotations;

namespace Logic.Simulation
{
    internal interface IAbility
    {
        public event EventHandler<BattleAction>? ActionPerformed;

        public int Id { get; init; }

        public int CurrentCharge { get; set; }

        public int MaxCharge { get; set; }

        public void TryCharge(BattleAction action);

        public bool CanUse(Unit user);

        public IEnumerable<BattleAction> Use(Unit user);
    }
}
