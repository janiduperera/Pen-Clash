using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaceFinishTrigger : MonoBehaviour
{
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            GameController.Instance.CallRaceFinish();
        }
    }
}
