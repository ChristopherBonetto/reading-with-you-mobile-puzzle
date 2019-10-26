using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldBackgroundWindow : UIControl
{
    public override UIControlName Name => UIControlName.WorldBackground;

    public RectTransform Size { get; private set; }
    public Image Image { get; private set; }


    private void Awake()
    {
        Size = GetComponent<RectTransform>();
        Image = GetComponent<Image>();
    }

    protected override void Start()
    {
        UIManager.Instance.Register(this);

        Size.sizeDelta = new Vector2(Camera.main.orthographicSize * 2 * Camera.main.aspect, Camera.main.orthographicSize * 2);
    }

    public void SetBackgroundImage(Sprite sprite)
    {
        Image.sprite = sprite;
    }
}
