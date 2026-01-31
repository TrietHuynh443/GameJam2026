using UnityEngine;

public class Drift : MonoBehaviour
{
    public float speed = 1f;

    // X position where the cloud resets
    public float leftX = -12f;
    public float rightX = 12f;

    void Update()
    {
        transform.Translate(Vector3.right * (speed * Time.deltaTime));

        if (transform.position.x > rightX)
        {
            transform.position = new Vector3(
                leftX,
                transform.position.y,
                transform.position.z
            );
        }
    }
}
