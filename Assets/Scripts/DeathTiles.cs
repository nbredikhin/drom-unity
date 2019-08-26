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
        var tilemapPosition = tilemap.WorldToCell(collider.gameObject.transform.position);
        if (!tilemap.HasTile(tilemapPosition))
            return;
        // var health = collider.gameObject.GetComponent<Health>();
        // if (health == null) return;

        Debug.Log("Kill " + collider.gameObject.name);
        collider.attachedRigidbody.AddForce(-100f * collider.attachedRigidbody.velocity);
    }
}
