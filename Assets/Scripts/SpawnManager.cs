using UnityEngine;
using System;
using System.Collections;

public class SpawnManager : MonoBehaviour
{
    public Transform spawnPoint;
    public Transform spawnPoint2;// Iþýnlanma noktasý
    public float respawnTime = 1f; // Iþýnlanma süresi

    public Animator karakteranimasyon;


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("ds1"))
        {
            karakteranimasyon.enabled = false;
            StartCoroutine(RespawnAfterDelay());
        }
        if (other.gameObject.CompareTag("ds2"))
        {
            karakteranimasyon.enabled = false;
            StartCoroutine(Respawn2AfterDelay());
        }
    }
 
    IEnumerator RespawnAfterDelay()
    {
        yield return new WaitForSeconds(respawnTime);
        Debug.Log("ÝSÝNLANDI");

       transform.position = new Vector3(spawnPoint.transform.position.x, spawnPoint.transform.position.y, spawnPoint.transform.position.z);
        yield return new WaitForSeconds(0.6f);
        karakteranimasyon.enabled = true;

    }


    IEnumerator Respawn2AfterDelay()
    {
        yield return new WaitForSeconds(respawnTime);
        Debug.Log("ÝSÝNLANDI2");

        transform.position = new Vector3(spawnPoint2.transform.position.x, spawnPoint2.transform.position.y, spawnPoint2.transform.position.z);
        yield return new WaitForSeconds(0.6f);
        karakteranimasyon.enabled = true;

    }
}
