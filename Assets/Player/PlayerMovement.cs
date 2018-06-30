using System;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

[RequireComponent(typeof(ThirdPersonCharacter))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float walkMoveStopRadius = 0.2f;
    [SerializeField] float attackMoveStopRadius = 5f;

    ThirdPersonCharacter m_Character;   // A reference to the ThirdPersonCharacter on the object
    CameraRaycaster cameraRaycaster;
    Vector3 currentDestination, clickPoint;

    bool isInDirectMode = false;

    bool m_Jump;
    private Vector3 m_Move;
    Vector3 m_Cam;
    Vector3 m_CamForward;

    private void Start()
    {
        ProcessRaycasting();
    }

    private void ProcessRaycasting()
    {
        cameraRaycaster = Camera.main.GetComponent<CameraRaycaster>();
        m_Character = GetComponent<ThirdPersonCharacter>();
        currentDestination = transform.position;
    }

    // Fixed update is called in sync with physics
    private void FixedUpdate()
    {
        DecideIfMovementIsDirectOrIndirect();
    }

    private void DecideIfMovementIsDirectOrIndirect()
    {
        if (Input.GetKeyDown(KeyCode.G)) // G for gamepad. TODO add to menu
        {
            isInDirectMode = !isInDirectMode; // toggle mode
            currentDestination = transform.position; // clear the click target
        }

        if (isInDirectMode)
        {
            ProcessDirectMovement();
        }
        else
        {
            ProcessJumpAndCrouchAndMouseMovement();
        }
    }

    private void Update()
    {
        if (!m_Jump)
        {
            m_Jump = Input.GetButtonDown("Jump");
        }
    }

    private void ProcessDirectMovement()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        bool crouch = Input.GetKey(KeyCode.C);

        // calculate move direction to pass to character
        if (m_Cam != null)
        {
            // calculate camera relative direction to move:
            m_CamForward = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;
            m_Move = v * m_CamForward + h * Camera.main.transform.right;
        }
        else
        {
            // we use world-relative directions in the case of no main camera
            m_Move = v * Vector3.forward + h * Vector3.right;
        }
#if !MOBILE_INPUT
        // walk speed multiplier
        if (Input.GetKey(KeyCode.LeftShift)) m_Move *= 0.5f;
#endif

        // pass all parameters to the character control script
        m_Character.Move(m_Move, crouch, m_Jump); // Movement happens here
        m_Jump = false;
    }

    private void ProcessJumpAndCrouchAndMouseMovement()
    {
        if (Input.GetMouseButton(0))
        {
            clickPoint = cameraRaycaster.hit.point;
            switch (cameraRaycaster.currentLayerHit)
            {
                case Layer.Walkable:
                    currentDestination = ShortDestination(clickPoint, walkMoveStopRadius);
                    break;
                case Layer.Enemy:
                    currentDestination = ShortDestination(clickPoint, attackMoveStopRadius);
                    break;
                default:
                    print("Unexpected layer found");
                    return;
            }
        }
        WalkToDestination();
        var playerToClickPoint = currentDestination - transform.position;
        if (playerToClickPoint.magnitude >= 0)
        {
            m_Character.Move(playerToClickPoint, false, false);
        }
        else
        {
            m_Character.Move(Vector3.zero, false, false);
        }
        bool crouch = Input.GetKey(KeyCode.C);
        m_Character.Move(m_Move, crouch, m_Jump); // Movement happens here
        m_Jump = false;
    }

    private void WalkToDestination()
    {
        var playerToClickPoint = currentDestination - transform.position;
        if (playerToClickPoint.magnitude >= 0)
        {
            m_Character.Move(playerToClickPoint, false, false);
        }
        else
        {
            m_Character.Move(Vector3.zero, false, false);
        }
    }

    Vector3 ShortDestination(Vector3 destination, float shortening)
    {
        Vector3 reductionVector = (destination - transform.position).normalized * shortening;
        return destination - reductionVector;
    }

    void OnDrawGizmos()
    {
        // Draw movement gizmos
        Gizmos.color = Color.black;
        Gizmos.DrawLine(transform.position, clickPoint);
        Gizmos.DrawSphere(currentDestination, 0.15f);
        Gizmos.DrawSphere(clickPoint, 0.1f);

        //Draw attac circ (Not protec circ)
        Gizmos.color = new Color(255f, 0f, 0, .5f);
        Gizmos.DrawWireSphere(transform.position, attackMoveStopRadius);

    }
}







