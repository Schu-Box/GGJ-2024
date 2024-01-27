using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blob : MonoBehaviour
{
    public List<GameObject> blobShapes;
    public List<GameObject> blobTextures;

    private void Awake()
    {
        int randomShapeIndex = Random.Range(0, blobShapes.Count);
        int randomTextureIndex = Random.Range(0, blobTextures.Count);
        
        for (int i = 0; i < blobShapes.Count; i++)
        {
            if (i == randomShapeIndex)
            {
                blobShapes[i].SetActive(true);
            }
            else
            {
                blobShapes[i].SetActive(false);
            }
        }
        
        for (int i = 0; i < blobTextures.Count; i++)
        {
            if (i == randomTextureIndex)
            {
                blobTextures[i].SetActive(true);
            }
            else
            {
                blobTextures[i].SetActive(false);
            }
        }
    }
}
