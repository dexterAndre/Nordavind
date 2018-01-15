using UnityEngine;
using System.Collections;
using Cinemachine;

public class ThirdPersonRigBehavior : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private CinemachineFreeLook mFreeLook;
    [SerializeField]
    private GameObject mPlayer = null;
    [SerializeField]
    private GameObject mStandardFocus = null;
    //[SerializeField]
    //private GameObject mThrowAimPosition = null;
    [SerializeField]
    private GameObject mThrowAimFocus = null;
    //[SerializeField]
    //private GameObject mThrowFreePosition = null;
    //[SerializeField]
    //private GameObject mThrowFreeFocus = null;

    private void Start ()
	{
        mFreeLook = GetComponent<CinemachineFreeLook>();

        mPlayer = GameObject.FindGameObjectWithTag("Player");
        // Gets the default focus object
        mStandardFocus = mPlayer.transform.GetChild(3).transform.GetChild(0).gameObject;
        //// Gets the aim mode camera position
        //mThrowAimPosition = mPlayer.transform.GetChild(3).transform.GetChild(1).transform.GetChild(0).gameObject;
        //// Gets the aim mode camera focus object
        //mThrowAimFocus = mThrowAimPosition.transform.GetChild(0).gameObject;
        mThrowAimFocus =
            mPlayer.transform.GetChild(3)
            .transform.GetChild(1)
            .transform.GetChild(0)
            .transform.GetChild(0).gameObject;

        // Actually setting the values
        mFreeLook.Follow = mPlayer.transform;
        mFreeLook.LookAt = mStandardFocus.transform;
        mFreeLook.GetRig(0).m_LookAt = mThrowAimFocus.transform;
    }

	private void Update ()
	{

	}
}