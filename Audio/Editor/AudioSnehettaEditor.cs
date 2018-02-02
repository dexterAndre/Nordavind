using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AudioSnehetta))]
public class AudioSnehettaEditor : Editor
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
        EditorGUILayout.PropertyField(PrimaryAudio, new GUIContent("Spawning sound:"));

        GUILayout.Space(20);

        PrimaryAudio = mPrimaryAudioClips_P.GetArrayElementAtIndex(1);
        EditorGUILayout.PropertyField(PrimaryAudio, new GUIContent("Take damage:"));

        GUILayout.Space(20);

        PrimaryAudio = mPrimaryAudioClips_P.GetArrayElementAtIndex(2);
        EditorGUILayout.PropertyField(PrimaryAudio, new GUIContent("Roar:"));

        GUILayout.Space(20);

        PrimaryAudio = mPrimaryAudioClips_P.GetArrayElementAtIndex(3);
        EditorGUILayout.PropertyField(PrimaryAudio, new GUIContent("Laughter:"));

        GUILayout.Space(20);


        EditorGUILayout.EndVertical();
    }

    private void CreateWindowForSecondary()
    {
        EditorGUILayout.BeginVertical("Window");
        GUILayout.Space(20);

        SerializedProperty SecondaryAudio = mSecondaryAudioClips_P.GetArrayElementAtIndex(0);
        EditorGUILayout.PropertyField(SecondaryAudio, new GUIContent("Crystal Sound:"));

        GUILayout.Space(20);



        GUILayout.Space(20);


        EditorGUILayout.EndVertical();
    }

}
