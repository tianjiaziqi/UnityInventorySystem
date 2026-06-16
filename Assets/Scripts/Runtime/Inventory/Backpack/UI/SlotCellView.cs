using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace JZQ.InventorySystem.Runtime.Inventory.Backpack.UI
{
public class SlotCellView : MonoBehaviour
{
    [SerializeField] private Image backgroundImage;
    private Sprite unlockedSprite;
    private Sprite lockedSprite;
    private Sprite previewValidSprite;
    private Sprite previewInvalidSprite;

    public int X { get; private set; }
    public int Y { get; private set; }

    private bool unlocked;

    public void Init(int x, int y, Sprite unlockedIcon, Sprite lockedIcon, Sprite previewValidIcon, Sprite previewInvalidIcon )
    {
        X = x;
        Y = y;
        unlockedSprite = unlockedIcon;
        lockedSprite = lockedIcon;
        previewInvalidSprite = previewInvalidIcon;
        previewValidSprite = previewValidIcon;
    }

    public void SetUnlocked(bool isUnlocked)
    {
        unlocked = isUnlocked;
        Sprite targetSprite = unlocked ? unlockedSprite : lockedSprite;

        if (backgroundImage.sprite != targetSprite)
        {
            backgroundImage.sprite = targetSprite;
        }
    }

    public void ShowPreview(bool valid)
    {
        backgroundImage.sprite = valid ? previewValidSprite : previewInvalidSprite;
    }

    public void ClearPreview()
    {
        backgroundImage.sprite = unlocked ? unlockedSprite : lockedSprite;   
    }
}
}
