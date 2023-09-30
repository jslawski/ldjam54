using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPaint : MonoBehaviour
{
    [SerializeField]
    private Sprite teamSprite;
    
    private void OnTriggerEnter(Collider other)
    {        
        other.gameObject.GetComponentInParent<GridQuadObject>().UpdateSprite(this.teamSprite);
    }
}
