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
        private Image _backgroundHolder;
        private Slider _slider;

        private bool _isInitialize;
        private readonly string _sceneKey = "Loader"; // Your Addressable Scene Key
        

        protected override void Awake()
        {
            base.Awake();
            _backgroundHolder = GetComponentInChildren<Image>();
            _slider = GetComponentInChildren<Slider>();
            Initialize().Forget();
        }
        
        private async UniTaskVoid Initialize()
        {
    
            var sizeHandle = Addressables.GetDownloadSizeAsync(_sceneKey);
            
            var downloadSize = await sizeHandle.ToUniTask();
            
            Debug.Log($"Get download size {downloadSize}");
            Addressables.Release(sizeHandle);

            if (downloadSize > 0)
            {
                await Addressables.DownloadDependenciesAsync(_sceneKey, autoReleaseHandle: true).ToUniTask(
                        progress: new Progress<float>((p) =>
                        {
                            if (_slider) _slider.value = p;
                        })
                    );
            }
    
            _backgroundHolder?.gameObject.SetActive(false);
            _isInitialize = true;
        }

        private void Start()
        {
            ChangeScene(EScene.MainScene).Forget();
        }

        public async UniTaskVoid ChangeScene(EScene scene, Action onComplete = null)
        {
            await UniTask.WaitUntil(() => _isInitialize);

            var sceneInstance = await Addressables.LoadSceneAsync(_sceneKey);

            var changeSceneTask = ChangeSceneInternal(scene.AsSerialized());
            
            var minTimeLoadTask = UniTask.WaitForSeconds(3);
            
            await UniTask.WhenAll(changeSceneTask, minTimeLoadTask);

            await Addressables.UnloadSceneAsync(sceneInstance);
        }

        private async UniTask ChangeSceneInternal(string sceneName)
        {
            await SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        }
        
    }
}