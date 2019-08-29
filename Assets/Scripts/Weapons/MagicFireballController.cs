using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicFireballController : MonoBehaviour
{
    bool isShooting = false;

    public float TravelSpeed = 1.0f;
    GameObject shooter;

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

       //    transform.Translate(velocity.x * Time.deltaTime, velocity.y * Time.deltaTime, 0, Space.World);
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

        shooter = transform.parent.gameObject;

        transform.parent = null;
        if (direction == Vector2.zero)
        {
            direction = Vector2.up;
        }
        GetComponent<Rigidbody2D>().velocity = direction.normalized * TravelSpeed;
    }

    void SwapShooterWithGameObject(GameObject target)
    {
        (target.transform.position, shooter.transform.position) = (shooter.transform.position, target.transform.position);
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (((1 << collider.gameObject.layer) & LayerMask.GetMask("DeathTiles", "Level")) == 0)
        {
            SwapShooterWithGameObject(collider.gameObject);  
        }
        isShooting = false;
        Destroy(gameObject);
    }
}
