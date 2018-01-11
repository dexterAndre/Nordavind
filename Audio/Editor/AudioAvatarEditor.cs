using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AudioAvatar))]
public class AudioAvatarEditor : Editor {

    public SerializedProperty mPrimaryAudioClips_P, mSecondaryAudioClips_P, mThirdAudioClips_P;

    private void OnEnable()
    {
        mPrimaryAudioClips_P = serializedObject.FindProperty("mPrimaryAudioClips");
        mSecondaryAudioClips_P = serializedObject.FindProperty("mSecondaryAudioClips");
        mThirdAudioClips_P = serializedObject.FindProperty("mThirdAudioClips");

    }

    public override void OnInspectorGUI()
    {
       // base.OnInspectorGUI();

        serializedObject.Update();



        GUILayout.Space(20);

        EditorGUILayout.BeginVertical(GUI.skin.box);
        GUILayout.Label("Primary Audio Clips", EditorStyles.boldLabel);
        EditorGUILayout.EndVertical();


        CreateWindowForPrimary();

        EditorGUILayout.BeginVertical(GUI.skin.box);
        GUILayout.Label("Secondary Audio Clips", EditorStyles.boldLabel);
        EditorGUILayout.EndVertical();


        CreateWindowForSecondary();

        EditorGUILayout.BeginVertical(GUI.skin.box);
        GUILayout.Label("Third Audio Clips", EditorStyles.boldLabel);
        EditorGUILayout.EndVertical();

        CreateWindowForThird();


        serializedObject.ApplyModifiedProperties();
    }

    private void CreateWindowForPrimary()
    {

        EditorGUILayout.BeginVertical("Window");
        GUILayout.Space(20);


        SerializedProperty PrimaryAudio = mPrimaryAudioClips_P.GetArrayElementAtIndex(0);
        EditorGUILayout.PropertyField(PrimaryAudio, new GUIContent("Footstep (Snow):"));

        GUILayout.Space(20);

        PrimaryAudio = mPrimaryAudioClips_P.GetArrayElementAtIndex(1);
        EditorGUILayout.PropertyField(PrimaryAudio, new GUIContent("Footstep (Gravel):"));

        GUILayout.Space(20);

        PrimaryAudio = mPrimaryAudioClips_P.GetArrayElementAtIndex(2);
        EditorGUILayout.PropertyField(PrimaryAudio, new GUIContent("Footstep (Ice):"));

        GUILayout.Space(20);

        PrimaryAudio = mPrimaryAudioClips_P.GetArrayElementAtIndex(3);
        EditorGUILayout.PropertyField(PrimaryAudio, new GUIContent("Footstep (Wood):"));

        GUILayout.Space(20);

        PrimaryAudio = mPrimaryAudioClips_P.GetArrayElementAtIndex(4);
        EditorGUILayout.PropertyField(PrimaryAudio, new GUIContent("Roll:"));

        GUILayout.Space(20);

        PrimaryAudio = mPrimaryAudioClips_P.GetArrayElementAtIndex(5);
        EditorGUILayout.PropertyField(PrimaryAudio, new GUIContent("Jump:"));

        GUILayout.Space(20);

        PrimaryAudio = mPrimaryAudioClips_P.GetArrayElementAtIndex(6);
        EditorGUILayout.PropertyField(PrimaryAudio, new GUIContent("Grab:"));

        GUILayout.Space(20);

        PrimaryAudio = mPrimaryAudioClips_P.GetArrayElementAtIndex(7);
        EditorGUILayout.PropertyField(PrimaryAudio, new GUIContent("Hanging Movement:"));

        GUILayout.Space(20);

        PrimaryAudio = mPrimaryAudioClips_P.GetArrayElementAtIndex(8);
        EditorGUILayout.PropertyField(PrimaryAudio, new GUIContent("Climb:"));

        GUILayout.Space(20);

        PrimaryAudio = mPrimaryAudioClips_P.GetArrayElementAtIndex(9);
        EditorGUILayout.PropertyField(PrimaryAudio, new GUIContent("Hit ground(after fall):"));

        GUILayout.Space(20);

        PrimaryAudio = mPrimaryAudioClips_P.GetArrayElementAtIndex(10);
        EditorGUILayout.PropertyField(PrimaryAudio, new GUIContent("Death by height:"));

        GUILayout.Space(20);

        PrimaryAudio = mPrimaryAudioClips_P.GetArrayElementAtIndex(11);
        EditorGUILayout.PropertyField(PrimaryAudio, new GUIContent("Death by combat:"));

        GUILayout.Space(20);

        PrimaryAudio = mPrimaryAudioClips_P.GetArrayElementAtIndex(12);
        EditorGUILayout.PropertyField(PrimaryAudio, new GUIContent("Bumslider(Snow):"));

        GUILayout.Space(20);

        PrimaryAudio = mPrimaryAudioClips_P.GetArrayElementAtIndex(13);
        EditorGUILayout.PropertyField(PrimaryAudio, new GUIContent("Bumslider(Ice):"));

        GUILayout.Space(20);

        PrimaryAudio = mPrimaryAudioClips_P.GetArrayElementAtIndex(14);
        EditorGUILayout.PropertyField(PrimaryAudio, new GUIContent("Bumslider(Gravel):"));

        GUILayout.Space(20);

        PrimaryAudio = mPrimaryAudioClips_P.GetArrayElementAtIndex(15);
        EditorGUILayout.PropertyField(PrimaryAudio, new GUIContent("Walking in deep snow:"));

        GUILayout.Space(20);


        EditorGUILayout.EndVertical();
    }

    private void CreateWindowForSecondary()
    {
        EditorGUILayout.BeginVertical("Window");
        GUILayout.Space(20);

        SerializedProperty SecondaryAudio = mSecondaryAudioClips_P.GetArrayElementAtIndex(0);
        EditorGUILayout.PropertyField(SecondaryAudio, new GUIContent("Throwing snowball:"));

        GUILayout.Space(20);

        SecondaryAudio = mSecondaryAudioClips_P.GetArrayElementAtIndex(1);
        EditorGUILayout.PropertyField(SecondaryAudio, new GUIContent("Shielding:"));

        GUILayout.Space(20);

        SecondaryAudio = mSecondaryAudioClips_P.GetArrayElementAtIndex(2);
        EditorGUILayout.PropertyField(SecondaryAudio, new GUIContent("Take damage:"));

        GUILayout.Space(20);

        SecondaryAudio = mSecondaryAudioClips_P.GetArrayElementAtIndex(3);
        EditorGUILayout.PropertyField(SecondaryAudio, new GUIContent("Acceleration(during bumslider or falling):"));



        GUILayout.Space(20);


        EditorGUILayout.EndVertical();
    }

    private void CreateWindowForThird()
    {
        EditorGUILayout.BeginVertical("Window");
        GUILayout.Space(20);

        SerializedProperty ThirdAudio = mThirdAudioClips_P.GetArrayElementAtIndex(0);
        EditorGUILayout.PropertyField(ThirdAudio, new GUIContent("Coveralls rubbing:"));

        GUILayout.Space(20);

        ThirdAudio = mSecondaryAudioClips_P.GetArrayElementAtIndex(1);
        EditorGUILayout.PropertyField(ThirdAudio, new GUIContent("Freezing(vocal/flavor sound):"));

        GUILayout.Space(20);

        ThirdAudio = mSecondaryAudioClips_P.GetArrayElementAtIndex(2);
        EditorGUILayout.PropertyField(ThirdAudio, new GUIContent("Low health breathing:"));


        GUILayout.Space(20);


        EditorGUILayout.EndVertical();
    }

}
