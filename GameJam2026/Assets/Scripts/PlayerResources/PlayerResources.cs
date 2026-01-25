using UnityEngine;

namespace PlayerResources
{
    public abstract class PlayerResources
    {
        public abstract float Amount { get; protected set; }
        public abstract float GrowRate { get; protected set; }

        public virtual float Current()
        {
            Amount += Amount * GrowRate;
            return Amount;
        }

        public abstract void UpdateResource(PlayerResourceChangeReason reason, float amount);
    }    
}


