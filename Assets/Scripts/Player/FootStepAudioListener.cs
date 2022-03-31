using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootStepAudioListener : MonoBehaviour
{
    public FootStepAudioData footAudioData;
    public Transform rayTransform;
    AudioSource audioSource;
    CharacterController characterController;
    PlayerController playerController;
    float nextPlayTime;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        characterController = GetComponent<CharacterController>();
        playerController = GetComponent<PlayerController>();
    }

    private void Update()
    {
        rayTransform.localPosition = new Vector3(rayTransform.localPosition.x, -characterController.height / 2 - 0.05f, rayTransform.localPosition.z);

        if (playerController.isGrounded())
        {
            //在移动的情况下
            if(characterController.velocity.magnitude >= 0.1f)
            {
                nextPlayTime += Time.deltaTime;

                //检测是否触碰到东西
                bool isHit = Physics.Linecast(rayTransform.position,rayTransform.position + Vector3.down * 0.15f, out RaycastHit hitInfo);

                //触碰到就播放声音，检测碰撞到的tag来播放不同的声音
                if (isHit)
                {
                    foreach (var audio in footAudioData.footStepAudios)
                    {
                        if(hitInfo.collider.CompareTag(audio.tag))
                        {
                            //根据不同的速度 来替换不同的播放频率
                            float temp_delay;
                            if(characterController.velocity.magnitude > 7)
                            {
                                temp_delay = audio.sprintingDelay;
                            }else if(characterController.velocity.magnitude > 3)
                            {
                                temp_delay = audio.delay;
                            }else if(characterController.velocity.magnitude > 1)
                            {
                                temp_delay = audio.crouchDelay;
                            }
                            else
                            {
                                temp_delay = audio.delay;
                            }

                            if(nextPlayTime > temp_delay)
                            {
                                nextPlayTime = 0;
                                AudioClip temp_clip = audio.AudioClips[Random.Range(0, audio.AudioClips.Count)];
                                audioSource.clip = temp_clip;
                                audioSource.Play();
                                break;
                            }
                        }
                    }
                }
            }
        }
    }
}
