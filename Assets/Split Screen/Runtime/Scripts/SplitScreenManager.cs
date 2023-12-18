using System;
using UnityEngine;
using UnityEngine.UI;

namespace SplitScreenPro {

    public enum SplitMode {
        Off = 0,
        SingleCameraOnPlayer1 = 1,
        SingleCameraOnPlayer2 = 2,
        SplitScreenFixedDivision = 10,
        SplitScreenAutomatic = 20
    }

    public enum SplitScreenState {
        RenderingMain = 0,
        RenderingBoth = 1,
        SwitchingToBoth = 2,
        SwitchingToMain = 3
    }


    [ExecuteAlways]
    [DefaultExecutionOrder(1000)]
    public partial class SplitScreenManager : MonoBehaviour {

        public Transform player1;
        public Transform player2;
        [Tooltip("Perform camera changes also while not in play-mode")]
        public bool previewInEditMode = true;

        [Header("Split Screen Settings")]
        public SplitMode splitMode = SplitMode.SplitScreenFixedDivision;

        [Range(-Mathf.PI, Mathf.PI)]
        public float splitLineAngle = 1.4f;
        public float splitLineWidth = 5f;

        [ColorUsage(showAlpha: false)]
        public Color splitLineColor = Color.black;
        public float splitLineFadeDuration = 0.5f;
        public Vector2 splitLineCenter = new Vector2(0.5f, 0.5f);
        [Range(0.25f, 0.9f)]
        [Tooltip("Maximum seperation of a player from the center of screen")]
        public float maxScreenSeparation = 0.5f;

        [Header("Camera Control")]
        public Camera mainCamera;
        public float cameraVerticalRotation = 30;
        public bool cameraVerticalRotationConstraint = true;
        [Range(0, 90)]
        public float cameraVerticalRotationMinAngle = 5;
        [Range(0, 90)]
        public float cameraVerticalRotationMaxAngle = 70;
        public float cameraHorizontalRotation;
        [Tooltip("If enabled, the horizontal rotation will be managed automatically to keep the split line vertical in the center.")]
        public bool cameraHorizontalRotationAuto;
        [Tooltip("Smoothes camera rotation. A value of 0 means no smoothing. The higher the value, the faster the rotation change.")]
        public float cameraRotationSmoothing = 2f;

        public float cameraDistance = 20f;
        public bool cameraDistanceClamped = true;
        public float cameraDistanceMin = 5;
        public float cameraDistanceMax = 100;
        [Tooltip("Zooms out to try to keep both players in same view (up to camera distance max)")]
        public bool cameraDistanceManaged = true;
        [Tooltip("If a different camera distance must be used when in split mode.")]
        public bool cameraUseCustomDistanceWhenSplit = true;
        [Tooltip("The camera distance used when in split screen mode.")]
        public float cameraDistanceWhenSplit = 30f;
        [Tooltip("Zoom duration when restoring the camera distance in split mode.")]
        public float cameraZoomDuration = 1.5f;

        [Header("Audio")]
        [Tooltip("If enabled, sounds will be emitted from a position relative to the nearest player. Please check the documentation about how to setup audio sources correctly to make them compatible with split screen mode.")]
        public bool splitAudio;

        [Header("Composition")]
        [Range(0.1f, 1f)]
        [Tooltip("Downsamples individual camera rendering to improve performance")]
        public float downsampling = 1f;
        [Tooltip("Enables HDR in final composition")]
        public bool hdr;
        [Tooltip("Show the cameras rendering on each side")]
        public bool showCameraIds;

        static SplitScreenManager _instance;
        public static SplitScreenManager instance {
            get {
                if (_instance == null) {
                    _instance = FindObjectOfType<SplitScreenManager>();
                }
                return _instance;
            }
        }


        Camera cam, secondaryCamera;
        SplitScreenState splitScreenState = SplitScreenState.RenderingMain;

        public SplitScreenState currentSplitScreenState => splitScreenState;

        float transitionStartTime;
        RenderTexture rtMainCamera, rtSecondaryCamera;
        float currentSplitLineAngle;
        Canvas canvas;
        RectTransform canvasRect;
        Material mat;
        float currentCameraDistance;
        // time when split screen is enabled (in automatic mode)
        float timeSplit;
        RectTransform cameraLabel1, cameraLabel2;
        Vector2 currentSplitLineCenter;
        static Vector2 vector2half = new Vector2(0.5f, 0.5f);


        [RuntimeInitializeOnLoadMethod]
        void DomainReloadDisabledSupport() {
            _instance = null;
            splitAudioSources.Clear();
        }

        private void OnEnable() {
            cam = GetComponent<Camera>();
            if (cam == null) {
                Debug.LogError("SplitScreenComposeCamera requires a camera!");
                return;
            }
            if (mainCamera == null) {
                mainCamera = Camera.main;
            }
            secondaryCamera = transform.Find("SecondaryCamera").GetComponent<Camera>();
            secondaryCamera.CopyFrom(mainCamera);

            canvas = GetComponentInChildren<Canvas>();
            canvas.enabled = false;
            canvasRect = canvas.GetComponent<RectTransform>();
            RawImage rawImage = GetComponentInChildren<RawImage>();
            mat = rawImage.material;
            currentCameraDistance = cameraDistance;
            cameraLabel1 = canvas.transform.Find("Camera1").GetComponent<RectTransform>();
            cameraLabel2 = canvas.transform.Find("Camera2").GetComponent<RectTransform>();
        }

        private void OnDisable() {
            DisableSplitCameras();
        }

        private void OnDestroy() {
            if (rtMainCamera != null) rtMainCamera.Release();
            if (rtSecondaryCamera != null) rtSecondaryCamera.Release();
        }

        private void OnValidate() {
            splitLineWidth = Mathf.Max(1, splitLineWidth);
            cameraRotationSmoothing = Mathf.Max(0, cameraRotationSmoothing);
            cameraZoomDuration = Mathf.Max(0, cameraZoomDuration);
            cameraVerticalRotationMaxAngle = Mathf.Max(cameraVerticalRotationMaxAngle, cameraVerticalRotationMinAngle);
            cameraDistanceMin = Mathf.Max(0, cameraDistanceMin);
            cameraDistanceMax = Mathf.Max(cameraDistanceMin, cameraDistanceMax);
        }

        private void LateUpdate() {

            if (!(previewInEditMode || Application.isPlaying)) {
                if (splitScreenState != SplitScreenState.RenderingMain) {
                    DisableSplitCameras();
                }
                return;
            }

            if (splitMode == SplitMode.Off) return;

            switch (splitMode) {
                case SplitMode.SingleCameraOnPlayer1:
                    ManageSinglePlayer(player1);
                    break;
                case SplitMode.SingleCameraOnPlayer2:
                    ManageSinglePlayer(player2);
                    break;
                case SplitMode.SplitScreenFixedDivision:
                    ManageFixedMode();
                    break;
                case SplitMode.SplitScreenAutomatic:
                    ManageAutoMode();
                    break;
            }

            if (splitAudio && Application.isPlaying) {
                UpdateSplitAudioSources();
            }
        }

        void ManageSinglePlayer(Transform player) {
            DisableSplitCameras();
            if (player == null) return;

            currentCameraDistance = Mathf.Clamp(cameraDistance, cameraDistanceMin, cameraDistanceMax);
            UpdateCamera(mainCamera, player.position, GetRotation());
        }

        void ManageFixedMode() {

            if (mainCamera == null || secondaryCamera == null) {
                DisableSplitCameras();
                return;
            }

            splitScreenState = SplitScreenState.RenderingBoth;

            EnableSplitCameras();

            currentSplitLineAngle = splitLineAngle;
            currentSplitLineCenter = splitLineCenter;

            Render();
        }

        void ManageAutoMode() {

            if (secondaryCamera == null || player1 == null || player2 == null) {
                DisableSplitCameras();
                return;
            }

            float now = Time.time;

            Vector3 playersAxis = player2.position - player1.position;
            float distanceWS = playersAxis.magnitude;
            Vector3 playersDir = playersAxis / distanceWS;

            Vector3 cam1Pos, cam2Pos;
            bool singleCamera = true;

            if (cameraDistanceClamped) {
                cameraDistance = Mathf.Clamp(cameraDistance, cameraDistanceMin, cameraDistanceMax);
            }
            float desiredCameraDistance = cameraDistance;

            if (distanceWS > 0) {
                float halfSeparationWS = distanceWS * 0.5f;
                float tangent = Mathf.Tan(mainCamera.fieldOfView * 0.5f * Mathf.Deg2Rad);
                float fovFactor = tangent * mainCamera.aspect * maxScreenSeparation;
                float adjustedDistance = halfSeparationWS / fovFactor;
                adjustedDistance = Mathf.Clamp(adjustedDistance, cameraDistanceMin, cameraDistanceMax);

                // if distance is auto-managed, check if we can zoom out to keep both players in same view (up to max camera distance)
                if (cameraDistanceManaged) {
                    if (cameraDistance < adjustedDistance) {
                        desiredCameraDistance = adjustedDistance + 0.001f;
                    }
                    // if we're in split screen mode, check if we can return zoom to normal
                    if (splitScreenState == SplitScreenState.RenderingBoth && cameraUseCustomDistanceWhenSplit) {
                        if (cameraZoomDuration > 0 && Application.isPlaying) {
                            float t = (now - timeSplit) / cameraZoomDuration;
                            t = Lerp.SmootherStep(t);
                            currentCameraDistance = Mathf.Lerp(currentCameraDistance, cameraDistanceWhenSplit, t);
                        } else {
                            currentCameraDistance = cameraDistanceWhenSplit;
                        }
                    } else {
                        currentCameraDistance = desiredCameraDistance;
                    }
                }
                float fovSize = currentCameraDistance * fovFactor;
                float minDist = Mathf.Min(halfSeparationWS, fovSize);
                cam1Pos = player1.position + playersDir * minDist;
                cam2Pos = player2.position - playersDir * minDist;
                singleCamera = fovSize > halfSeparationWS;
            } else {
                cam1Pos = cam2Pos = player1.position;
            }

            Quaternion cameraRotation = cameraHorizontalRotationAuto ? GetRotation(playersDir) : GetRotation();
            UpdateCamera(mainCamera, cam1Pos, cameraRotation);
            UpdateCamera(secondaryCamera, cam2Pos, cameraRotation);

            if (singleCamera) {
                if (splitScreenState == SplitScreenState.RenderingBoth) {
                    transitionStartTime = now;
                    splitScreenState = SplitScreenState.SwitchingToMain;
                }
            } else {
                if (splitScreenState == SplitScreenState.RenderingMain) {
                    transitionStartTime = now;
                    splitScreenState = SplitScreenState.SwitchingToBoth;
                }
            }


            if (splitScreenState == SplitScreenState.RenderingMain) {
                DisableSplitCameras();
                return;
            }

            EnableSplitCameras();

            switch (splitScreenState) {
                case SplitScreenState.SwitchingToBoth: {
                        float t = Application.isPlaying ? (now - transitionStartTime) / splitLineFadeDuration : 1f;
                        if (t >= 1) {
                            splitScreenState = SplitScreenState.RenderingBoth;
                            timeSplit = now;
                        }
                        break;
                    }
                case SplitScreenState.SwitchingToMain: {
                        float t = Application.isPlaying ? (now - transitionStartTime) / splitLineFadeDuration : 1f;
                        if (t >= 1) {
                            splitScreenState = SplitScreenState.RenderingMain;
                        }
                        break;
                    }
            }

            Vector3 scr1 = mainCamera.WorldToViewportPoint(player1.position);
            Vector3 scr2 = secondaryCamera.WorldToViewportPoint(player2.position);
            float dx = scr1.x - scr2.x;
            float dy = scr2.y - scr1.y;
            currentSplitLineAngle = Mathf.Atan2(dx, dy);
            currentSplitLineCenter = vector2half;

            Render();
        }

        void Render() {
            switch (splitScreenState) {
                case SplitScreenState.SwitchingToBoth:
                    mat.SetVector(ShaderParams.transitionParams, new Vector4(transitionStartTime, splitLineFadeDuration, 1));
                    break;
                case SplitScreenState.SwitchingToMain:
                    mat.SetVector(ShaderParams.transitionParams, new Vector4(transitionStartTime, splitLineFadeDuration, 0));
                    break;
                case SplitScreenState.RenderingBoth:
                    mat.SetVector(ShaderParams.transitionParams, new Vector4(-1f, 0.001f, 1f));
                    break;
            }
            mat.SetVector(ShaderParams.compareParams, new Vector4(Mathf.Cos(currentSplitLineAngle), Mathf.Sin(currentSplitLineAngle), -Mathf.Cos(currentSplitLineAngle), splitLineWidth * 0.001f));
            mat.SetTexture(ShaderParams.source1, rtMainCamera);
            mat.SetTexture(ShaderParams.source2, rtSecondaryCamera);
            mat.SetColor(ShaderParams.splitLineColor, splitLineColor);
            mat.SetVector(ShaderParams.splitLineCenter, currentSplitLineCenter);

            if (cameraLabel1 != null && cameraLabel2 != null) {
                if (showCameraIds) {
                    float x = Mathf.Cos(currentSplitLineAngle);
                    float y = Mathf.Sin(currentSplitLineAngle);
                    Vector2 disp = new Vector2(-y, x) * 0.3f;
                    cameraLabel1.gameObject.SetActive(true);
                    cameraLabel1.localPosition = (currentSplitLineCenter - vector2half - disp) * canvasRect.rect.width; // new Vector2(Screen.width, Screen.height);
                    cameraLabel2.gameObject.SetActive(true);
                    cameraLabel2.localPosition = (currentSplitLineCenter - vector2half + disp) * canvasRect.rect.height; // new Vector2(Screen.width, Screen.height);
                } else {
                    cameraLabel1.gameObject.SetActive(false);
                    cameraLabel2.gameObject.SetActive(false);
                }
            }
        }

        void UpdateCamera(Camera cam, Vector3 target, Quaternion newRotation) {
            if (cameraRotationSmoothing > 0 && Application.isPlaying) {
                cam.transform.rotation = Quaternion.Slerp(cam.transform.rotation, newRotation, cameraRotationSmoothing * Time.deltaTime);
            } else {
                cam.transform.rotation = newRotation;
            }
            cam.transform.position = target - cam.transform.forward * currentCameraDistance;
        }

        Quaternion GetRotation(Vector3 playersDir) {
            cameraVerticalRotation = Mathf.Clamp(cameraVerticalRotation, cameraVerticalRotationMinAngle, cameraVerticalRotationMaxAngle);
            return Quaternion.LookRotation(Vector3.Cross(playersDir, Vector3.up), Vector3.up) * Quaternion.Euler(cameraVerticalRotation, 0, 0);
        }

        Quaternion GetRotation() {
            cameraVerticalRotation = Mathf.Clamp(cameraVerticalRotation, cameraVerticalRotationMinAngle, cameraVerticalRotationMaxAngle);
            return Quaternion.Euler(cameraVerticalRotation, cameraHorizontalRotation, 0);
        }


        void DisableSplitCameras() {
            if (mainCamera != null) {
                mainCamera.targetTexture = null;
            }
            if (secondaryCamera != null) {
                secondaryCamera.enabled = false;
                secondaryCamera.targetTexture = null;
            }
            if (cam != null) {
                cam.enabled = false;
            }
            if (canvas != null) {
                canvas.enabled = false;
            }
            splitScreenState = SplitScreenState.RenderingMain;
        }


        void EnableSplitCameras() {

            CheckRTs();

            if (!secondaryCamera.enabled) {
                secondaryCamera.enabled = true;
                if (Application.isPlaying) return; // give it a frame of rendering before composing in playmode to avoid black flash
            }
            cam.enabled = true;
            canvas.enabled = true;
            mainCamera.targetTexture = rtMainCamera;
        }


        void CheckRTs() {

            int w = Mathf.CeilToInt(Screen.width * downsampling);
            int h = Mathf.CeilToInt(Screen.height * downsampling);
            if (rtMainCamera == null || rtMainCamera.width != w || rtMainCamera.height != h) {
                if (rtMainCamera != null) {
                    rtMainCamera.Release();
                }
                rtMainCamera = new RenderTexture(w, h, 24, hdr ? RenderTextureFormat.DefaultHDR : RenderTextureFormat.Default);
            }

            if (rtSecondaryCamera == null || rtSecondaryCamera.width != w || rtSecondaryCamera.height != h) {
                if (rtSecondaryCamera != null) {
                    rtSecondaryCamera.Release();
                }
                rtSecondaryCamera = new RenderTexture(w, h, 24, hdr ? RenderTextureFormat.DefaultHDR : RenderTextureFormat.Default);
            }
            secondaryCamera.targetTexture = rtSecondaryCamera;
        }


    }

}
