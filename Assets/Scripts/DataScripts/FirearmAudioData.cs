using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "FPS/Firearm Audio Data")]
public class FirearmAudioData : ScriptableObject
{
    public AudioClip shootingAudio;//攻击音频(后续可以增加消音器之类的的不同攻击音频效果)
    public AudioClip reloadAudio;//换弹音频
    public AudioClip reloadoutAudio;//丢弃弹夹换弹音频
}
