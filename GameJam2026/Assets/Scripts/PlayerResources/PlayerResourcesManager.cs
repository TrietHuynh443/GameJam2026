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
                var instance = (PlayerResources)Activator.CreateInstance(x);
                return new KeyValuePair<Type, PlayerResources>(x, instance);
            }));

            RegisterGameEvents();
        }

        private void RegisterGameEvents()
        {
            GameEvent.GameEvent.Subscribe<ScoreEvent>(HandleScoreChanged);
        }

        private object _lock = new();
        private void HandleScoreChanged(ScoreEvent obj)
        {
            lock (_lock)
            {
                Get<PlayerScore>().Masked += obj.Masked;
                Get<PlayerScore>().Normal += obj.Normal;
                Get<PlayerScore>().Sick += obj.Infected;
            }   
        }

        public T Get<T>() where T : PlayerResources
        {
            if (!IsInitialized)
            {
                InitializeSingleton();
            }
            return _playerResourcesMap[typeof(T)] as T;
        }
    
        
    }
}