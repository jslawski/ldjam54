using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridQuadObject : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer sprite;

    public void UpdateSprite(Sprite newSprite)
    {
        if (newSprite == this.sprite.sprite)
        {
            return;
        }

        this.sprite.sprite = newSprite;
    }
}
