using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    void Update()
    {
        transform.position = new Vector3(Mathf.PingPong(Time.time * 2, 10),
        transform.position.y, transform.position.z);

    }
}
