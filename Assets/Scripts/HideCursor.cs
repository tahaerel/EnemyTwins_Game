using UnityEngine;


namespace SplitScreenPro_Demos {

    public class HideCursor : MonoBehaviour {

        private void Start() {
            Cursor.lockState = CursorLockMode.Locked;    
        }
    }

}