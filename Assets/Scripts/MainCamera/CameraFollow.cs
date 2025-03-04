using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // Xe cần theo dõi
    public Vector3 khoangCach = new Vector3(0, 5, -10); // Khoảng cách camera so với xe
    public float doMuot = 5f; // Độ mượt khi di chuyển camera

    void LateUpdate()
    {
        if (target == null) return;

        // Vị trí mong muốn của camera
        Vector3 desiredPosition = target.position + target.TransformDirection(khoangCach);

        // Di chuyển camera một cách mượt mà
        transform.position = Vector3.Lerp(transform.position, desiredPosition, doMuot * Time.deltaTime);

        // Camera luôn nhìn về phía xe
        transform.LookAt(target.position + Vector3.up * 2f);
    }
}
