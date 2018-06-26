using System;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

[RequireComponent(typeof(ThirdPersonCharacter))]
public class PlayerMovement : MonoBehaviour
{


    [SerializeField] float walkMoveStopRadius = 0.5f;

    ThirdPersonCharacter m_Character;   // A reference to the ThirdPersonCharacter on the object
    CameraRaycaster cameraRaycaster;
    Vector3 currentClickTarget;
    bool m_Jump;
    private Vector3 m_Move;
    Vector3 m_Cam;
    Vector3 m_CamForward;
    bool isInDirectMode = false; 


    private void Start()
    {
        ProcessRaycasting();
    }


    private void ProcessRaycasting()
    {
        cameraRaycaster = Camera.main.GetComponent<CameraRaycaster>();
        m_Character = GetComponent<ThirdPersonCharacter>();
        currentClickTarget = transform.position;
    }


    // Fixed update is called in sync with physics
    private void FixedUpdate()
    {
        DecideIfMovementIsDirectOrIndirect();
        
    }


    private void DecideIfMovementIsDirectOrIndirect()
    {
        if (Input.GetKeyDown(KeyCode.G)) //G for Gamepad. //TODO add to menu
        {
            isInDirectMode = !isInDirectMode; //toggle mode
            currentClickTarget = transform.position; //clear the click target
        }

        if (isInDirectMode)
        {
            ProcessDirectMovement();
        }
        else
        {
            ProcessMouseMovement();
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
        // read inputs
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

    private void ProcessMouseMovement()
    {
        if (Input.GetMouseButton(0))
        {
            print("Cursor raycast hit layer: " + cameraRaycaster.layerHit);
            switch (cameraRaycaster.layerHit)
            {
                case Layer.Walkable:
                    currentClickTarget = cameraRaycaster.hit.point;
                    break;
                case Layer.Enemy:
                    print("Not moving to enemy!");
                    break;
                default:
                    print("Unexpected layer found");
                    return;
            }

        }
        var playerToClickPoint = currentClickTarget - transform.position;
        if (playerToClickPoint.magnitude >= walkMoveStopRadius)
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

  
}

    
        

