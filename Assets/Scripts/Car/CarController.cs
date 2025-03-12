using UnityEngine;

public class CarController : MonoBehaviour
{
    public WheelCollider frontLeftWheel, frontRightWheel, rearLeftWheel, rearRightWheel;
    public Transform frontLeftMesh, frontRightMesh, rearLeftMesh, rearRightMesh;

    public float motorTorque = 150f; // Tăng lực truyền động
    public float maxSteeringAngle = 30f; // Góc quay tối đa
    public float brakingForce = 30f; // Giảm lực phanh
    public float driftFactor = 0.6f; // Tăng hệ số drift
    public float rearDriftDelay = 0.1f; // Giảm độ trễ khi drift bánh sau
    public float dragFactor = 1.5f; // Giảm lực cản không khí
    public float angularDragFactor = 1.8f; // Giảm xoay xe quá mức
    public float engineBrakingForce = 70f; // Phanh động cơ khi buông ga
    public float tiltAngleMax = 15f; // Tăng góc nghiêng tối đa khi vào cua
    public float maxSpeed = 10f; // Tăng giới hạn tốc độ tối đa

    private bool isDrifting = false;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.drag = dragFactor; // Cản không khí, giúp xe chậm lại tự nhiên
        rb.angularDrag = angularDragFactor; // Giảm xoay quá mức
    }

    void FixedUpdate()
    {
        float moveInput = Input.GetAxis("Vertical");   // W/S hoặc mũi tên lên/xuống
        float steerInput = Input.GetAxis("Horizontal"); // A/D hoặc mũi tên trái/phải

        // Kiểm tra nếu đang drift
        isDrifting = Input.GetKey(KeyCode.Space);

        // Giới hạn tốc độ tối đa
        if (rb.velocity.magnitude > maxSpeed)
        {
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }

        // Truyền lực vào bánh sau
        float currentMotorTorque = isDrifting ? motorTorque * 0.7f : motorTorque;
        rearLeftWheel.motorTorque = moveInput * currentMotorTorque;
        rearRightWheel.motorTorque = moveInput * currentMotorTorque;

        // Điều khiển hướng bánh trước
        float steerAngle = maxSteeringAngle * steerInput;
        frontLeftWheel.steerAngle = steerAngle;
        frontRightWheel.steerAngle = steerAngle;

        // Phanh chỉ tác động lên bánh trước khi nhấn Space (giống GTA V)
        if (isDrifting)
        {
            frontLeftWheel.brakeTorque = brakingForce;
            frontRightWheel.brakeTorque = brakingForce;
            rearLeftWheel.brakeTorque = 0;
            rearRightWheel.brakeTorque = 0;
        }
        else
        {
            frontLeftWheel.brakeTorque = 0;
            frontRightWheel.brakeTorque = 0;
        }

        // Phanh động cơ khi buông ga
        if (moveInput == 0 && rb.velocity.magnitude > 0.1f)
        {
            rearLeftWheel.brakeTorque = engineBrakingForce * 0.2f; // Tăng lực phanh động cơ để xe chậm lại nhanh hơn
            rearRightWheel.brakeTorque = engineBrakingForce * 0.2f;
        }
        else
        {
            rearLeftWheel.brakeTorque = 0;
            rearRightWheel.brakeTorque = 0;
        }

        // Giảm ma sát ngang khi drift
        AdjustDrift(rearLeftWheel, isDrifting);
        AdjustDrift(rearRightWheel, isDrifting);

        // Hiệu ứng nghiêng xe khi vào cua
        float tiltAngle = Mathf.Lerp(0, tiltAngleMax, Mathf.Abs(steerInput));
        Quaternion targetTilt = Quaternion.Euler(0, transform.rotation.eulerAngles.y, -steerInput * tiltAngle);
        rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetTilt, Time.deltaTime * 2f));

        // Cập nhật vị trí và xoay của bánh xe
        UpdateWheelPose(frontLeftWheel, frontLeftMesh);
        UpdateWheelPose(frontRightWheel, frontRightMesh);
        UpdateWheelPose(rearLeftWheel, rearLeftMesh);
        UpdateWheelPose(rearRightWheel, rearRightMesh);
    }

    void UpdateWheelPose(WheelCollider collider, Transform mesh)
    {
        Vector3 pos;
        Quaternion rot;
        collider.GetWorldPose(out pos, out rot);
        mesh.position = pos;
        mesh.rotation = rot;
    }

    void AdjustDrift(WheelCollider wheel, bool drifting)
    {
        WheelFrictionCurve friction = wheel.sidewaysFriction;
        friction.stiffness = drifting ? 0.3f : 1.0f; // Giảm độ cứng khi drift
        wheel.sidewaysFriction = friction;
    }
}