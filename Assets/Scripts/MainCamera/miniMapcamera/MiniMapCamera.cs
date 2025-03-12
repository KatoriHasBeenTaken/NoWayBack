using UnityEngine;

public class MiniMapFollow : MonoBehaviour
{
    public Transform player; // Gán nhân v?t vào ?ây
    public float height = 20f; // ?? cao c?a mini-map camera

    void LateUpdate()
    {
        if (player != null)
        {
            // Camera di chuy?n theo nhân v?t
            Vector3 newPosition = player.position;
            newPosition.y += height;
            transform.position = newPosition;

            // Gi? góc nhìn t? trên xu?ng
            transform.rotation = Quaternion.Euler(90f, player.eulerAngles.y, 0f);
        }
    }
}
