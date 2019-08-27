using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinArrow : MonoBehaviour
{
    bool isShooting = false;

    public float TravelSpeed = 1.0f;
    public float Damage = 10.0f;
    public float KnockBackForce = 20.0f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!isShooting)
        {
            return;
        }
    }

    void LaunchAttack(Vector2 direction)
    {
        if (isShooting)
        {
            return;
        }

        isShooting = true;
        transform.Rotate(0, 0, 90);
        transform.parent.SendMessage("DoneShooting");
        transform.position = transform.parent.position;
        transform.parent = null;
        GetComponent<Rigidbody2D>().velocity = direction.normalized * TravelSpeed;
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        Debug.Log(collider.name);

        collider.SendMessage("DecreaseHealth", Damage, SendMessageOptions.DontRequireReceiver);
        collider.SendMessage("KnockBack",
                             new Vector2(transform.up.x, transform.up.y) * KnockBackForce,
                             SendMessageOptions.DontRequireReceiver);
        Destroy(gameObject);
    }
}
