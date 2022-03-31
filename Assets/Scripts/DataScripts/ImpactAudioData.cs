using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="FPS/Impact Audio Data")]
public class ImpactAudioData : ScriptableObject
{
    public List<ImpactWithTagAudio> ImpactAudios = new List<ImpactWithTagAudio>();
    

    [System.Serializable]
    public class ImpactWithTagAudio
    {
        public string tag;
        public List<AudioClip> AudioClips = new List<AudioClip>();
    }
}
