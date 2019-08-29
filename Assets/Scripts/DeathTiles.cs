using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DeathTiles : MonoBehaviour
{
    private Tilemap tilemap;

    private void Awake()
    {
        tilemap = GetComponent<Tilemap>();
    }

    private void OnTriggerStay2D(Collider2D collider)
    {
        if (tilemap != null)
        {
            var tilemapPosition = tilemap.WorldToCell(collider.gameObject.transform.position);
            if (!tilemap.HasTile(tilemapPosition))
                return;
        }

        collider.gameObject.SendMessage("DecreaseHealth", 10000.0f);
    }
}
