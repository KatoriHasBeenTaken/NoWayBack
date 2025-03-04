using UnityEngine;

public class CarController : MonoBehaviour
{
    public float acceleration = 3000f; // Lực tăng tốc hợp lý hơn
    public float maxSpeed = 80f; // Tốc độ tối đa (thực tế hơn)
    public float turnSpeed = 1.5f; // Giảm xoay để xe vào cua mượt
    public float driftFactor = 0.95f; // Giữ độ bám đường khi drift nhẹ
    public float brakingForce = 5000f; // Lực phanh vừa đủ để xe trượt nhẹ

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = new Vector3(0, -0.8f, 0); // Trọng tâm thấp để xe ổn định
        rb.drag = 0.5f; // Giá trị 0.5 giúp xe dừng dần tự nhiên hơn
    }

    void FixedUpdate()
    {
        float moveInput = Input.GetAxis("Vertical"); // W/S hoặc mũi tên lên/xuống
        float turnInput = Input.GetAxis("Horizontal"); // A/D hoặc mũi tên trái/phải

        // Nếu có đầu vào từ người chơi, tăng tốc
        if (Mathf.Abs(moveInput) > 0.1f)
        {
            Vector3 forwardForce = transform.forward * moveInput * acceleration * Time.deltaTime;
            if (rb.velocity.magnitude < maxSpeed)
            {
                rb.AddForce(forwardForce, ForceMode.Acceleration);
            }
        }
        else
        {
            // Khi không có đầu vào, giảm tốc từ từ
            rb.velocity *= 0.98f; // Giúp xe dừng lại tự nhiên hơn
        }

        // Giới hạn tốc độ tối đa
        if (rb.velocity.magnitude > maxSpeed)
        {
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }

        // Quay xe chỉ khi đang di chuyển
        if (Mathf.Abs(moveInput) > 0.1f)
        {
            rb.MoveRotation(rb.rotation * Quaternion.Euler(0f, turnInput * turnSpeed * (rb.velocity.magnitude / maxSpeed), 0f));
        }

        // Áp dụng drift nhẹ
        Vector3 driftForce = rb.velocity - transform.forward * Vector3.Dot(rb.velocity, transform.forward);
        rb.velocity -= driftForce * (1 - driftFactor) * Time.deltaTime;

        // Phanh trượt khi nhấn phím Space
        if (Input.GetKey(KeyCode.Space))
        {
            rb.AddForce(-rb.velocity.normalized * brakingForce * Time.deltaTime, ForceMode.Acceleration);
        }
    }
}
