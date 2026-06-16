using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace JZQ.InventorySystem.Runtime.Inventory.Backpack.UI
{
public class InventoryDragGhostView : MonoBehaviour
{
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private Image iconImage;
    [SerializeField] private CanvasGroup canvasGroup;

    private bool rotated;

    public void SetIcon(Sprite icon)
    {
        iconImage.sprite = icon;
    }
    

    public void SetSize(Vector2 size)
    {
        rectTransform.sizeDelta = size;
        SetVisualLayout(size, rotated);
    }

    public Vector2 GetSize()
    {
        return rectTransform.sizeDelta;
    }

    public void SetVisualLayout(Vector2 rootSize, bool rotated)
    {
        this.rotated = rotated;
        if (!rotated)
        {
            iconImage.rectTransform.sizeDelta = new Vector2(rootSize.x, rootSize.y);
        }
        else
        {
            iconImage.rectTransform.sizeDelta = new Vector2(rootSize.y, rootSize.x);
        }
        iconImage.rectTransform.localEulerAngles = new Vector3(0, 0, rotated ? 90 : 0);
    }

    public void SetLocalPosition(Vector2 localPosition)
    {
        rectTransform.localPosition = localPosition;
    }

    public void SetAlpha(float alpha)
    {
        iconImage.color = new Color(iconImage.color.r, iconImage.color.g, iconImage.color.b, alpha);
    }
}
}
