using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObsticleTrigger : MonoBehaviour
{
    #region Unity Attributes
    private Transform m_ParentTransform;
    private int m_ObsticleType;
    #endregion Unity Attributes

    #region Attributes
    private int m_RandomXPointIndex;
    public bool IsPlatform;
    List<float> m_AvaiableXPoints = new List<float>(); // This will have free X cordinates that runners can use in Obstacle areas as well as half platforms. 
    #endregion Attributes

    void Start()
    {
        if(IsPlatform) // This is put to true only for Half platform. 
        {
            m_ParentTransform = transform.parent;

            float m_MinX = m_ParentTransform.position.x - m_ParentTransform.localScale.x * 0.5f + 2;
            float m_MaxX = m_ParentTransform.position.x + m_ParentTransform.localScale.x * 0.5f - 2;

            m_AvaiableXPoints.Clear();

            for (int i = (int)m_MinX; i <= (int)m_MaxX; i++)
            {
                m_AvaiableXPoints.Add(i);
            }
        }
    }

    public List<float> Initiate(int _obsticleType) // This is called for Obstacles. 
    {
        m_ObsticleType = _obsticleType;
        m_ParentTransform = transform.parent;

        // Here the Parent is the Obstacle
        float m_MinX = Mathf.Round(m_ParentTransform.position.x) - m_ParentTransform.localScale.x * 0.5f - 1.5f;
        float m_MaxX = Mathf.Round(m_ParentTransform.position.x) + m_ParentTransform.localScale.x * 0.5f + 1.5f;

        m_AvaiableXPoints.Clear();

        if (_obsticleType == 1)// Prefabs/OneObsticle
        {
            for (int i = -7; i < 8; i++)
            {
                if (i < m_MinX)
                {
                    m_AvaiableXPoints.Add(i);
                }
                else if (i > m_MaxX)
                {
                    m_AvaiableXPoints.Add(i);
                }
            }
        }
        else if (_obsticleType == 2) // Prefabs/TwoObsticle
        {
            for (int i = -2; i < 3; i++)
            {
                m_AvaiableXPoints.Add(i);
            }

        }
        else if (_obsticleType == 3)  // Prefabs/SidedObsticle
        {
            for (int i = -7; i < 8; i++)
            {
                if (i < m_MinX)
                {
                    m_AvaiableXPoints.Add(i);
                }
                else if (i > m_MaxX)
                {
                    m_AvaiableXPoints.Add(i);
                }
            }
        }

        return m_AvaiableXPoints;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "NPCPlayer")
        {
            m_RandomXPointIndex = Random.Range(0, m_AvaiableXPoints.Count);
            other.gameObject.GetComponent<NPCPlayer>().GetRandomObsticleFreeXPoint(m_AvaiableXPoints[m_RandomXPointIndex], m_ParentTransform.position.z);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "NPCPlayer")
        {
            other.gameObject.GetComponent<NPCPlayer>().OnCollsionAreaExit();
        }
    }
}
