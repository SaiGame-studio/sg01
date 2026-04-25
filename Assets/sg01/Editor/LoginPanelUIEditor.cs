using UnityEditor;
using UnityEngine;

namespace Sg01.UI.Editor
{
    [CustomEditor(typeof(LoginPanelUI))]
    public class LoginPanelUIEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            // Draw all default fields first.
            this.DrawDefaultInspector();

            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Debug Tools", EditorStyles.boldLabel);

            bool isPlaying = Application.isPlaying;

            GUI.enabled = isPlaying;

            Color prev = GUI.backgroundColor;
            GUI.backgroundColor = isPlaying
                ? new Color(0.3f, 0.8f, 0.4f)   // green — available
                : new Color(0.5f, 0.5f, 0.5f);   // grey  — disabled

            if (GUILayout.Button("Load Credentials from SaiAuth", GUILayout.Height(32)))
            {
                LoginPanelUI panel = (LoginPanelUI)this.target;
                panel.LoadCredentialsFromAuth();
                Debug.Log("[LoginPanelUI] Credentials loaded from SaiAuth into UI fields.");
            }

            GUI.backgroundColor = prev;
            GUI.enabled = true;

            if (!isPlaying)
            {
                EditorGUILayout.HelpBox(
                    "Enter Play Mode to use this button.",
                    MessageType.Info);
            }
        }
    }
}
