using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonLink : MonoBehaviour
{
    [TextArea]
    public string Url;


    /// <summary>
    /// Open privacy policy on click.
    /// </summary>
    public void OnClickLink()
    {
        Application.OpenURL(Url);
    }
}
