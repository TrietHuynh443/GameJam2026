using System.Collections;
using UnityEngine;

public class IslandButton : MonoBehaviour
{
    enum State
    {
        Normal,
        Hovered,
        Clicked
    }
    
    [SerializeField] private GameObject normal;
    [SerializeField] private GameObject hovered;
    [SerializeField] private GameObject clicked;
    
    private State currentState = State.Normal;
    private bool isMouseOver = false;
    
    private void OnMouseEnter()
    {
        isMouseOver = true;
        ShowState(State.Hovered);
    }

    private void OnMouseExit()
    {
        isMouseOver = false;
        if (currentState == State.Hovered)
            ShowState(State.Normal);
    }

    private void OnMouseDown()
    {
        ShowState(State.Clicked);
        StartCoroutine(ResetClickState());
    }

    private IEnumerator ResetClickState()
    {
        yield return new WaitForSeconds(0.3f);
        ShowState(isMouseOver ? State.Hovered : State.Normal);
    }


    private void ShowState(State state)
    {
        normal.SetActive(state == State.Normal);
        hovered.SetActive(state == State.Hovered);
        clicked.SetActive(state == State.Clicked);
        
        currentState = state;
    }
}
