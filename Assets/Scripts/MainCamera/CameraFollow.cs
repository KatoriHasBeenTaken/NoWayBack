using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // Xe cần theo dõi
    public Vector3 khoangCach = new Vector3(0, 3, -6); // Khoảng cách camera so với xe
    public float doMuot = 5f; // Độ mượt khi di chuyển camera
    public float tocDoXoay = 3f; // Tốc độ xoay camera theo xe
    public float gocNhinY = 10f; // Góc nhìn cố định theo trục Y

    void LateUpdate()
    {
        if (target == null) return;

        // Tính toán vị trí mong muốn của camera
        Vector3 desiredPosition = target.position + target.TransformDirection(khoangCach);
        transform.position = Vector3.Lerp(transform.position, desiredPosition, doMuot * Time.deltaTime);

        // Giữ góc nhìn ngang, không để camera bị xoay lên trời
        Vector3 lookDirection = target.position - transform.position;
        lookDirection.y = 0; // Giữ camera ngang
        Quaternion targetRotation = Quaternion.LookRotation(lookDirection) * Quaternion.Euler(gocNhinY, 0, 0);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, tocDoXoay * Time.deltaTime);
    }
}