using UnityEngine;

[CreateAssetMenu(fileName = "PoolID_", menuName = "PangoBlocks/PoolID")]
public class PoolID : ScriptableObject
{
	public int ID;

	public int GenerateID()
	{
		ID = Random.Range(0, 1000);
		return ID;
	}
}
