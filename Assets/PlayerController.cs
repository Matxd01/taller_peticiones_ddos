using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Demo movement (optional)")]
    public float speed = 5f;

    // Used by ApiClient to query position
    public Vector3 GetPosition() => transform.position;

    private void Update()
    {
        // Simple WASD/Arrow movement for testing
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        if (h != 0f || v != 0f)
        {
            Vector3 dir = new Vector3(h, 0f, v);
            transform.Translate(dir * speed * Time.deltaTime, Space.World);
        }
    }
}
