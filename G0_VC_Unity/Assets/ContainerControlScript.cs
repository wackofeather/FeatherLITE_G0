using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ContainerControlScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Button CreateGameButton;
    public void OnEnable()
    {
        EventSystem.current.SetSelectedGameObject(CreateGameButton.gameObject);
    }
}
