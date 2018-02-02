using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioHedgehog : SFXController {


    #region Snowballer Unique SFXs

    /// <summary>
    /// This function will be called in a animation key frame to play the roar sound of the snowballer.
    /// </summary>
    public void Audio_Roar()
    {
        mPrimaryAudioSource.PlayOneShot(mPrimaryAudioClips[4], 0.75f);
    }


    /// <summary>
    /// This function will be called in a animation key frame to play the attack sound of the snowballer.
    /// </summary>
    public void Audio_Attack()
    {
        mPrimaryAudioSource.PlayOneShot(mPrimaryAudioClips[5], 0.75f);
    }

    #endregion

    #region Snowballer Secondary sounds
    /// <summary>
    /// Used to create verbal sounds from this unit, uses the secondary AudioSource.
    /// </summary>
    public void Audio_SnowballerVerbalSound()
    {
        int tempRandNumber = Random.Range(0, 3);

        if (tempRandNumber == 0)
            mSecondaryAudioSource.PlayOneShot(mSecondaryAudioClips[0], 0.5f);
        else if (tempRandNumber == 0)
            mSecondaryAudioSource.PlayOneShot(mSecondaryAudioClips[1], 0.5f);
        else if (tempRandNumber == 0)
            mSecondaryAudioSource.PlayOneShot(mSecondaryAudioClips[2], 0.5f);
        else
        {
            mSecondaryAudioSource.PlayOneShot(mSecondaryAudioClips[0], 0.5f);
            print("tempRandNumber in the function Audio_SnowballerVerbalSound(), in the AudioSnowballer-Script was out of bounds");
        }
            

    }

    /// <summary>
    /// This function will be called in a animation key frame to play the charge sound of the snowballer.
    /// </summary>
    public void Audio_Charge()
    {
        mSecondaryAudioSource.PlayOneShot(mPrimaryAudioClips[3], 0.75f);
    }

    public void Audio_AttackMumbling()
    {
        mSecondaryAudioSource.PlayOneShot(mPrimaryAudioClips[4], 0.75f);
    }

    #endregion

    #region Update Functions

    private void Start()
    {
        if (transform.parent.GetChild(3).GetComponent<AudioSource>() != null)
            mPrimaryAudioSource = transform.parent.GetChild(3).GetComponent<AudioSource>();
        else
            print("This unit has no audio source avaible. ###consider adding one or removing audio scripts###");

        //Checks for child number two in child 2 (audio-child).
        if (transform.parent.GetChild(3).transform.GetChild(0).GetComponent<AudioSource>() != null)
            mSecondaryAudioSource = transform.parent.GetChild(3).transform.GetChild(0).GetComponent<AudioSource>();
    }

    #endregion

}
