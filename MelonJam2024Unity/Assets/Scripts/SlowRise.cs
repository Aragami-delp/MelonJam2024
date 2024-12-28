using UnityEngine;

public class SlowRise : MonoBehaviour
{
    [SerializeField]
    float speed = 0.05f;

    private void FixedUpdate()
    {
        transform.position += Vector3.up * speed * Time.fixedDeltaTime;         
    }
}
