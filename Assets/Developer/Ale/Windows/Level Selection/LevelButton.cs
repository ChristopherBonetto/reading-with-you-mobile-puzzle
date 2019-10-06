using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Component attached to buttons in level selection.
/// </summary>
public class LevelButton : MonoBehaviour
{
    // Button (Unity object) references.
    public Button Button { get; private set; }
    public Image Image { get; private set; }


    private void Awake()
    {
        Button = GetComponent<Button>();
        Image = GetComponentInChildren<Image>();
    }
}
