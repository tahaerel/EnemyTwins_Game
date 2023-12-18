using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Finish : MonoBehaviour
{
    public GameObject winner1,winner2,winscene,splitsceen;

    public Sayac sayac;

    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("kaptan"))
        {
            winscene.SetActive(true);
            sayac.StopTimer();
            splitsceen.SetActive(false);
            winner1.SetActive(true);
            Destroy(other.gameObject);
        }

        if (other.CompareTag("hook"))
        {
            winscene.SetActive(true);
            sayac.StopTimer();
            splitsceen.SetActive(false);
            winner2.SetActive(true);
            Destroy(other.gameObject);

        }
    }

  
}
