using UnityEngine;
using UnityEngine.UI;

public class LevelInfo : MonoBehaviour
{
    public Text Text;

	private void OnEnable()
	{
		if (GameManager.Instance)
		{
			Text.text = "WORLD: " + GameManager.Instance.CurrentWorld.ToString() + " | LEVEL: " + GameManager.Instance.CurrentLevel.ToString(); 
		}
	}
}
