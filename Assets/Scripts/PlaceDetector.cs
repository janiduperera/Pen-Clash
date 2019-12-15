using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceDetector : MonoBehaviour
{
    private string m_MyParentTag;
    private void Start()
    {
        m_MyParentTag = transform.parent.gameObject.tag;
    }

    private void OnTriggerExit(Collider other)
    {
        //This is used to sort the runners.By this way maximum times that the sort the runners gets called at a time is twise.Better than using it in Update
        if (other.gameObject.tag == "Player" || other.gameObject.tag == "NPCPlayer")
        {
            GameController.Instance.SortTheRunners();
        }
        else if (m_MyParentTag == "Player" && other.gameObject.tag == "PropEnd") // This is to used to spawn props
        {
            GameController.Instance.SpawnProps(1);
        }
    }
}
