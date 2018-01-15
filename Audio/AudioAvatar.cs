﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioAvatar : SFXController {

    #region Player Primary SFX

    public void Audio_WalkingInDeepSnow()
    {
        mPrimaryAudioSource.PlayOneShot(mPrimaryAudioClips[15], 0.5f);
    }

    public void Audio_Roll()
    {
        mPrimaryAudioSource.PlayOneShot(mPrimaryAudioClips[4], 0.5f);
    }

    public void Audio_Jump()
    {
        mPrimaryAudioSource.PlayOneShot(mPrimaryAudioClips[5], 0.5f);
    }

    public void Audio_Grab()
    {
        mPrimaryAudioSource.PlayOneShot(mPrimaryAudioClips[6], 0.5f);
    }

    public void Audio_LedgeMovement()
    {
        mPrimaryAudioSource.PlayOneShot(mPrimaryAudioClips[7], 0.5f);
    }

    public void Audio_HitGround()
    {
        mPrimaryAudioSource.PlayOneShot(mPrimaryAudioClips[9], 0.5f);
    }

    public void Audio_Climb()
    {
        mPrimaryAudioSource.PlayOneShot(mPrimaryAudioClips[8], 0.5f);
    }

    public void Audio_FallFromHeightDeath()
    {
        mPrimaryAudioSource.PlayOneShot(mPrimaryAudioClips[10], 0.5f);
    }

    public void Audio_DyingFromCombat()
    {
        mPrimaryAudioSource.PlayOneShot(mPrimaryAudioClips[11], 0.5f);
    }

    //This needs a lot of work, fix footsteps first and reuse the code to make this work better.
    public void Audio_Bumslider()
    {
        RaycastHit footHit;
        if (Physics.Raycast(transform.position, -Vector3.up, out footHit, 5f))
        {
            //Recreate to read the texture under your feet instead of using tags.
            if (footHit.collider.tag == "Snow")
                mPrimaryAudioSource.PlayOneShot(mPrimaryAudioClips[12]);
            else if (footHit.collider.tag == "Ice")
                mPrimaryAudioSource.PlayOneShot(mPrimaryAudioClips[13]);
            else if (footHit.collider.tag == "Gravel")
                mPrimaryAudioSource.PlayOneShot(mPrimaryAudioClips[14]);
            else
            {
                print("No material texture map detected. ###Snow used instead###");
                mPrimaryAudioSource.PlayOneShot(mPrimaryAudioClips[12]);
            }

        }
    }


    #endregion

    #region Player Secondary SFX


    public void Audio_ThrowingSnowball()
    {
        mSecondaryAudioSource.PlayOneShot(mSecondaryAudioClips[0], 0.5f);
    }

    public void Audio_Shielding()
    {
        mSecondaryAudioSource.PlayOneShot(mSecondaryAudioClips[1], 0.5f);
    }

    public void Audio_GetHit()
    {
        mSecondaryAudioSource.PlayOneShot(mSecondaryAudioClips[2], 0.5f);
    }

    public void Audio_VelocityIncrease() //used by falling down from heights and bumslider.
    {
        mSecondaryAudioSource.PlayOneShot(mSecondaryAudioClips[3], 0.5f);
    }


    #endregion

    #region Player Third SFX

    /// <summary>
    /// Because this is the player/avatar, it needs 3 AudioSources to create the most immersive feeling during gameplay.
    /// <para>  This should not be used all the time, but more flavor SFX.</para>
    /// </summary>
    [SerializeField]
    private AudioSource mThirdAudioSource = null;

    /// <summary>
    /// List of all the Audio Clips for the third Audio Source.
    /// </summary>
    [SerializeField]
    private AudioClip[] mThirdAudioClips = new AudioClip[3];

    private void Audio_CoverallsRubbing()
    {
        mThirdAudioSource.PlayOneShot(mThirdAudioClips[0], 0.5f);
    }

    public void Audio_LongIdleEvent()
    {
        mThirdAudioSource.PlayOneShot(mThirdAudioClips[1], 0.5f);
    }

    public void Audio_LowHealthBreathing()
    {
        mThirdAudioSource.PlayOneShot(mThirdAudioClips[2], 0.5f);
    }


    #endregion

    #region Update Functions

    private void Start()
    {
        GetSFXComponents();

        //Gets the third child of the Audio-child.
        if(transform.parent.GetChild(3).transform.GetChild(2).GetComponent<AudioSource>() != null)
            mThirdAudioSource = transform.parent.GetChild(3).transform.GetChild(2).GetComponent<AudioSource>();
    }

    #endregion
}
