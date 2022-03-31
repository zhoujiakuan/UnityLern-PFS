using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "FPS/Foot Step Audio Data")]
public class FootStepAudioData : ScriptableObject
{
    public List<FootStepAudio> footStepAudios = new List<FootStepAudio>();
}


[System.Serializable]
public class FootStepAudio
{
    //通过判断不同的tag来播放不同的音效，例如tag为草地，就播放踩在草地的声音
    public string tag;
    //存储声音片段的列表
    public List<AudioClip> AudioClips = new List<AudioClip>();
    //声音的延迟
    public float delay = 0.4f;
    public float sprintingDelay = 0.3f;
    public float crouchDelay = 0.5f;
}
