using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwingableSword : MonoBehaviour
{
    public float SwordLength = 1.0f;

    public float SwingAngleDeg = 90;

    float swingStart;
    float swingEnd;


    float swingStartTimestamp;
    float swingPoint;
    public float SwingSpeed = 0.1f;

    bool isShooting = false;

    public float Damage = 10.0f;
    public float KnockBackForce = 20.0f;

    // Start is called before the first frame update
    void Start()
    {
        swingPoint = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isShooting)
        {
            return;
        }

        if (Time.time - swingStartTimestamp >= SwingSpeed)
        {
            transform.parent.SendMessage("DoneShooting");
            Destroy(gameObject);
            return;
        }
        else
        {
            var q = Quaternion.Euler(0, 0, Mathf.LerpAngle(swingStart, swingEnd, swingPoint / SwingSpeed));
            transform.rotation = q;
            swingPoint += Time.deltaTime;
        }
    }

    void LaunchAttack(Vector2 direction)
    {
        if (isShooting)
        {
            return;
        }

        swingStartTimestamp = Time.time;

        swingStart = transform.rotation.eulerAngles.z - SwingAngleDeg / 2.0f;
        swingEnd = swingStart + SwingAngleDeg;

        isShooting = true;
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        collider.SendMessage("DecreaseHealth", Damage, SendMessageOptions.DontRequireReceiver);
        collider.SendMessage("KnockBack",
                             new Vector2(transform.up.x, transform.up.y) * KnockBackForce,
                             SendMessageOptions.DontRequireReceiver);
    }
}
