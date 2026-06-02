using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Camera))]
public class MouseGrabber : MonoBehaviour
{
    [SerializeField] float maxDistance = 3f;
    [SerializeField] LayerMask grabbableLayer = ~0;
    [SerializeField] Transform holdPoint;
    [SerializeField] float attachForce = 600f;

    Rigidbody grabbedRb;
    FixedJoint joint;

    Camera cam;

    void Awake()
    {
        cam = GetComponent<Camera>();
        if (holdPoint == null)
        {
            GameObject hp = new GameObject("HoldPoint");
            hp.transform.SetParent(transform);
            hp.transform.localPosition = new Vector3(0f, 0f, 1.5f);
            holdPoint = hp.transform;
        }
    }

    void Update()
    {
        if (Mouse.current == null) return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            TryGrab();
        }
        else if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            Release();
        }
    }

    void TryGrab()
    {
        if (grabbedRb != null) return;

        Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, grabbableLayer))
        {
            Rigidbody rb = hit.rigidbody;
            if (rb == null) return;

            grabbedRb = rb;
            joint = grabbedRb.gameObject.AddComponent<FixedJoint>();
            joint.connectedBody = null;
            joint.breakForce = Mathf.Infinity;
            joint.breakTorque = Mathf.Infinity;

            // create a kinematic holder rigidbody to connect to for more stable holding
            GameObject holder = new GameObject("GrabHolder");
            holder.transform.position = holdPoint.position;
            holder.transform.rotation = holdPoint.rotation;
            Rigidbody holderRb = holder.AddComponent<Rigidbody>();
            holderRb.isKinematic = true;

            joint.connectedBody = holderRb;
            // optional: reduce gravity while held
            grabbedRb.useGravity = false;
        }
    }

    void Release()
    {
        if (grabbedRb == null) return;

        if (joint != null)
        {
            Rigidbody holder = joint.connectedBody;
            Destroy(joint);
            joint = null;
            if (holder != null) Destroy(holder.gameObject);
        }

        // re-enable physics
        grabbedRb.useGravity = true;
        grabbedRb = null;
    }

    void LateUpdate()
    {
        if (joint != null && joint.connectedBody != null)
        {
            joint.connectedBody.transform.position = holdPoint.position;
            joint.connectedBody.transform.rotation = holdPoint.rotation;
        }
    }
}