using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : BaseRunner
{
    #region Unity Attributes
    public GameObject ConfettiCelebration;
    private Camera m_MainCamera;
    #endregion Unity Attributes

    #region Attributes
    private Vector2 m_MousePosition;
    private float m_DeltaX;
    private Vector3 m_NextPosition;

    private enum PlayerStates
    {
        Idle, 
        Moving
    };
    private PlayerStates m_MyState = PlayerStates.Idle;

    private enum TouchStates
    {
        Idle,
        Touched,
        Released
    };
    private TouchStates m_MyTouchState = TouchStates.Idle;

    private float m_ZDistanceToFinish;
    private Vector3 m_MyVelocity;
    #endregion Attributes

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
        m_MainCamera = Camera.main;
        m_MyState = PlayerStates.Idle;
        m_Speed = 14f;
        Type = "Player";
    }

    protected override void OnCountDownFinish()
    {
        m_MyState = PlayerStates.Moving;
        m_ZDistanceToFinish = GameController.Instance.RaceFinishLineZ - m_MyTransform.position.z;

        ParticleSystem[] m_Ps = ConfettiCelebration.GetComponents<ParticleSystem>();
        foreach (ParticleSystem p in m_Ps)
            p.Play();
    }

    protected override void OnRaceFinishEvent()
    {
        m_MyState = PlayerStates.Idle;
    }

    public override void AfterSortingRunners()
    {
        UIController.Instance.UpdatePlayerPlace(PlaceInTheRace);
    }

    // Update is called once per frame
    void Update()
    {
        if (m_MyState == PlayerStates.Moving)
        {
            if (Input.GetMouseButtonDown(0)) 
            {
                m_MousePosition = m_MainCamera.ScreenToViewportPoint(Input.mousePosition);
                m_MyTouchState = TouchStates.Touched;
            }

            if (Input.GetMouseButton(0)) 
            {
                if (m_MainCamera.ScreenToViewportPoint(Input.mousePosition).x < m_MousePosition.x)
                {
                    if (m_DeltaX > -1)
                    {
                        m_DeltaX -= 0.1f;
                    }
                }
                else if (m_MainCamera.ScreenToViewportPoint(Input.mousePosition).x > m_MousePosition.x)
                {
                    if (m_DeltaX < 1)
                    {
                        m_DeltaX += 0.1f;
                    }
                }

                m_MousePosition = m_MainCamera.ScreenToViewportPoint(Input.mousePosition);
            }

            if (Input.GetMouseButtonUp(0))
            {
                m_MyTouchState = TouchStates.Released;
            }

            UIController.Instance.UpdateRaceProgress(m_ZDistanceToFinish - GameController.Instance.RaceFinishLineZ + m_MyTransform.position.z / m_ZDistanceToFinish);
        }
    }

    protected override void Move()
    {
        if (m_MyState == PlayerStates.Moving)
        {
            if (m_MyTouchState == TouchStates.Released)
            {
                if (m_DeltaX > 0)
                {
                    m_DeltaX -= 0.02f; // Change X cordinate from 0.02 points. 0.02 is taken because Time.fixedDelta time is 0.02. 
                }
                else if (m_DeltaX < 0)
                {
                    m_DeltaX += 0.02f;
                }
                else
                {
                    m_DeltaX = 0;
                    m_MyTouchState = TouchStates.Idle;
                }
            }

            m_MyVelocity = m_NextPosition;
            m_NextPosition = m_MyTransform.position + new Vector3(m_DeltaX, 0, 1) * Time.deltaTime * m_Speed;
            m_NextPosition.x = Mathf.Clamp(m_NextPosition.x, -9, 9);

            m_MyRigidBody.MovePosition(m_NextPosition);
        }
    }

    protected override void OnCollisionEnter(Collision collision)
    {
        base.OnCollisionEnter(collision);

        if (collision.gameObject.tag == "NPCPlayer")
        {
            if(GameController.Instance.IsTypeContainsInWord(collision.gameObject.GetComponent<BaseRunner>().Type)) // True if the collided NPC has stolen letter
            {
                GameController.Instance.AddCoins(3);
                AudioController.Instance.PlayEffectAudio("LetterFound");
                collision.gameObject.GetComponent<BaseRunner>().AfterCollidedWithAnObstacle(Vector3.zero);
            }
            else
            {
                //AfterCollidedWithAnObstacle(Vector3.zero);
            }
        }
        else if(collision.gameObject.tag == "Outside") // For Half platforms, when player moved outside. 
        {
            Vector3 m_ForceDirection = (m_NextPosition - m_MyVelocity) * m_Speed * Time.fixedDeltaTime;
            m_ForceDirection = m_ForceDirection.normalized;
            collision.gameObject.GetComponent<Collider>().enabled = false;
            AfterCollidedWithAnObstacle(m_ForceDirection * 10);
        }
    }

    public override void AfterCollidedWithAnObstacle(Vector3 _force)
    {
        Debug.Log("Player collided");
        m_MyState = PlayerStates.Idle;
        m_MyRigidBody.velocity = Vector3.zero;
        m_MyRigidBody.isKinematic = true;
        gameObject.tag = "Untagged";
        hJointObject.GetComponent<Rigidbody>().useGravity = true;
        hJointObject.GetComponent<Rigidbody>().constraints &= ~RigidbodyConstraints.FreezePositionY;
        if (hJointObject.GetComponent<HingeJoint>())
        {
            hJointObject.GetComponent<HingeJoint>().breakForce = 5;
        }
        hJointObject.GetComponent<Rigidbody>().AddForce(_force);
        StartCoroutine(GameController.Instance.CallGameOver(m_MyTransform.position));
    }

    public override void StopRace()
    {
        m_MyState = PlayerStates.Idle;
        m_MyRigidBody.velocity = Vector3.zero;
        hJointObject.GetComponent<Rigidbody>().isKinematic = true;
    }
}
