using System;
using UnityEngine;

namespace SplitScreenPro {

    [ExecuteAlways, RequireComponent(typeof(AudioSource))]
    public class SplitAudioSource : MonoBehaviour {

        [Tooltip("The original audio source. This audio source is muted so the mirrored audio source can be positioned elsewhere and play the sound from that position instead.")]
        public AudioSource audioSource;

        [Tooltip("The audio source that produces actual sound.")]
        public AudioSource mirrorAudioSource;

        const string AS_MIRROR_NAME = "Audio Source Mirror";

        private void OnEnable() {
            if (mirrorAudioSource == null) {
                GameObject m = new GameObject(AS_MIRROR_NAME, typeof(AudioSource));
                mirrorAudioSource = m.GetComponent<AudioSource>();
                mirrorAudioSource.playOnAwake = false;
                m.transform.SetParent(transform, false);
            }
        }

        private void Start() {
            if (audioSource == null) {
                audioSource = GetComponent<AudioSource>();
            }
            if (audioSource == null) return;
            audioSource.mute = true;

            SplitScreenManager.RegisterAudioSource(this);
        }

        private void OnDestroy() {
            SplitScreenManager.UnregisterAudioSource(this);
            if (mirrorAudioSource != null) {
                DestroyImmediate(mirrorAudioSource.gameObject);
            }
            if (audioSource != null) {
                audioSource.mute = false;
            }
        }

        public void SyncAudioSourceProperties() {
            mirrorAudioSource.bypassEffects = audioSource.bypassEffects;
            mirrorAudioSource.bypassListenerEffects = audioSource.bypassListenerEffects;
            mirrorAudioSource.bypassReverbZones = audioSource.bypassReverbZones;
            mirrorAudioSource.clip = audioSource.clip;
            mirrorAudioSource.dopplerLevel = audioSource.dopplerLevel;
            mirrorAudioSource.ignoreListenerPause = audioSource.ignoreListenerPause;
            mirrorAudioSource.ignoreListenerVolume = audioSource.ignoreListenerVolume;
            mirrorAudioSource.loop = audioSource.loop;
            mirrorAudioSource.maxDistance = audioSource.maxDistance;
            mirrorAudioSource.minDistance = audioSource.minDistance;
            mirrorAudioSource.outputAudioMixerGroup = audioSource.outputAudioMixerGroup;
            mirrorAudioSource.panStereo = audioSource.panStereo;
            mirrorAudioSource.pitch = audioSource.pitch;
            mirrorAudioSource.priority = audioSource.priority;
            mirrorAudioSource.reverbZoneMix = audioSource.reverbZoneMix;
            mirrorAudioSource.rolloffMode = audioSource.rolloffMode;
            mirrorAudioSource.spatialBlend = audioSource.spatialBlend;
            mirrorAudioSource.spatialize = audioSource.spatialize;
            mirrorAudioSource.spatializePostEffects = audioSource.spatializePostEffects;
            mirrorAudioSource.spread = audioSource.spread;
            mirrorAudioSource.time = audioSource.time;
            mirrorAudioSource.timeSamples = audioSource.timeSamples;
            mirrorAudioSource.velocityUpdateMode = audioSource.velocityUpdateMode;
            mirrorAudioSource.volume = audioSource.volume;
            audioSource.mute = true;
        }

    }

}