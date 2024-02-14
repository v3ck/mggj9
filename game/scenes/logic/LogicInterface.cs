using Godot;

namespace Game
{
    public partial class LogicInterface : Node2D
    {
        private readonly Logic.IController _controller = Logic.Util.CreateController();
    }
}
