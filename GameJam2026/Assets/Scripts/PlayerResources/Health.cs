namespace PlayerResources
{
    public class Health : PlayerResources
    {
        public override float Amount { get; protected set; } = 60;
        public override float GrowRate { get; protected set; } = -0.15f;
        public override void UpdateResource(PlayerResourceChangeReason reason, float amount)
        {
        }
    }
}