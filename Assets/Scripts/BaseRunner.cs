using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseRunner : MonoBehaviour
{
    #region Unity Attributes
    protected Rigidbody m_MyRigidBody;
    protected Transform m_MyTransform;
    public GameObject hJointObject;
    #endregion Unity Attributes

    #region Attributes
    private string m_Type;
    public string Type
    {
        get { return m_Type; }
        set { m_Type = value; }
    }
    protected float m_Speed = 10;

    public float ZPosition
    {
        get { return m_MyTransform.position.z; }
    }

    private int m_PlaceInTheRace;
    public int PlaceInTheRace
    {
        get { return m_PlaceInTheRace; }
        set { m_PlaceInTheRace = value; }
    }
    #endregion Attributes

    protected virtual void Awake()
    {
        m_MyTransform = transform;
        m_MyRigidBody = GetComponent<Rigidbody>();

    }

    protected virtual void Start()
    {
        UIController.Instance.OnCountDownFinish += OnCountDownFinish;
        GameController.Instance.OnRaceFinish += OnRaceFinishEvent;
    }

    // Update is called once per frame
    protected virtual void FixedUpdate()
    {
        Move();
    }

    protected virtual void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Obstacle")
        {
            Vector3 dir = collision.contacts[0].point - m_MyTransform.position;

            dir = -dir.normalized;

            AfterCollidedWithAnObstacle(dir * 15);
        }
    }

    protected abstract void OnCountDownFinish();
    protected abstract void OnRaceFinishEvent();
    protected abstract void Move();
    public abstract void AfterCollidedWithAnObstacle(Vector3 _force);
    public abstract void AfterSortingRunners();
    public abstract void StopRace();
}
