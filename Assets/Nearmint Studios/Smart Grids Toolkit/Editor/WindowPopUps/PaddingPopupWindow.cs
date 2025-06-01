namespace SmartGridsToolkit
{
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    /// A popup window for adjusting the cell size of a Grid2D object.
    /// </summary>
    public class PaddingPopupWindow : EditorWindow
    {
        private static bool isOpen = false;

        private int defaultPaddingWidth;
        private int defaultPaddingHeight;

        private int newWidthPadding;
        private int newHeightPadding;
        private Grid2DEditorBase gridEditor;

        // Warnings
        private bool showWarning = false;
        private string warningMessage = "";

        /// <summary>
        /// Cell size adjustment window.
        /// </summary>
        public static void ShowWindow(Grid2DEditorBase editor, int currentWidthPadding, int currentHeightPadding, int defaultWidthPadding, int defaultHeightPadding)
        {
            if (isOpen) return;

            PaddingPopupWindow window = ScriptableObject.CreateInstance<PaddingPopupWindow>();
            window.titleContent = new GUIContent("Change Padding");
            window.gridEditor = editor;
            window.newWidthPadding = currentWidthPadding;
            window.newHeightPadding = currentHeightPadding;
            window.defaultPaddingWidth = defaultWidthPadding;
            window.defaultPaddingHeight = defaultHeightPadding;
            window.minSize = new Vector2(300, 160);
            window.ShowUtility();

            isOpen = true;
        }

        private void OnDestroy()
        {
            isOpen = false;
        }

        private void OnGUI()
        {
            GUILayout.Space(10);

            #region Cell Width

            EditorGUILayout.BeginHorizontal();
            newWidthPadding = EditorGUILayout.IntField("Width Padding", newWidthPadding);

            EditorGUILayout.BeginHorizontal(GUILayout.Width(40)); // Adjust width as needed
            if (GUILayout.Button("▲", GUILayout.Width(20), GUILayout.Height(EditorGUIUtility.singleLineHeight)))
            {
                GUI.FocusControl(null);
                newWidthPadding++;
            }

            if (GUILayout.Button("▼", GUILayout.Width(20), GUILayout.Height(EditorGUIUtility.singleLineHeight)))
            {
                GUI.FocusControl(null);
                newWidthPadding--;
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndHorizontal();

            #endregion

            #region Cell Height

            EditorGUILayout.BeginHorizontal();
            newHeightPadding = EditorGUILayout.IntField("Height Padding", newHeightPadding);

            EditorGUILayout.BeginHorizontal(GUILayout.Width(40));
            if (GUILayout.Button("▲", GUILayout.Width(20), GUILayout.Height(EditorGUIUtility.singleLineHeight)))
            {
                GUI.FocusControl(null);
                newHeightPadding++;
            }

            if (GUILayout.Button("▼", GUILayout.Width(20), GUILayout.Height(EditorGUIUtility.singleLineHeight)))
            {
                GUI.FocusControl(null);
                newHeightPadding--;
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndHorizontal();

            #endregion

            #region Validation Logic - Warnings

            showWarning = false;
            warningMessage = "";

            if (newWidthPadding < 0)
            {
                showWarning = true;
                warningMessage = "Width padding cannot be less than 0.";
            }
            else if (newWidthPadding > 50)
            {
                showWarning = true;
                warningMessage = "Width padding cannot be greater than 50.";
            }

            if (newHeightPadding < 0)
            {
                showWarning = true;
                warningMessage = "Height padding cannot be less than 0.";
            }
            else if (newHeightPadding > 50)
            {
                showWarning = true;
                warningMessage = "Height padding cannot be greater than 50.";
            }

            GUILayout.Space(15);  // Add space between the fields and the buttons

            if (showWarning)
            {
                EditorGUILayout.HelpBox(warningMessage, MessageType.Warning);
            }

            #endregion

            #region Buttons
            EditorGUI.BeginDisabledGroup(showWarning);
            if (GUILayout.Button("Confirm", GUILayout.Height(25)))
            {
                gridEditor.SetPadding(newWidthPadding, newHeightPadding);
                Close();
            }
            EditorGUI.EndDisabledGroup();

            GUILayout.Space(3);

            GUIStyle resetButtonStyle = new GUIStyle(GUI.skin.button);
            resetButtonStyle.normal.textColor = new Color(0.96f, 0.93f, 0.88f);
            resetButtonStyle.normal.background = MakeTex(1, 1, new Color(0.286f, 0.035f, 0.035f));
            resetButtonStyle.fontStyle = FontStyle.Bold;

            if (GUILayout.Button("Reset to Defaults", resetButtonStyle, GUILayout.Height(25)))
            {
                GUI.FocusControl(null);
                newWidthPadding = defaultPaddingWidth;
                newHeightPadding = defaultPaddingHeight;
            }
            #endregion

            minSize = maxSize = new Vector2(300, showWarning ? 175 : 135);
        }

        private Texture2D MakeTex(int width, int height, Color col)
        {
            Color[] pix = new Color[width * height];
            for (int i = 0; i < pix.Length; i++)
            {
                pix[i] = col;
            }
            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();
            return result;
        }
    }
}

