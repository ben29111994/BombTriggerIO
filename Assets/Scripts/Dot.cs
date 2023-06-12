using UnityEngine;

public class Dot : MonoBehaviour
{
    Rigidbody rigidbody;
    int currentScene;

    void OnEnable()
    {
        rigidbody = GetComponent<Rigidbody>();
        transform.rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
    }

    void Update()
    {
        if (rigidbody.velocity.magnitude < 3)
            rigidbody.AddRelativeForce(Vector3.forward * 1);
    }
}
