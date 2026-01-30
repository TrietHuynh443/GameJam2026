using PlayerResources;

namespace PlayerResources
{
    public class PlayerScore : PlayerResources
    {
        public override float Amount { get; protected set; }
        public override float GrowRate { get; protected set; }

        private int _masked;
        private int _normal;
        private int _sick;

        public PlayerScore(float growRate = 1f)
        {
            GrowRate = growRate;
        }

        public override void UpdateResource(PlayerResourceChangeReason reason, float amount)
        {
            switch (reason)
            {
                case PlayerResourceChangeReason.Masked:
                    _masked += (int)amount;
                    break;

                case PlayerResourceChangeReason.Infected:
                    _sick += (int)amount;
                    _normal -= (int)amount;
                    break;

                case PlayerResourceChangeReason.Normal:
                    _normal += (int)amount;
                    break;
            }

            Recalculate();
        }

        private void Recalculate()
        {
            if (_sick <= 0) _sick = 1; // prevent div by zero
            Amount = _masked * _normal / (float)_sick;
        }
    }
}