using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXController : MonoBehaviour {

    #region Global indices

    [SerializeField]
    protected int SnowStepIndex = 0;
    [SerializeField]
    protected int GravelStepIndex = 1;
    [SerializeField]
    protected int IceStepIndex = 2;
    [SerializeField]
    protected int WoodStepIndex = 3;

    #endregion

    #region Audio
    /// <summary>
    /// This is where you will put in Primary Audio Clips that will be played from this unit's actions.
    /// </summary>
    [Header("Audio files")]
    [SerializeField]
    protected AudioClip[] mAudioClips;

    /// <summary>
    /// This is where you will put in Secondary Audio Clips that will be played from this unit's actions.
    /// </summary>
    [SerializeField]
    protected AudioClip[] mSecondaryAudioClips;

    /// <summary>
    /// Saves the current volume, especially important while muted so you know what volume to put back on when unmuted.
    /// </summary>
    protected float mCurrentVolume = 0f;

    /// <summary>
    /// This will set the volume of the unit's master volume.
    /// </summary>
    /// <param name="volume"></param>
    protected void SetUnitsMasterVolume(float volume)
    {
        mPrimaryAudioSource.volume = volume;
        mCurrentVolume = volume;
    }

    /// <summary>
    /// Will mute all sound coming from this unit.
    /// </summary>
    protected void MuteThisUnit()
    {
        mCurrentVolume = mPrimaryAudioSource.volume;
        mPrimaryAudioSource.volume = 0f;
    }

    /// <summary>
    /// Will unmute all sound coming from this unit.
    /// </summary>
    protected void UnmuteThisUnit()
    {
        mPrimaryAudioSource.volume = mCurrentVolume;
    }
    #endregion

    #region FootSteps

    protected AudioClip Audio_ReturnCorrectAudioClipForStep()
    {
        RaycastHit footHit;
        if (Physics.Raycast(transform.position, -Vector3.up, out footHit, 5f))
        {
            //Recreate to read the texture under your feet instead of using tags.
            if (footHit.collider.tag == "Snow")
                return mAudioClips[SnowStepIndex];
            else if (footHit.collider.tag == "Gravel")
                return mAudioClips[GravelStepIndex];
            else if (footHit.collider.tag == "Ice")
                return mAudioClips[IceStepIndex];
            else if (footHit.collider.tag == "Wood")
                return mAudioClips[WoodStepIndex];
            else
            {
                print("No footstep mapped to this tag. ###Snow used instead###");
                return mAudioClips[SnowStepIndex];
            }
                
        }

        return null;
    }

    /// <summary>
    /// This function will figure out what type of ground you are walking on, and then play the correct sound to match this.
    /// </summary>
    public void Audio_PlayFootStep()
    {
        AudioClip tempClip = Audio_ReturnCorrectAudioClipForStep();

        if (tempClip != null)
            mPrimaryAudioSource.PlayOneShot(tempClip, 0.5f);
        else
            print("No footstep mapped to this Texture.");
    }

    #endregion

    #region Components

    /// <summary>
    /// The audio source that every unit uses to send out the main SFXs.
    /// </summary>
    protected AudioSource mPrimaryAudioSource = null;

    /// <summary>
    /// The audio source that every unit uses to send out the secondary SFXs.
    /// </summary>
    protected AudioSource mSecondaryAudioSource = null;

    /// <summary>
    /// This function is used to set the SFX components in the each individual script.
    /// </summary>
    protected void GetSFXComponents()
    {
        if (GetComponent<AudioSource>() != null)
            mPrimaryAudioSource = GetComponent<AudioSource>();
        else
            print("This unit has no audio source avaible. ###consider adding one or removing audio scripts###");

        if (this.transform.GetChild(0).GetComponent<AudioSource>() != null)
            mSecondaryAudioSource = this.transform.GetChild(0).GetComponent<AudioSource>();

    }

    #endregion

}
