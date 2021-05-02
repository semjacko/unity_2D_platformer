using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffect : MonoBehaviour
{

    public AudioClip hurt;
    public AudioClip heal;
    public AudioClip attack1;
    public AudioClip attack2;
    private AudioSource audioSrc;


    void Awake()
    {
        this.audioSrc = this.gameObject.GetComponent<AudioSource>();
    }


    public void PlayHurt()
    {
        this.audioSrc.PlayOneShot(this.hurt);
    }


    public void PlayHeal()
    {
        this.audioSrc.PlayOneShot(this.heal);
    }


    public void PlayAttack(int attackNumber)
    {
        if (attackNumber == 1)
            this.audioSrc.PlayOneShot(this.attack1);
        else
            this.audioSrc.PlayOneShot(this.attack2);
    }

}
