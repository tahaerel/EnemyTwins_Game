/// <summary>
/// Screen Split Pro inspector. Copyright 2021 Kronnect
/// </summary>
using UnityEditor;
using UnityEngine;

namespace SplitScreenPro {
    [CustomEditor(typeof(SplitScreenManager))]
    public class SplitScreenManagerInspector : Editor {

        SerializedProperty mainCamera;
        SerializedProperty player1, player2;
        SerializedProperty splitMode, splitLineAngle, splitLineWidth, splitLineColor, splitLineFadeDuration, splitLineCenter;
        SerializedProperty maxScreenSeparation;
        SerializedProperty previewInEditMode;
        SerializedProperty cameraVerticalRotation, cameraVerticalRotationMinAngle, cameraVerticalRotationMaxAngle;
        SerializedProperty cameraHorizontalRotation, cameraHorizontalRotationAuto, cameraRotationSmoothing;
        SerializedProperty cameraUseCustomDistanceWhenSplit, cameraDistanceWhenSplit, cameraZoomDuration;
        SerializedProperty cameraDistance, cameraDistanceMin, cameraDistanceMax, cameraDistanceManaged;
        SerializedProperty splitAudio;
        SerializedProperty downsampling, hdr, showCameraIds;

        void OnEnable() {

            mainCamera = serializedObject.FindProperty("mainCamera");
            player1 = serializedObject.FindProperty("player1");
            player2 = serializedObject.FindProperty("player2");
            splitMode = serializedObject.FindProperty("splitMode");
            splitLineAngle = serializedObject.FindProperty("splitLineAngle");
            splitLineWidth = serializedObject.FindProperty("splitLineWidth");
            splitLineColor = serializedObject.FindProperty("splitLineColor");
            splitLineFadeDuration = serializedObject.FindProperty("splitLineFadeDuration");
            splitLineCenter = serializedObject.FindProperty("splitLineCenter");
            maxScreenSeparation = serializedObject.FindProperty("maxScreenSeparation");
            previewInEditMode = serializedObject.FindProperty("previewInEditMode");
            cameraVerticalRotation = serializedObject.FindProperty("cameraVerticalRotation");
            cameraVerticalRotationMinAngle = serializedObject.FindProperty("cameraVerticalRotationMinAngle");
            cameraVerticalRotationMaxAngle = serializedObject.FindProperty("cameraVerticalRotationMaxAngle");
            cameraHorizontalRotation = serializedObject.FindProperty("cameraHorizontalRotation");
            cameraHorizontalRotationAuto = serializedObject.FindProperty("cameraHorizontalRotationAuto");
            cameraRotationSmoothing = serializedObject.FindProperty("cameraRotationSmoothing");
            cameraDistance = serializedObject.FindProperty("cameraDistance");
            cameraDistanceMin = serializedObject.FindProperty("cameraDistanceMin");
            cameraDistanceMax = serializedObject.FindProperty("cameraDistanceMax");
            cameraDistanceManaged = serializedObject.FindProperty("cameraDistanceManaged");
            cameraUseCustomDistanceWhenSplit = serializedObject.FindProperty("cameraUseCustomDistanceWhenSplit");
            cameraDistanceWhenSplit = serializedObject.FindProperty("cameraDistanceWhenSplit");
            cameraZoomDuration = serializedObject.FindProperty("cameraZoomDuration");
            splitAudio = serializedObject.FindProperty("splitAudio");
            downsampling = serializedObject.FindProperty("downsampling");
            hdr = serializedObject.FindProperty("hdr");
            showCameraIds = serializedObject.FindProperty("showCameraIds");
        }

        public override void OnInspectorGUI() {

            serializedObject.Update();

            EditorGUILayout.PropertyField(player1);
            EditorGUILayout.PropertyField(player2);
            EditorGUILayout.PropertyField(previewInEditMode);

            EditorGUILayout.PropertyField(splitMode, new GUIContent("Mode"));
            switch (splitMode.intValue) {
                case (int)SplitMode.Off:
                    EditorGUILayout.HelpBox("Split Screen Manager fully deactivated.", MessageType.Info);
                    break;
                case (int)SplitMode.SingleCameraOnPlayer1:
                    if (player1.objectReferenceValue == null) {
                        EditorGUILayout.HelpBox("Player 1 property must be assigned.", MessageType.Error);
                    } else {
                        EditorGUILayout.HelpBox("Split screen deactivated. Camera follows player 1.", MessageType.Info);
                    }
                    break;
                case (int)SplitMode.SingleCameraOnPlayer2:
                    if (player2.objectReferenceValue == null) {
                        EditorGUILayout.HelpBox("Player 2 property must be assigned.", MessageType.Error);
                    } else {
                        EditorGUILayout.HelpBox("Split screen deactivated. Camera follows player 2.", MessageType.Info);
                    }
                    break;
                case (int)SplitMode.SplitScreenFixedDivision:
                    EditorGUILayout.HelpBox("Split screen activated. Split line position, rotation and main/secondary cameras position/rotation managed by user.", MessageType.Info);
                    break;
                case (int)SplitMode.SplitScreenAutomatic:
                    if (player1.objectReferenceValue == null || player2.objectReferenceValue == null) {
                        EditorGUILayout.HelpBox("Player 1 and Player 2 properties must be assigned.", MessageType.Error);
                    } else {
                        EditorGUILayout.HelpBox("Split screen activated. Split line, main and secondary cameras managed automatically.", MessageType.Info);
                    }
                    break;
            }
            if (splitMode.intValue == (int)SplitMode.SplitScreenFixedDivision) {
                EditorGUILayout.PropertyField(splitLineAngle);
                EditorGUILayout.PropertyField(splitLineCenter);
            }
            EditorGUILayout.PropertyField(splitLineWidth);
            EditorGUILayout.PropertyField(splitLineColor);
            EditorGUILayout.PropertyField(splitLineFadeDuration);
            EditorGUILayout.PropertyField(maxScreenSeparation);

            EditorGUILayout.PropertyField(mainCamera);
            if (mainCamera.objectReferenceValue == null && splitMode.intValue != (int)SplitMode.Off) {
                EditorGUILayout.HelpBox("Main Camera property must be assigned.", MessageType.Error);
            }

            if (splitMode.intValue != (int)SplitMode.SplitScreenFixedDivision) {
                EditorGUILayout.PropertyField(cameraVerticalRotation, new GUIContent("Vertical Rotation"));
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(cameraVerticalRotationMinAngle, new GUIContent("Min Angle"));
                EditorGUILayout.PropertyField(cameraVerticalRotationMaxAngle, new GUIContent("Max Angle"));
                EditorGUI.indentLevel--;

                EditorGUILayout.PropertyField(cameraHorizontalRotation, new GUIContent("Horizontal Rotation"));
                if (splitMode.intValue == (int)SplitMode.SplitScreenAutomatic) {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(cameraHorizontalRotationAuto, new GUIContent("Automatic"));
                    EditorGUI.indentLevel--;
                }
                EditorGUILayout.PropertyField(cameraRotationSmoothing, new GUIContent("Rotation Smoothing"));

                EditorGUILayout.PropertyField(cameraDistance);
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(cameraDistanceMin, new GUIContent("Min Distance"));
                EditorGUILayout.PropertyField(cameraDistanceMax, new GUIContent("Max Distance"));
                EditorGUILayout.PropertyField(cameraDistanceManaged, new GUIContent("Auto-Manage Distance"));
                if (cameraDistanceManaged.boolValue) {
                    EditorGUILayout.PropertyField(cameraUseCustomDistanceWhenSplit, new GUIContent("Custom Distance When Split"));
                    if (cameraUseCustomDistanceWhenSplit.boolValue) {
                        EditorGUILayout.PropertyField(cameraDistanceWhenSplit, new GUIContent("Distance When Split"));
                        EditorGUILayout.PropertyField(cameraZoomDuration, new GUIContent("Zoom Duration"));
                    }
                }
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.PropertyField(splitAudio);
            if (splitAudio.boolValue && Application.isPlaying) {
                EditorGUILayout.HelpBox("Currently managing " + SplitScreenManager.splitAudioSources.Count + " audio sources.", MessageType.Info);
            }
            EditorGUILayout.PropertyField(downsampling);
            EditorGUILayout.PropertyField(hdr, new GUIContent("HDR"));
            EditorGUILayout.PropertyField(showCameraIds);


            serializedObject.ApplyModifiedProperties();
        }

        [MenuItem("GameObject/Split Screen Manager", priority = 20, validate = false)]
        static void AddSplitScreenManager() {
            SplitScreenManager i = SplitScreenManager.instance;
            if (i != null) {
                Selection.activeGameObject = i.gameObject;
                return;
            }
            GameObject p = Resources.Load<GameObject>("Prefabs/SplitScreenManager");
            GameObject o = Instantiate(p);
            Undo.RegisterCompleteObjectUndo(o, "Create Split Screen Manager");
            o.name = p.name;
            Selection.activeGameObject = o;
        }

    }
}
