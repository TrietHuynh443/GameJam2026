using UnityEngine;

namespace PlayerResources
{
    public abstract class PlayerResources
    {
        public abstract float Amount { get; protected set; }
        public abstract float GrowRate { get; protected set; }

        public virtual float Current()
        {
            var res = Amount + Amount * GrowRate;
            return res;
        }

        public abstract void UpdateResource(PlayerResourceChangeReason reason, float amount);
    }    
}


