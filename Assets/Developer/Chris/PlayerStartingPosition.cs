using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStartingPosition : MonoBehaviour
{
    private void OnEnable()
    {
        GameManager.Instance.Player.transform.position = gameObject.transform.position;
    }
}
