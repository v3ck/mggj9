using Logic.Models;
using System.Diagnostics;

namespace Logic.Simulation
{
    internal class BattleSpawn(SpawnModel model)
    {
        public string UnitCode => _model.UnitCode;

        private double _progress = 0.5;

        private readonly SpawnModel _model = model;

        private readonly Random rng = new();

        public int Spawn(int round)
        {
            if (round < _model.BeginRound)
            {
                return 0;
            }

            if (_model.EndRound <= round)
            {
                return 0;
            }

            _progress += (_model.Volatility * 2.0 * rng.NextDouble() + (1.0 - _model.Volatility)) * _model.Rate;
            //Debug.WriteLine($"[{_model.UnitCode}] -- [{_progress}]");
            var count = (int)(_progress);
            _progress -= Math.Floor(_progress);
            return count;
        }
    }
}
