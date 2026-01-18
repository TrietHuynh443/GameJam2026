using System;
using Cysharp.Threading.Tasks;
using Extension;
using Unity.VisualScripting;
using UnityCommunity.UnitySingleton;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace SceneManagement
{
    public enum EScene
    {
        [SerializeAs("SampleScene")] SampleScene = 0,
        [SerializeAs("Main Scene")] MainScene = 1
    }
    public class SceneLoader : PersistentMonoSingleton<SceneLoader>
    {
        [SerializeField] private AssetReference _splashScene;
        [SerializeField] private Image _backgroundHolder;
        private bool _isInitialize;

        protected override void Awake()
        {
            base.Awake();
            if (_splashScene == null)
            {
                throw new NullReferenceException("Missing Reference");
            }

            _isInitialize = true;
        }

        private void Start()
        {
            ChangeScene(EScene.MainScene).Forget();
        }

        public async UniTaskVoid ChangeScene(EScene scene, Action onComplete = null)
        {
            _backgroundHolder.gameObject.SetActive(true);
            await UniTask.WaitUntil(() => _isInitialize);
            var sceneInstance = await Addressables.LoadSceneAsync(_splashScene, LoadSceneMode.Single);
            _backgroundHolder.gameObject.SetActive(false);
            var changeSceneTask = ChangeSceneInternal(scene.AsSerialized());
            var minTimeLoadTask = UniTask.WaitForSeconds(5);
            await UniTask.WhenAll(changeSceneTask, minTimeLoadTask);
            await Addressables.UnloadSceneAsync(sceneInstance);
        }

        private async UniTask ChangeSceneInternal(string sceneName)
        {
            await SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        }
    }
}