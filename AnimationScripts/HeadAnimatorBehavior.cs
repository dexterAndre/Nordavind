using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadAnimatorBehavior : MonoBehaviour
{

    protected Animator mAnimator = null;

    protected void GetHeadAnimtorComponents()
    {
        if (GetComponent<Animator>() != null)
            mAnimator = GetComponent<Animator>();
    }
}