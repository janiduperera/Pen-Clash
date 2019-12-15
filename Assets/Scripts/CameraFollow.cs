using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private Transform m_PlayerTransform;
    private Vector3 m_OffSet;
    private Transform m_MyTransform;
    private Vector3 m_MoveVector;

    // Start is called before the first frame update
    void Start()
    {
        m_PlayerTransform = GameObject.FindWithTag("Player").GetComponent<Transform>();
        //m_PlayerTransform = GameObject.FindWithTag("NPCPlayer").GetComponent<Transform>();
        m_MyTransform = transform;

        m_OffSet = m_MyTransform.position - m_PlayerTransform.position;
       // m_OffSet = new Vector3(0.0f, 8.6f, -9.7f);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        m_MoveVector = m_PlayerTransform.position + m_OffSet;
        m_MoveVector.y = m_MyTransform.position.y;
        m_MoveVector.x = Mathf.Clamp(m_MoveVector.x, -6f, 6f);
        m_MyTransform.position = m_MoveVector;
    }
}
