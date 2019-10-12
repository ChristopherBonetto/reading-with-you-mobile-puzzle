using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalObjectPosition : MonoBehaviour
{
    private void OnEnable()
    {
        GameManager.Instance.FinalObject.transform.position = gameObject.transform.position;
    }
    
}
