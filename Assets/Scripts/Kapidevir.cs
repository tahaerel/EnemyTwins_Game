using Unity.VisualScripting;
using UnityEngine;

public class Kapidevir : MonoBehaviour
{
    public Vector3 impulse = new Vector3(0.0f, 0.0f, 0.0f);

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("kapi")){ 
        other.gameObject.GetComponent<Rigidbody>().AddForce(impulse, ForceMode.Impulse);
        Debug.Log("sas");
        }
    }

  
   
}
