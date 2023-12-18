/****************************************
 *      Simple Character Controller     *
 *  Adapted from Pirates & Hunters Pack *
 ****************************************/
using UnityEngine;


namespace SplitScreenPro_Demos {

    public class SimpleController : MonoBehaviour {

        [Tooltip("Global movement speed")]
        public float Speed = 5.5f;
        public float RotationSpeed = 200;

        public KeyCode leftKey = KeyCode.LeftArrow;
        public KeyCode rightKey = KeyCode.RightArrow;
        public KeyCode forwardKey = KeyCode.UpArrow;
        public KeyCode backKey = KeyCode.DownArrow;
        public KeyCode jumpKey = KeyCode.Space;

        const float Gravity = 5;
        const float JumpTime = 0.5f;
        public float JumpForce = 18f;


        Vector3 direction;
        float inpVer;
        float inpHor;

        bool jumping;
        float jumpDuration;
        float sprint = 0.5f;
        bool animatedJump;

        Animator thisAnimator;
        CharacterController thisController;


        void Start() {


            thisAnimator = GetComponent<Animator>();
            thisController = GetComponent<CharacterController>();

            if (thisAnimator.applyRootMotion) {
                animatedJump = true;
            } else {
                animatedJump = false;
            }
            thisAnimator.SetInteger(AnimationKeywords.Weapon, 0);
        }


        void Update() {
            inpVer = 0;
            inpHor = 0;

            if (this.gameObject.name == "Pirate_Hook" && Arduino.forward)
            {
                inpVer = 1;

            }
            if (this.gameObject.name == "Pirate_Hook" && Arduino.back)
            {
                inpVer = -1;

            }
            if (this.gameObject.name == "Pirate_Hook" && Arduino.left)
            {
                inpHor = -1;

            }
            if (this.gameObject.name == "Pirate_Hook" && Arduino.right)
            {
                inpHor = 1;

            }
           
            if (Input.GetKey(forwardKey))
            {
                inpVer = 1;
            }
            else if (Input.GetKey(backKey)) {
                inpVer = -1;
            }
            if (Input.GetKey(leftKey)) {
                inpHor = -1;
            } else if (Input.GetKey(rightKey)) {
                inpHor = 1;
            }

            if (inpVer != 0 || inpHor != 0) {
                thisAnimator.applyRootMotion = true;
            }

            // Swim mode
            if (Input.GetKeyDown(KeyCode.P)) {
                thisAnimator.SetFloat(AnimationKeywords.Speed, 0);

                if (thisAnimator.GetBool(AnimationKeywords.Swimming)) {
                    thisAnimator.SetBool(AnimationKeywords.Swimming, false);
                } else {
                    thisAnimator.SetBool(AnimationKeywords.Swimming, true);
                }
            }

            float backSpeed;
            if (inpVer < 0) {
                backSpeed = 0.5f;
            } else {
                backSpeed = 1;
            }

            transform.Rotate(0, inpHor * RotationSpeed * Time.deltaTime, 0, Space.World);
            direction = transform.forward * Speed * inpVer * sprint * backSpeed;
            thisAnimator.SetFloat(AnimationKeywords.Speed, inpVer * sprint * Speed);

            if (jumping) {
                jumpDuration -= Time.deltaTime;
                if (jumpDuration <= 0) jumping = false;
            } 
            else {
                if (thisController.isGrounded)
                    direction.y = Mathf.Epsilon;
                else
                    direction += Vector3.down * Gravity;
            }

            thisController.Move(direction * Time.deltaTime);

            if (jumpDuration <= 0) jumping = false;

            if (thisController.isGrounded) jumping = false;


            if (this.gameObject.name == "Pirate_Hook" && Arduino.jump)
            {
                if (animatedJump)
                {
                    if (!jumping)
                    {
                        Jump();
                    }
                }
                else
                {
                    if (thisController.isGrounded)
                    {
                        JumpInPlace();
                    }
                }

            }


            if (Input.GetKeyDown(jumpKey)) {
                if (animatedJump) {
                    if (!jumping) {
                        Jump();
                    }
                } 
                
                else {
                    if (thisController.isGrounded) {
                        JumpInPlace();
                    }
                }
            }
        }

        /// Animations
        public void Idle() {
            Reset(false, 0);
        }

        public void Walk() {
            Reset(false, 0.5f);
        }

        public void Run() {
            Reset(false, 1);
        }

        public void Jump() {
            jumping = true;
            jumpDuration = JumpTime;
            thisAnimator.SetTrigger(AnimationKeywords.Jump);

            // Apply additional forward force to the jump
            Vector3 jumpDirection = transform.up * JumpForce;

            // Modify the position directly during the jump
            thisController.Move(jumpDirection * Time.deltaTime);
        }

        public void JumpInPlace() {
            jumping = true;
            jumpDuration = JumpTime;
            thisAnimator.SetTrigger(AnimationKeywords.JumpInPlace);

            // Apply additional forward force to the jump in place
            Vector3 jumpDirection = transform.forward * JumpForce;

            // Modify the position directly during the jump in place
            thisController.Move(jumpDirection * Time.deltaTime);
        }

        public void Reset(bool rootMotion, float speed) {
            Stop(rootMotion, speed);
        }

        public void Stop(bool rootMotion, float speed) {
            thisAnimator.applyRootMotion = rootMotion;
            thisAnimator.SetFloat(AnimationKeywords.Speed, speed);
        }

    }

}