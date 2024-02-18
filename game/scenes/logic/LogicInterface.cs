using Godot;
using Logic.Util;
using System.Linq;

namespace Game
{
    /// <summary>
    /// Wraps Logic module to be used by Godot
    /// </summary>
	public partial class LogicInterface : Node
	{
        [Signal]
        public delegate void UnitMovedEventHandler(int id, Vector2I fromLocation, Vector2I toLocation, bool isTeleport);

        [Signal]
        public delegate void StatusChangedEventHandler(int id, Vector2I location, string status);

        [Signal]
        public delegate void HealthChangedEventHandler(int id, Vector2I location, int health);

        [Signal]
        public delegate void AbilityFiredEventHandler(Vector2I fromLocation, Vector2I toLocation, string ability);

		private readonly Logic.IController _controller = Logic.Api.CreateController();

        public override void _Ready()
        {
            _controller.UnitMoved += Controller_UnitMoved;
            _controller.StatusChanged += Controller_StatusChanged;
            _controller.HealthChanged += Controller_HealthChanged;
            _controller.AbilityFired += Controller_AbilityFired;
        }

        public void AddUnit(Resource unitResource)
		{
            var abilities = unitResource
                .Get("default_abilities")
                .AsGodotArray<Resource>()
                .AsEnumerable()
                .Select(abilityResource => abilityResource.Get("code").AsString())
                .ToArray();

            _controller.AddUnit(
                unitResource.Get("code").AsString(),
                unitResource.Get("default_health").AsInt32(),
                unitResource.Get("faction").AsString(),
                abilities);
		}

        private void Controller_UnitMoved(object sender, Logic.Events.UnitMovedEventArgs e)
        {
            EmitSignal(
                SignalName.UnitMoved,
                e.UnitId,
                IntVector2ToVector2I(e.FromLocation),
                IntVector2ToVector2I(e.ToLocation),
                e.IsTeleport);
        }

        private void Controller_StatusChanged(object sender, Logic.Events.StatusChangedEventArgs e)
        {
            EmitSignal(
                SignalName.StatusChanged,
                e.UnitId,
                IntVector2ToVector2I(e.Location),
                e.Status);
        }

        private void Controller_HealthChanged(object sender, Logic.Events.HealthChangedEventArgs e)
        {
            EmitSignal(
                SignalName.HealthChanged,
                e.UnitId,
                IntVector2ToVector2I(e.Location),
                e.Amount);
        }

        private void Controller_AbilityFired(object sender, Logic.Events.AbilityFiredEventArgs e)
        {
            EmitSignal(
                SignalName.AbilityFired,
                IntVector2ToVector2I(e.FromLocation),
                IntVector2ToVector2I(e.ToLocation),
                e.Ability);
        }

        private static Vector2I IntVector2ToVector2I(IntVector2 iv)
        {
            return new Vector2I(iv.X, iv.Y);
        }
	}
}
