using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    private Transform m_MyTransform;
    // Start is called before the first frame update
    void Start()
    {
        m_MyTransform = transform; 
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            AudioController.Instance.PlayEffectAudio("CoinsCollect");
            GameController.Instance.AddCoins(1);
            GetComponent<Collider>().enabled = false;
            StartCoroutine(PlayRotateAnimation());
        }
        else if(other.gameObject.tag == "PlayerPen")
        {
            GetComponent<Collider>().enabled = false;
            StartCoroutine(PlayRotateAnimation());
        }

    }

    IEnumerator PlayRotateAnimation()
    {
        float m_StartY = -3;
        while (m_StartY < 10)
        {
            m_StartY += 0.1f;
            m_MyTransform.position = new Vector3(m_MyTransform.position.x, m_StartY, m_MyTransform.position.z);
            yield return null;
        }

        GameController.Instance.PlaceCoinAgain(m_MyTransform.gameObject);
    }
}
