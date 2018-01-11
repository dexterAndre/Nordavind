using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSnowballer : SFXController {


    public void Audio_Roar()
    {
        mAudioSource.PlayOneShot(mAudioClips[4], 0.75f);
    }

    public void Audio_Charge()
    {
        mAudioSource.PlayOneShot(mAudioClips[5], 0.75f);
    }

    public void Audio_Attack()
    {
        mAudioSource.PlayOneShot(mAudioClips[6], 0.75f);
    }

}
