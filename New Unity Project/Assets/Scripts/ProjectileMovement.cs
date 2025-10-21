using UnityEngine;
using Unity.Netcode;

public class ProjectileMovement : NetworkBehaviour
{
    public float projectileSpeed;
    void Start()
    {
        projectileSpeed = 3.0f;
    }

    void FixedUpdate()
    {
        GetComponent<Rigidbody2D>().AddForce(transform.right * projectileSpeed);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "TargetPracticePoint")
        {
            GetComponent<NetworkObject>().Despawn(true);
            Destroy(this);
        }
    }
}
