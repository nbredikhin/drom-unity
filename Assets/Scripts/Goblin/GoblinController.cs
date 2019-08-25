using UnityEngine;

public class GoblinController : MonoBehaviour
{
    [Header("Movement Stats")]
    [Range(1, 500)] public float MOVEMENT_BASE_SPEED;
    [Space(10)]

    [Header("References")]
    public Rigidbody2D rb;
    private GameObject _target;

    private void Awake()
    {
        if (rb == null)
        {
            rb = gameObject.GetComponent<Rigidbody2D>();
        }
        _target = GameObject.Find("Player");
    }

    private void Update()
    {
        if (_target == null)
        {
            return;
        }

        var direction = _target.transform.position - this.transform.position;
        if (direction.magnitude > 0.15f)
        {
            rb.velocity = direction.normalized * MOVEMENT_BASE_SPEED * Time.deltaTime;
        }
        else
        {
            rb.velocity = Vector2.zero;
        }
    }
}
