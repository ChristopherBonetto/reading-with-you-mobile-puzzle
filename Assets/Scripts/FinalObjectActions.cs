using UnityEngine;

public class FinalObjectActions : MonoBehaviour
{
	public void ResetLevel(Vector3 startPosition)
	{
		transform.position = startPosition;
	}
}
