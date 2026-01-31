using PlayerResources;

namespace PlayerResources
{
    public class PlayerScore : PlayerResources
    {
        public override float Amount { get; protected set; }
        public override float GrowRate { get; protected set; } = 0f;

        public int Masked;
        public int Normal;
        public int Sick;
        

        public override void UpdateResource(PlayerResourceChangeReason reason, float amount)
        {
            switch (reason)
            {
                case PlayerResourceChangeReason.Masked:
                    Masked += (int)amount;
                    break;

                case PlayerResourceChangeReason.Infected:
                    Sick += (int)amount;
                    Normal -= (int)amount;
                    break;

                case PlayerResourceChangeReason.Normal:
                    Normal += (int)amount;
                    break;
            }

            Recalculate();
        }

        private void Recalculate()
        {
            if (Sick <= 0) Sick = 1; // prevent div by zero
            Amount = Masked * Normal / (float)Sick;
        }
    }
}