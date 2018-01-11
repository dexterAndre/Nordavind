using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXController : MonoBehaviour {

    #region Audio
    /// <summary>
    /// This is where you will put in Audio Clips that will be played from this unit's actions.
    /// </summary>
    [Header("Audio files")]
    [SerializeField]
    protected AudioClip[] mAudioClips;

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
        mAudioSource.volume = volume;
        mCurrentVolume = volume;
    }

    /// <summary>
    /// Will mute all sound coming from this unit.
    /// </summary>
    protected void MuteThisUnit()
    {
        mCurrentVolume = mAudioSource.volume;
        mAudioSource.volume = 0f;
    }

    /// <summary>
    /// Will unmute all sound coming from this unit.
    /// </summary>
    protected void UnmuteThisUnit()
    {
        mAudioSource.volume = mCurrentVolume;
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
                return mAudioClips[0];
            else if (footHit.collider.tag == "Gravel")
                return mAudioClips[1];
            else if (footHit.collider.tag == "Ice")
                return mAudioClips[2];
            else if (footHit.collider.tag == "Wood")
                return mAudioClips[3];
            else
                print("No footstep mapped to this tag.");
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
            mAudioSource.PlayOneShot(tempClip, 0.5f);
        else
            print("No footstep mapped to this Texture.");
    }

    #endregion

    #region Components

    /// <summary>
    /// The audio source that every unit uses to send out the main SFXs.
    /// </summary>
    protected AudioSource mAudioSource = null;

    /// <summary>
    /// The audio source that every unit uses to send out the secondary SFXs.
    /// </summary>
    protected AudioSource mBonusAudioSource = null;

    protected void GetSFXComponents()
    {
        if (GetComponent<AudioSource>() != null)
            mAudioSource = GetComponent<AudioSource>();
        else if (this.transform.parent.GetComponent<AudioSource>() != null)
            mAudioSource = this.transform.parent.GetComponent<AudioSource>();
        else
            print("This unit has no audio source avaible. ###consider adding one or removing audio scripts###");

        if(this.transform.GetChild(0).GetComponent)
    }

    #endregion

}
