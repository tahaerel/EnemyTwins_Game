using UnityEngine;


namespace SplitScreenPro {

    public class CameraInputController : MonoBehaviour {

        public float rotationSpeed = 100f;

        private void Start() {
            Cursor.lockState = CursorLockMode.Locked;    
        }

        void Update() {

            float x = Input.GetAxis("Mouse X") * Time.deltaTime * rotationSpeed;
            float y = Input.GetAxis("Mouse Y") * Time.deltaTime * rotationSpeed;
            SplitScreenManager.instance.cameraHorizontalRotation += x;
            SplitScreenManager.instance.cameraVerticalRotation += y;

            float z = Input.GetAxis("Mouse ScrollWheel");
            SplitScreenManager.instance.cameraDistance += z;

        }
    }

}