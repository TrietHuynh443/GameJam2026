namespace PlayerResources
{
    public class Cash : PlayerResources
    {
        public override float Amount { get; protected set; } = 1000;
        public override float GrowRate { get; protected set; } = 0.03f;
        public override void UpdateResource(PlayerResourceChangeReason reason, float amount)
        {
        }
    }
}