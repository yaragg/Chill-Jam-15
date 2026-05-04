using System.Collections.Generic;
using UnityEngine;

public class SpriteRandomizer : MonoBehaviour
{
    public List<Sprite> sprites;
    private SpriteRenderer spriteRenderer;

    private Sprite _originalSprite;

    void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _originalSprite = spriteRenderer.sprite;
        spriteRenderer.sprite = Utils.GetRandomItem(sprites);
    }

    public void RestoreOriginalSprite ()
    {
        spriteRenderer.sprite = _originalSprite;
    }

    public void SetFromIndex (int index)
    {
        spriteRenderer.sprite = sprites[index];
    }
}
