using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Next : MonoBehaviour
{
    public Tile[] nextBlock;
    public GameObject iceBlock;
    public Tilemap tilemap;
    public Animator[] freezingEffect;
    private Vector3Int nextPos = new Vector3Int(0, -2, 0);

    void Start()
    {
        for (int i = 0; i < freezingEffect.Length; i++)
        {
            freezingEffect[i] = iceBlock.GetComponentInChildren<Animator>();
        }
    }

    public void SpawnNext(int[] nextID)
    {
        for (int i = 0; i < 7; i++)
        {
            nextPos.x = i;
            tilemap.SetTile(nextPos, nextBlock[nextID[i]]);
        }
    }

    public void FreezeBlock(bool isFreeze)
    {
        iceBlock.SetActive(isFreeze);
        if (isFreeze && freezingEffect != null)
        {
            foreach (Animator animator in freezingEffect)
            {
                animator.Play("Freezing");
            }
        }
    }
}
