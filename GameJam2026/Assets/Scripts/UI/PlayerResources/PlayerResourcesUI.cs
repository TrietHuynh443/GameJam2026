using System;
using System.Globalization;
using Cysharp.Threading.Tasks;
using GameEvent.Events;
using PlayerResources;
using TMPro;
using UnityEngine;

namespace PlyerResources
{
    public enum EPlayerScoreType
    {
        Normal = 1,
        Infected = 2,
        Masked = 3,
    }
    public class PlayerResourcesUI : MonoBehaviour
    {
        [SerializeField] private EPlayerScoreType _uiType;
        [SerializeField] private TextMeshProUGUI _resourcesText;
        private void Start()
        {
            SetResource();
            GameEvent.GameEvent.Subscribe<ScoreEvent>(UpdateScore);
        }

        private void OnDestroy()
        {
            GameEvent.GameEvent.Unsubscribe<ScoreEvent>(UpdateScore);
        }

        private void UpdateScore(ScoreEvent obj)
        {
            UniTask.WaitForEndOfFrame().ContinueWith(SetResource);
        }

        private void SetResource()
        {
            switch (_uiType)
            {
                case EPlayerScoreType.Normal:
                    _resourcesText.text = $"{PlayerResourcesManager.Instance.Get<PlayerScore>().Normal}";
                    break;
                case EPlayerScoreType.Infected:
                    var sicks = PlayerResourcesManager.Instance.Get<PlayerScore>().Sick;
                    _resourcesText.text = $"{sicks}";
                    break;
                case EPlayerScoreType.Masked:
                    var masked = PlayerResourcesManager.Instance.Get<PlayerScore>().Masked;
                    _resourcesText.text = $"{masked}";
                    break;
                default:
                    return;
            }
        }
        
        
    }

}
