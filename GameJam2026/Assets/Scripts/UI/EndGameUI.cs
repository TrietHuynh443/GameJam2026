using System;
using GameEvent.Events;
using PlayerResources;
using SceneManagement;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EndGameUI : MonoBehaviour
{
    [SerializeField] private Button _victoryContinueButton;
    [SerializeField] private Button _victoryBackButton;
    [SerializeField] private Button _defeatContinueButton;
    [SerializeField] private Button _defeatBackButton;

    [SerializeField] private GameObject _victory;
    [SerializeField] private GameObject _defeat;
    [SerializeField] private TextMeshProUGUI _normalText;
    [SerializeField] private TextMeshProUGUI _infectedText;
    [SerializeField] private TextMeshProUGUI _maskedText;
    [SerializeField] private TextMeshProUGUI _scoreText;
    
    private void Start()
    {
        _victory.SetActive(GameManager.IsWin);
        _defeat.SetActive(!GameManager.IsWin);
        
        _victoryContinueButton.onClick.AddListener(() => ChangeScene(false));
        _victoryBackButton.onClick.AddListener(() => ChangeScene(true));
        _defeatContinueButton.onClick.AddListener(() => ChangeScene(false));
        _defeatBackButton.onClick.AddListener(() => ChangeScene(true));
        ShowResult();
    }

    private void OnDestroy()
    {
        _victoryContinueButton.onClick.RemoveAllListeners();
        _victoryBackButton.onClick.RemoveAllListeners();
        _defeatContinueButton.onClick.RemoveAllListeners();
        _defeatBackButton.onClick.RemoveAllListeners();
    }

    private void ChangeScene(bool isBack)
    {
        PlayerResourcesManager.Instance.Get<PlayerScore>().Normal = 0;
        PlayerResourcesManager.Instance.Get<PlayerScore>().Sick = 0;
        PlayerResourcesManager.Instance.Get<PlayerScore>().Masked = 0;
        
        var scene = EScene.MainScene;
        if (isBack)
        {
            scene = EScene.Start;
        }
        SceneLoader.Instance.ChangeScene(scene).Forget();
    }

    private void ShowResult()
    {
        var res = PlayerResourcesManager.Instance.Get<PlayerScore>();
        _normalText.text = res.Normal.ToString();
        _infectedText.text = res.Sick.ToString();
        _maskedText.text = res.Masked.ToString();
        _scoreText.text = $"Score: {(int)(GameManager.Result * 100)}";
    }
}
