using System.Collections.Generic;
using UnityEngine;

namespace SplitScreenPro {

    public partial class SplitScreenManager : MonoBehaviour {

        public readonly static List<SplitAudioSource> splitAudioSources = new List<SplitAudioSource>();

        public static void RegisterAudioSource(SplitAudioSource audioSource) {
            if (audioSource != null && !splitAudioSources.Contains(audioSource)) splitAudioSources.Add(audioSource);
        }

        public static void UnregisterAudioSource(SplitAudioSource audioSource) {
            if (audioSource != null && splitAudioSources.Contains(audioSource)) splitAudioSources.Remove(audioSource);
        }

        public static void PlayClipAtPoint(AudioClip clip, Vector3 position, float volume = 1f) {

            SplitScreenManager ssp = instance;
            if (ssp == null || ssp.splitMode == SplitMode.Off || ssp.player1 == null || ssp.player2 == null) {
                AudioSource.PlayClipAtPoint(clip, position, volume);
                return;
            }

            position = AdjustPlayPosition(ssp, position);
            AudioSource.PlayClipAtPoint(clip, position, volume);
        }

        static Vector3 AdjustPlayPosition(SplitScreenManager ssp, Vector3 soundPosition) { 
            // Which player is nearer?
            Vector3 middlePlayerPosition = (ssp.player1.position + ssp.player2.position) * 0.5f;
            float distToMiddlePosiiton = Vector3.SqrMagnitude(soundPosition - middlePlayerPosition);
            float distToPlayer1 = Vector3.SqrMagnitude(soundPosition - ssp.player1.position);
            float distToPlayer2 = Vector3.SqrMagnitude(soundPosition - ssp.player2.position);

            if (distToMiddlePosiiton < distToPlayer1 && distToMiddlePosiiton < distToPlayer2) {
                // Convert position to local middle position
                soundPosition -= middlePlayerPosition - ssp.player1.transform.position;
                return soundPosition;
            }

            // If nearer to player 1, play from his perspective
            if (distToPlayer1 <= distToPlayer2) {
                return soundPosition;
            }

            // Convert position to local space of player 2
            soundPosition -= ssp.player2.transform.position - ssp.player1.transform.position;
            return soundPosition;
        }


        void UpdateSplitAudioSources() {
            int asCount = splitAudioSources.Count;
            for (int k = 0; k < asCount; k++) {
                SplitAudioSource sas = splitAudioSources[k];
                if (sas == null || !sas.isActiveAndEnabled || sas.audioSource == null || sas.mirrorAudioSource == null) continue;

                // check play state
                if (sas.audioSource.isPlaying != sas.mirrorAudioSource.isPlaying) {
                    if (sas.audioSource.isPlaying) {
                        UpdateSplitAudioSourcePosition(sas, sas.audioSource.transform.position);
                        sas.SyncAudioSourceProperties();
                        sas.mirrorAudioSource.Play();
                    } else {
                        sas.mirrorAudioSource.Stop();
                    }
                } else if (sas.audioSource.isPlaying) {
                    UpdateSplitAudioSourcePosition(sas, sas.audioSource.transform.position);
                }
            }
        }

        void UpdateSplitAudioSourcePosition(SplitAudioSource sas, Vector3 soundPosition) {
            Vector3 playPosition = AdjustPlayPosition(this, soundPosition);
            sas.mirrorAudioSource.transform.position = playPosition;
        }

    }

}