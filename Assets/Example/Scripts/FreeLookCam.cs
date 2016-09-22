using UnityEngine;

public class FreeLookCam : MonoBehaviour
{
    //Based on the FreeLookCam script form StandardAssets.

    // This script is designed to be placed on the root object of a camera rig,
    // comprising 3 gameobjects, each parented to the next:

    // 	Camera Rig
    // 		Pivot
    // 			Camera

    [SerializeField]  private float m_MoveSpeed = 1f;                     // How fast the rig will move to keep up with the target's position.
    [Range(0f, 10f)] [SerializeField] private float m_TurnSpeed = 1.5f;   // How fast the rig will rotate from user input.
    [SerializeField] private float m_TurnSmoothing = 0.0f;                // How much smoothing to apply to the turn input, to reduce mouse-turn jerkiness
    [SerializeField] private float m_TiltMax = 89f;                       // The maximum value of the x axis rotation of the pivot.
    [SerializeField] private float m_TiltMin = 89f;                       // The minimum value of the x axis rotation of the pivot.
    [SerializeField] private float m_MinCameraDistance = 1f;              // The minimum distance to the target.
    [SerializeField] private float m_MaxCameraDistance = 20f;             // The maximum distance to the target.

    [SerializeField] private Rigidbody m_Target;

    private Transform m_Cam;                      // the transform of the camera
    private Transform m_Pivot;                    // the point at which the camera pivots around
    private float m_LookAngle;                    // The rig's y axis rotation.
    private float m_TiltAngle;                    // The pivot's x axis rotation.
    private float m_CamDistance;                  
    private Vector3 m_PivotEulers;
    private Quaternion m_PivotTargetRot;
    private Quaternion m_TransformTargetRot;
    private bool m_InputEnabled;
    private Vector3 m_LastTargetPosition;

    void Awake()
    {
        // find the camera in the object hierarchy
        m_Cam = GetComponentInChildren<Camera>().transform;
        m_Pivot = m_Cam.parent;

        m_PivotEulers = m_Pivot.rotation.eulerAngles;

        m_PivotTargetRot = m_Pivot.transform.localRotation;
        m_TransformTargetRot = transform.localRotation;

        m_CamDistance = m_Cam.localPosition.z;
    }

    protected void Update()
    {
        m_InputEnabled = Input.GetMouseButton(1);
        HandleRotationMovement();
        HandleZoom(Time.deltaTime);
    }

    void FixedUpdate()
    {
        FollowTarget(Time.fixedDeltaTime);
    }

    protected void FollowTarget(float deltaTime)
    {
        if (m_Target == null) return;

        Vector3 targetPos =  m_Target.worldCenterOfMass;            

        // Move the rig towards target position.
        transform.position = Vector3.Lerp(transform.position, targetPos, deltaTime * m_MoveSpeed);
    }

    private void HandleZoom(float deltaTime)
    {
        var scroll = Input.mouseScrollDelta;

        m_CamDistance += scroll.y;

        m_CamDistance = Mathf.Clamp(m_CamDistance, -m_MaxCameraDistance, -m_MinCameraDistance);

        Vector3 camPos = new Vector3(m_Cam.localPosition.x, m_Cam.localPosition.y, m_CamDistance);

        m_Cam.localPosition = Vector3.Lerp(m_Cam.localPosition, camPos, deltaTime * m_MoveSpeed);
    }

    private void HandleRotationMovement()
    {
        if (Time.timeScale < float.Epsilon)
            return;

        // Read the user input
        var x = Input.GetAxis("Mouse X");
        var y = Input.GetAxis("Mouse Y");

        if (m_InputEnabled == false)
        {
            x = y = 0;
        }

        // Adjust the look angle by an amount proportional to the turn speed and horizontal input.
        m_LookAngle += x * m_TurnSpeed;

        // Rotate the rig (the root object) around Y axis only:
        m_TransformTargetRot = Quaternion.Euler(0f, m_LookAngle, 0f);

        // on platforms with a mouse, we adjust the current angle based on Y mouse input and turn speed
        m_TiltAngle -= y * m_TurnSpeed;
        // and make sure the new value is within the tilt range
        m_TiltAngle = Mathf.Clamp(m_TiltAngle, -m_TiltMin, m_TiltMax);
            

        // Tilt input around X is applied to the pivot (the child of this object)
        m_PivotTargetRot = Quaternion.Euler(m_TiltAngle, m_PivotEulers.y, m_PivotEulers.z);

        if (m_TurnSmoothing > 0)
        {
            m_Pivot.localRotation = Quaternion.Slerp(m_Pivot.localRotation, m_PivotTargetRot, m_TurnSmoothing * Time.deltaTime);
            transform.localRotation = Quaternion.Slerp(transform.localRotation, m_TransformTargetRot, m_TurnSmoothing * Time.deltaTime);
        }
        else
        {
            m_Pivot.localRotation = m_PivotTargetRot;
            transform.localRotation = m_TransformTargetRot;
        }
    }

    public void SetTarget(Rigidbody target)
    {
        m_Target = target;
    }
}
