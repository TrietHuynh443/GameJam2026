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
    [SerializeField] private Button _victoryReplayButton;
    [SerializeField] private Button _victoryBackButton;
    [SerializeField] private Button _defeatReplayButton;
    [SerializeField] private Button _defeatBackButton;

    [SerializeField] private GameObject _victory;
    [SerializeField] private GameObject _defeat;
    [SerializeField] private TextMeshProUGUI _normalText;
    [SerializeField] private TextMeshProUGUI _infectedText;
    [SerializeField] private TextMeshProUGUI _maskedText;
    private void Start()
    {
        _victory.SetActive(GameManager.IsWin);
        _defeat.SetActive(!GameManager.IsWin);
        
        _victoryContinueButton.onClick.AddListener(() => ChangeScene(2));
        _victoryReplayButton.onClick.AddListener(() => ChangeScene(1));
        _victoryBackButton.onClick.AddListener(() => ChangeScene(0));
        _defeatReplayButton.onClick.AddListener(() => ChangeScene(1));
        _defeatBackButton.onClick.AddListener(() => ChangeScene(0));
        ShowResult();
    }

    private void OnDestroy()
    {
        _victoryContinueButton.onClick.RemoveAllListeners();
        _victoryReplayButton.onClick.RemoveAllListeners();
        _victoryBackButton.onClick.RemoveAllListeners();
        _defeatReplayButton.onClick.RemoveAllListeners();
        _defeatBackButton.onClick.RemoveAllListeners();
    }

    private void ChangeScene(int phase)
    {
        PlayerResourcesManager.Instance.Get<PlayerScore>().Normal = 0;
        PlayerResourcesManager.Instance.Get<PlayerScore>().Sick = 0;
        PlayerResourcesManager.Instance.Get<PlayerScore>().Masked = 0;
        
        var scene = EScene.MainScene;
        switch (phase)
        {
            case 0:
                scene = EScene.Start;
                break;
            case 2:
                GameManager.NextLevel();
                break;
        }
        
        SceneLoader.Instance.ChangeScene(scene).Forget();
    }

    private void ShowResult()
    {
        var res = PlayerResourcesManager.Instance.Get<PlayerScore>();
        _normalText.text = res.Normal.ToString();
        _infectedText.text = res.Sick.ToString();
        _maskedText.text = res.Masked.ToString();
    }
}
