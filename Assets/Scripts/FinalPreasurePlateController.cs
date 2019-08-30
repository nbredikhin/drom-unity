using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalPreasurePlateController : MonoBehaviour
{
    public List<EnemyController> Goblins;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void FinalizeAll()
    {
        gameObject.GetComponent<BoxCollider2D>().enabled = false;

        foreach (var enemy in Goblins)
        {
            enemy.gameObject.SetActive(true);
        }
    }
}
