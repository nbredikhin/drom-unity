using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwingableSword : MonoBehaviour
{
    public float SwordLength = 1.0f;

    public float SwingAngleDeg = 90;

    Vector2 swingDirection;

    float swingStartTimestamp;
    Vector2 swingStart;
    Vector2 swingEnd;
    float swingPoint;
    public float SwingSpeed = 0.1f;

    bool isShooting = false;

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

        if (Time.realtimeSinceStartup - swingStartTimestamp >= SwingSpeed)
        {
            transform.parent.SendMessage("DoneShooting");
            Destroy(gameObject);
            return;
        }
        else
        {
            swingPoint += Time.deltaTime;
            Vector2 path = Vector2.Lerp(swingStart, swingEnd, swingPoint / SwingSpeed) * SwordLength;

            DrawDebugSword(path);
        }
    }

    void FixedUpdate()
    {
        Vector2 path = Vector2.Lerp(swingStart, swingEnd, swingPoint / SwingSpeed) * SwordLength;
        var hit = Physics2D.Raycast(transform.parent.position, path.normalized, SwordLength, LayerMask.GetMask("Goblins"));
        if (hit.collider != null)
        {
            hit.collider.gameObject.SendMessage("DecreaseHealth", 50);
        }
    }

    void LaunchAttack(Vector2 direction)
    {
        if (isShooting)
        {
            return;
        }

        swingDirection = direction.normalized;
        swingStartTimestamp = Time.realtimeSinceStartup;

        swingStart = Quaternion.Euler(0, 0, -SwingAngleDeg / 2) * swingDirection;
        swingEnd = Quaternion.Euler(0, 0, SwingAngleDeg / 2) * swingDirection;

        isShooting = true;
    }

    void DrawDebugSword(Vector2 swordPos)
    {
        Debug.DrawLine(transform.parent.position,
            transform.parent.position + new Vector3(swordPos.x, swordPos.y, 0),
            Color.blue);
        Debug.DrawLine(transform.parent.position,
            transform.parent.position + new Vector3(swingStart.x, swingStart.y, 0) * SwordLength,
            Color.red);
        Debug.DrawLine(transform.parent.position,
            transform.parent.position + new Vector3(swingEnd.x, swingEnd.y, 0) * SwordLength,
            Color.green);
    }
}
