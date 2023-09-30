using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridGenerator : MonoBehaviour
{
    [SerializeField]
    private Vector2Int gridDimensions;
    [SerializeField]
    float sizeOfGridQuads;

    [SerializeField]
    GameObject gridQuadPrefab;

    private void Awake()
    {
        this.GenerateGrid();
    }

    private void GenerateGrid()
    {
        
        for (int i = 0; i < this.gridDimensions.y; i++)
        {
            for (int j = 0; j < this.gridDimensions.x; j++)
            {
                GameObject quadInstance = Instantiate(this.gridQuadPrefab, this.transform);
                float xPos = this.transform.position.x + (this.sizeOfGridQuads * j);
                float yPos = this.transform.position.z + (this.sizeOfGridQuads * i);

                quadInstance.transform.localScale = new Vector3(this.sizeOfGridQuads, this.sizeOfGridQuads, this.sizeOfGridQuads);
                quadInstance.transform.localPosition = new Vector3(xPos, 0.0f, yPos);
            }
        }
    }
}
