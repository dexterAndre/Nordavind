using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AudioHedgehog))]
public class AudioHedgehogEditor : Editor
{

    public SerializedProperty mPrimaryAudioClips_P, mSecondaryAudioClips_P;

    private void OnEnable()
    {
        mPrimaryAudioClips_P = serializedObject.FindProperty("mPrimaryAudioClips");
        mSecondaryAudioClips_P = serializedObject.FindProperty("mSecondaryAudioClips");

    }

    public override void OnInspectorGUI()
    {
         //base.OnInspectorGUI();

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
        EditorGUILayout.PropertyField(PrimaryAudio, new GUIContent("Roar:"));

        GUILayout.Space(20);

        PrimaryAudio = mPrimaryAudioClips_P.GetArrayElementAtIndex(5);
        EditorGUILayout.PropertyField(PrimaryAudio, new GUIContent("Attack:"));

        GUILayout.Space(20);

        EditorGUILayout.EndVertical();
    }

    private void CreateWindowForSecondary()
    {
        EditorGUILayout.BeginVertical("Window");
        GUILayout.Space(20);

        SerializedProperty SecondaryAudio = mSecondaryAudioClips_P.GetArrayElementAtIndex(0);
        EditorGUILayout.PropertyField(SecondaryAudio, new GUIContent("Voice mumbling(1):"));

        GUILayout.Space(20);

        SecondaryAudio = mSecondaryAudioClips_P.GetArrayElementAtIndex(1);
        EditorGUILayout.PropertyField(SecondaryAudio, new GUIContent("Voice mumbling(2):"));

        GUILayout.Space(20);

        SecondaryAudio = mSecondaryAudioClips_P.GetArrayElementAtIndex(2);
        EditorGUILayout.PropertyField(SecondaryAudio, new GUIContent("Voice mumbling(3):"));

        GUILayout.Space(20);

        SecondaryAudio = mSecondaryAudioClips_P.GetArrayElementAtIndex(3);
        EditorGUILayout.PropertyField(SecondaryAudio, new GUIContent("Charge:"));

        GUILayout.Space(20);

        SecondaryAudio = mSecondaryAudioClips_P.GetArrayElementAtIndex(4);
        EditorGUILayout.PropertyField(SecondaryAudio, new GUIContent("Voice mumbling(during attack):"));


        GUILayout.Space(20);


        EditorGUILayout.EndVertical();
    }

}
