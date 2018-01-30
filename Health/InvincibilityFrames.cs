using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvincibilityFrames : MonoBehaviour {

    [Range(0.1f, 0.5f)]
    [SerializeField]
    private float durationOfInvincibility = 0.1f;

    private bool isInvincible = false;

    private IEnumerator StartInvincibilityCorutine()
    {
        isInvincible = true;
        yield return new WaitForSeconds(durationOfInvincibility);
        isInvincible = false;
    }
    public void StartInvincibility()
    {
        StartCoroutine(StartInvincibilityCorutine());
    }

    public bool GetInvincibleState() { return isInvincible; }
}
