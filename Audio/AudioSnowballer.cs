using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSnowballer : SFXController {


    #region Snowballer Unique SFXs

    [Header("Snowballer Primary SFX")]
    [SerializeField]
    private int RoarIndex = 4;
    [SerializeField]
    private int ChargeIndex = 5;
    [SerializeField]
    private int AttackIndex = 6;

    /// <summary>
    /// This function will be called in a animation key frame to play the roar sound of the snowballer.
    /// </summary>
    public void Audio_Roar()
    {
        mPrimaryAudioSource.PlayOneShot(mPrimaryAudioClips[RoarIndex], 0.75f);
    }

    /// <summary>
    /// This function will be called in a animation key frame to play the charge sound of the snowballer.
    /// </summary>
    public void Audio_Charge()
    {
        mPrimaryAudioSource.PlayOneShot(mPrimaryAudioClips[ChargeIndex], 0.75f);
    }

    /// <summary>
    /// This function will be called in a animation key frame to play the attack sound of the snowballer.
    /// </summary>
    public void Audio_Attack()
    {
        mPrimaryAudioSource.PlayOneShot(mPrimaryAudioClips[AttackIndex], 0.75f);
    }

    #endregion

    #region Snowballer Secondary sounds

    [Header("Snowballer Secondary SFX")]
    [SerializeField]
    private int Secondary_SnowballerMumblingFirst_index = 0;
    [SerializeField]
    private int Secondary_SnowballerMumblingSecond_index = 1;
    [SerializeField]
    private int Secondary_SnowballerMumblingThird_index = 2;

    /// <summary>
    /// Used to create verbal sounds from this unit, uses the secondary AudioSource.
    /// </summary>
    public void Audio_SnowballerVerbalSound()
    {
        int tempRandNumber = Random.Range(0, 3);

        if (tempRandNumber == 0)
            mSecondaryAudioSource.PlayOneShot(mSecondaryAudioClips[Secondary_SnowballerMumblingFirst_index], 0.5f);
        else if (tempRandNumber == 0)
            mSecondaryAudioSource.PlayOneShot(mSecondaryAudioClips[Secondary_SnowballerMumblingSecond_index], 0.5f);
        else if (tempRandNumber == 0)
            mSecondaryAudioSource.PlayOneShot(mSecondaryAudioClips[Secondary_SnowballerMumblingThird_index], 0.5f);
        else
            print("tempRandNumber in the function Audio_SnowballerVerbalSound(), in the AudioSnowballer-Script was out of bounds");

    }
#endregion

    #region Update Functions

    private void Start()
    {
        GetSFXComponents();
    }

    #endregion

}
