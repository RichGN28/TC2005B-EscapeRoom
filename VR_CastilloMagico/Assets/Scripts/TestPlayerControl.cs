using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class TestPlayerControl : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float moveSpeed = 4.5f;

    [Header("Look")]
    [SerializeField] Transform cameraRoot;
    [SerializeField] float mouseSensitivity = 0.12f;
    [SerializeField] float minPitch = -80f;
    [SerializeField] float maxPitch = 80f;
    [SerializeField] bool lockCursorOnStart = true;

    Rigidbody playerRigidbody;
    Vector3 movementInput;
    float pitch;

    void Awake()
    {
        playerRigidbody = GetComponent<Rigidbody>();
        playerRigidbody.constraints |= RigidbodyConstraints.FreezeRotation;

        if (cameraRoot == null && Camera.main != null)
        {
            cameraRoot = Camera.main.transform;
        }
    }

    void Start()
    {
        if (lockCursorOnStart)
        {
            LockCursor(true);
        }
    }

    void Update()
    {
        HandleCursorToggle();
        HandleLook();

        Vector2 input = Vector2.zero;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed)
            {
                input.y += 1f;
            }

            if (Keyboard.current.sKey.isPressed)
            {
                input.y -= 1f;
            }

            if (Keyboard.current.dKey.isPressed)
            {
                input.x += 1f;
            }

            if (Keyboard.current.aKey.isPressed)
            {
                input.x -= 1f;
            }
        }

        input = Vector2.ClampMagnitude(input, 1f);
        movementInput = new Vector3(input.x, 0f, input.y) * moveSpeed;
    }

    void HandleCursorToggle()
    {
        if (Keyboard.current == null)
        {
            return;
        }

        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            LockCursor(Cursor.lockState != CursorLockMode.Locked);
        }
    }

    void HandleLook()
    {
        if (Cursor.lockState != CursorLockMode.Locked || Mouse.current == null)
        {
            return;
        }

        Vector2 lookDelta = Mouse.current.delta.ReadValue() * mouseSensitivity;

        transform.Rotate(Vector3.up * lookDelta.x);

        pitch = Mathf.Clamp(pitch - lookDelta.y, minPitch, maxPitch);

        if (cameraRoot != null)
        {
            cameraRoot.localRotation = Quaternion.Euler(pitch, 0f, 0f);
        }
    }

    void FixedUpdate()
    {
        if (playerRigidbody == null)
        {
            return;
        }

        Vector3 forward = transform.forward;
        Vector3 right = transform.right;

        if (cameraRoot != null)
        {
            forward = Vector3.ProjectOnPlane(cameraRoot.forward, Vector3.up).normalized;
            right = Vector3.ProjectOnPlane(cameraRoot.right, Vector3.up).normalized;
        }

        Vector3 cameraRelativeMovement = (right * movementInput.x) + (forward * movementInput.z);
        playerRigidbody.MovePosition(playerRigidbody.position + cameraRelativeMovement * Time.fixedDeltaTime);
    }

    void LockCursor(bool locked)
    {
        Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !locked;
    }
}
