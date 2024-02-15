using Logic.Models;
using Logic.Simulation.Actions;
using Logic.Util;
using System.ComponentModel.DataAnnotations;

namespace Logic.Simulation.Abilities
{
    internal interface IBattleAbility
    {
        public event EventHandler<BattleActionBase>? ActionPerformed;

        public static AbilityCode Code { get; }

        public int CurrentCharge { get; set; }

        public int MaxCharge { get; }

        public void TryCharge(BattleActionBase action);

        public bool CanUse(BattleUnit user);

        public IEnumerable<BattleActionBase> Use(BattleUnit user);
    }
}
