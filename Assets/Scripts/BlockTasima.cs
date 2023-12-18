using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockTasima : MonoBehaviour
{
    public GameObject kaptantasinacak1;
    public GameObject kaptantasinacak2;
    public GameObject kaptantasinacak3;
    public GameObject kaptantasiyor1;
    public GameObject kaptantasiyor2;
    public GameObject kaptantasiyor3;

    //public GameObject hooktasinacak1;
    //public GameObject hooktasinacak2;
    //public GameObject hooktasinacak3;
    //public GameObject hooktasiyor1;
    //public GameObject hooktasiyor2;
    //public GameObject hooktasiyor3;

 



    public int kaptantasidi = 1;
    public int hooktasidi = 1;
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "kaptan")
        {
            if (kaptantasidi == 1)
            {
                kaptantasinacak1.SetActive(false);
                kaptantasiyor1.SetActive(true);
            }
            if (kaptantasidi == 2)
            {
                kaptantasinacak2.SetActive(false);
                kaptantasiyor2.SetActive(true);
            }
            if (kaptantasidi == 3)
            {
                kaptantasinacak3.SetActive(false);
                kaptantasiyor3.SetActive(true);
            }
        }
        //if (other.tag == "hook")
        //{
        //    if (kaptantasidi == 1)
        //    {
        //        hooktasinacak1.SetActive(false);
        //        hooktasiyor1.SetActive(true);
        //    }
        //    if (hooktasidi == 2)
        //    {
        //        hooktasinacak2.SetActive(false);
        //        hooktasiyor2.SetActive(true);
        //    }
        //    if (hooktasidi == 3)
        //    {
        //        hooktasinacak3.SetActive(false);
        //        hooktasiyor3.SetActive(true);
        //    }
       //) }
    }
}
