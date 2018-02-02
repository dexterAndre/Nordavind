using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSnehetta : SFXController {

    #region Primary Sounds
    public void Audio_SpawningSound()
    {
        mPrimaryAudioSource.PlayOneShot(mPrimaryAudioClips[0], 0.75f);
    }

    public void Audio_TakeDamage()
    {
        mPrimaryAudioSource.Stop();
        mPrimaryAudioSource.PlayOneShot(mPrimaryAudioClips[1], 0.75f);
    }

    public void Audio_Roar()
    {
        mPrimaryAudioSource.PlayOneShot(mPrimaryAudioClips[2], 0.75f);
    }

    public void Audio_Laughter()
    {
        mPrimaryAudioSource.PlayOneShot(mPrimaryAudioClips[3], 0.75f);
    }

    #endregion

    #region Secondary
    public void Audio_CrystalSound()
    {
        mSecondaryAudioSource.PlayOneShot(mSecondaryAudioClips[0], 0.75f);
    }
    #endregion

    #region Update Functions
    void Start()
    {
        GetSFXComponents();
    }
    #endregion
}
