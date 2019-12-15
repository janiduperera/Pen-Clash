using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NPCPlayer : BaseRunner
{
    #region Unity Attributes
    private Transform m_PlayerTransform;
    private TextMeshPro m_PlaceInRaceTxt;
    private TextMeshPro m_TypeTxt;
    private TrailRenderer m_MyTrailRender;
    public GameObject[] Pens;
    #endregion Unity Attributes

    #region Attributes
    private enum NPCPlayerStates
    {
        Idle,
        AtStart,
        TurningLeft,
        TurningLeftComplete,
        TurningRight,
        TurningRightComplete,
        InsideCollisionArea,
        Moving
    };
    private NPCPlayerStates m_MyState = NPCPlayerStates.Idle;
    private float m_TargetDeltaX;
    private float m_DeltaX = 0;
    private Vector3 m_MovingVector;
    private float m_DestinationX;
    private float m_DestinationZ;
    private float m_TurnStartX;
    private float m_TurnStartZ;
    private bool m_IsTurnCurveDistanceSet = false;
    private float m_TurnCurveTravelDistanceX;
//    private float m_TurnCurveTravelDistanceZ;
    private bool m_TrailRendererStatus = true;
    private Vector2 m_ClampArea;
    #endregion Attributes

    protected override void Awake()
    {
        base.Awake();
        m_PlayerTransform = GameObject.FindWithTag("Player").GetComponent<Transform>();
        m_PlaceInRaceTxt = m_MyTransform.Find("TextMeshPro").gameObject.GetComponent<TextMeshPro>();
        m_TypeTxt = hJointObject.transform.Find("TypeTxt").gameObject.GetComponent<TextMeshPro>();
        m_MyTrailRender = m_MyTransform.Find("Trail").gameObject.GetComponent<TrailRenderer>();
    }

    protected override void Start()
    {
        base.Start();

        Pens[Random.Range(0, Pens.Length)].SetActive(true);
        hJointObject.transform.SetParent(null);
        m_MyState = NPCPlayerStates.Idle;
        Color m_TrailColor = GameController.Instance.GetRandomColor();
        m_MyTrailRender.startColor = m_TrailColor;
        m_MyTrailRender.endColor = new Color(m_TrailColor.r, m_TrailColor.g, m_TrailColor.b, 95f/255f);
        m_TypeTxt.text = Type;

        m_TrailRendererStatus = m_MyTrailRender.enabled;
        m_Speed = Random.Range(9, 12);
    }

    protected override void OnCountDownFinish()
    {
        m_MyState = NPCPlayerStates.AtStart; // At the begning, the player X value should not change. Otherwise it will collide with Obsticals near by. 
                                                          // Therefore, changing to the Moving State should come after the players 1st Obstacle by pass. 
    }

    protected override void OnRaceFinishEvent()
    {
       
    }

    public override void AfterSortingRunners()
    {
        if (PlaceInTheRace == 1)
        {
            m_PlaceInRaceTxt.text = PlaceInTheRace + " st";
        }
        else if (PlaceInTheRace == 2)
        {
            m_PlaceInRaceTxt.text = PlaceInTheRace + " nd";
        }
        else if (PlaceInTheRace == 3)
        {
            m_PlaceInRaceTxt.text = PlaceInTheRace + " rd";
        }
        else
        {
            m_PlaceInRaceTxt.text = PlaceInTheRace + " th";
        }
    }
     

    protected override void Move()
    {
        if (m_MyState != NPCPlayerStates.Idle)
        {
            if (m_MyState == NPCPlayerStates.TurningLeft)
            {
                if (m_TargetDeltaX < m_DeltaX)
                {
                    m_DeltaX -= 0.02f;

                    if (m_MyTransform.position.x <= m_DestinationX)
                    {
                        m_MyState = NPCPlayerStates.TurningLeftComplete;
                    }
                }
                else
                {
                    m_DeltaX = m_TargetDeltaX;
                    if (!m_IsTurnCurveDistanceSet)
                    {
                        m_IsTurnCurveDistanceSet = true;
                        m_TurnCurveTravelDistanceX = Mathf.Abs(m_MyTransform.position.x - m_TurnStartX);
                        //m_TurnCurveTravelDistanceZ = Mathf.Abs(m_MyTransform.position.z - m_TurnStartZ);
                    }

                    if (m_MyTransform.position.x <= m_DestinationX + m_TurnCurveTravelDistanceX)
                    {
                        if (m_DeltaX < 0)
                        {
                            m_DeltaX += 0.02f;
                        }
                       
                        m_IsTurnCurveDistanceSet = false;
                        m_MyState = NPCPlayerStates.TurningLeftComplete;
                    }
                }
            }

            if (m_MyState == NPCPlayerStates.TurningLeftComplete)
            {
                if (m_DeltaX < 0)
                {
                    m_DeltaX += 0.02f;
                }
                else
                {
                    m_DeltaX = 0;
                    m_MyState = NPCPlayerStates.InsideCollisionArea;
                }
            }


            if (m_MyState == NPCPlayerStates.TurningRight)
            {
                if(m_TargetDeltaX > m_DeltaX)
                {
                    m_DeltaX += 0.02f;

                    if (m_MyTransform.position.x >= m_DestinationX)
                    {
                        m_MyState = NPCPlayerStates.TurningRightComplete;
                    }
                }
                else
                {
                    m_DeltaX = m_TargetDeltaX;
                    if (!m_IsTurnCurveDistanceSet)
                    {
                        m_IsTurnCurveDistanceSet = true;
                        m_TurnCurveTravelDistanceX = Mathf.Abs(m_MyTransform.position.x - m_TurnStartX);
                       // m_TurnCurveTravelDistanceZ = Mathf.Abs(m_MyTransform.position.z - m_TurnStartZ);
                    }

                    if (m_MyTransform.position.x >= m_DestinationX - m_TurnCurveTravelDistanceX)
                    {
                        if (m_DeltaX > 0)
                        {
                            m_DeltaX -= 0.02f;
                        }

                        m_IsTurnCurveDistanceSet = true;
                        m_MyState = NPCPlayerStates.TurningRightComplete;
                    }
                }
            }

            if (m_MyState == NPCPlayerStates.TurningRightComplete)
            {
                if(m_DeltaX > 0)
                {
                    m_DeltaX -= 0.02f;
                }
                else
                {
                    m_DeltaX = 0;
                    m_MyState = NPCPlayerStates.InsideCollisionArea;
                }
            }

            if(m_MyState == NPCPlayerStates.Moving) // This part is to give a small X value change when NPC is moving. So the NPC is not always moving in straight line. 
            {
                if(m_DeltaX > 0)
                {
                    m_DeltaX -= 0.01f;
                }
                else if(m_DeltaX < 0)
                {
                    m_DeltaX += 0.01f;
                }
                if (System.Math.Abs(m_DeltaX) < Mathf.Epsilon)
                {
                    m_DeltaX = GetRandomXPointToGiveAWobble();
                }
            }
            m_MovingVector = m_MyTransform.position + new Vector3(m_DeltaX, 0, 1) * Time.fixedDeltaTime * m_Speed;
            m_ClampArea = GameController.Instance.ClampAreaOnPlatForm(m_MyTransform.position.z);
            m_MovingVector.x = Mathf.Clamp(m_MovingVector.x, m_ClampArea.x, m_ClampArea.y);
            m_MyRigidBody.MovePosition(m_MovingVector);

            // Controlling the Trail renderer
            if (m_MyTransform.position.z - 60 < m_PlayerTransform.position.z && m_MyTransform.position.z + 10 > m_PlayerTransform.position.z)
            {
                if (!m_TrailRendererStatus)
                {
                    m_TrailRendererStatus = true;
                    m_MyTrailRender.enabled = true;
                }
            }
            else
            {
                if (m_TrailRendererStatus)
                {
                    m_TrailRendererStatus = false;
                    m_MyTrailRender.Clear();
                    m_MyTrailRender.enabled = false;
                }
            }
        }
    }

    private float m_RandomX;
    private int m_RandomInt;
    private float GetRandomXPointToGiveAWobble()
    {
        m_RandomX = Random.Range(0.3f, 0.6f);

        m_RandomInt = Random.Range(0, 2);  //returns a 0 or 1
        if (m_RandomInt == 0)  //if 0 lets make randomX a negative value
        {
            m_RandomX -= (2f * m_RandomX);
        }

        return m_RandomX;
    }

    protected override void OnCollisionEnter(Collision collision)
    {
        base.OnCollisionEnter(collision);

        if (collision.gameObject.tag == "End")
        {
            m_MyState = NPCPlayerStates.Idle;
            m_MyRigidBody.velocity = Vector3.zero;
            Type = "";
            gameObject.tag = "Untagged";
            hJointObject.GetComponent<Rigidbody>().useGravity = true;
            hJointObject.GetComponent<Rigidbody>().constraints &= ~RigidbodyConstraints.FreezePositionY;
            if (hJointObject.GetComponent<HingeJoint>())
            {
                hJointObject.GetComponent<HingeJoint>().breakForce = 5;
            }
            hJointObject.GetComponent<Rigidbody>().AddForce(m_MyTransform.forward * 20);

            //hJointObject.transform.rotation = Quaternion.LookRotation(Vector3.down);
        }
    }

    public override void AfterCollidedWithAnObstacle(Vector3 _force)
    {
        Debug.Log("NPC Collided");
        m_MyState = NPCPlayerStates.Idle;
        m_MyRigidBody.velocity = Vector3.zero;
        Type = "";
        gameObject.tag = "Untagged";
        hJointObject.GetComponent<Rigidbody>().useGravity = true;
        hJointObject.GetComponent<Rigidbody>().constraints &= ~RigidbodyConstraints.FreezePositionY;
        if (hJointObject.GetComponent<HingeJoint>())
        {
            hJointObject.GetComponent<HingeJoint>().breakForce = 5;
        }
    }

    public override void StopRace()
    {
        m_MyState = NPCPlayerStates.Idle;
        m_MyRigidBody.velocity = Vector3.zero;
        hJointObject.GetComponent<Rigidbody>().isKinematic = true;
        m_TrailRendererStatus = false;
        m_MyTrailRender.Clear();
        m_MyTrailRender.enabled = false;
    }

    public void GetRandomObsticleFreeXPoint(float _destinationX, float _destinationZ)
    {
        if (m_MyState == NPCPlayerStates.Moving || m_MyState == NPCPlayerStates.AtStart)
        {
            m_DestinationX = _destinationX;
            m_DestinationZ = _destinationZ;
            m_TargetDeltaX = (_destinationX - m_MyTransform.position.x) / m_Speed;
            m_TurnStartX = m_MyTransform.position.x;
            m_TurnStartZ = m_MyTransform.position.z;
            m_DeltaX = 0;

            if (m_TargetDeltaX < 0)
            {
                m_MyState = NPCPlayerStates.TurningLeft;
            }
            else if(m_TargetDeltaX > 0)
            {
                m_MyState = NPCPlayerStates.TurningRight;
            }
            else
            {
                m_MyState = NPCPlayerStates.InsideCollisionArea;
            }
        }
    }

    public void OnCollsionAreaExit()
    {
        if (m_MyState == NPCPlayerStates.InsideCollisionArea)
        {
            m_MyState = NPCPlayerStates.Moving;
        }
    }
}
