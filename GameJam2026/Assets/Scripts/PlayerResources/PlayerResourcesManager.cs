using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Extension;
using GameEvent.Events;
using Unity.VisualScripting;
using UnityCommunity.UnitySingleton;

namespace PlayerResources
{
    public class PlayerResourcesManager : PersistentMonoSingleton<PlayerResourcesManager>
    {
        private readonly Dictionary<Type, PlayerResources> _playerResourcesMap = new();

        public override void InitializeSingleton()
        {
            base.InitializeSingleton();
            Assembly assembly = Assembly.GetExecutingAssembly();

            _playerResourcesMap.AddRange(assembly.GetClassesOfType<PlayerResources>().Select(x =>
            {
                var instance = (PlayerResources)assembly.CreateInstance(x.Name);
                return new KeyValuePair<Type, PlayerResources>(x, instance);
            }));

            RegisterGameEvents();
        }

        private void RegisterGameEvents()
        {
            GameEvent.GameEvent.Subscribe<NextDaysEvent>(SkipToNextTime);
        }

        public T Get<T>() where T : PlayerResources
        {
            if (!IsInitialized)
            {
                InitializeSingleton();
            }
            return _playerResourcesMap[typeof(T)] as T;
        }
    
        private void SkipToNextTime(NextDaysEvent evt)
        {
            foreach (var resource in _playerResourcesMap.Values)
            {
                for(int i = 1; i <= evt.Days; ++i)
                    resource.Current();
            }
        }
    }
}