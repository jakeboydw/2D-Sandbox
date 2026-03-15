using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Range(0, 1)]
    public float smoothTime;
    public Transform player;

    public void Spawn(Vector3 pos)
    {
        transform.position = pos;
    }

    private void FixedUpdate()
    {
        Vector3 pos = transform.position;

        pos.x = Mathf.Lerp(pos.x, player.position.x, smoothTime);
        pos.y = Mathf.Lerp(pos.y, player.position.y, smoothTime);

        transform.position = pos;
    }
}
