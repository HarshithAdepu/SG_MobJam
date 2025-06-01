namespace SmartGridsToolkit
{
    using UnityEngine;
    using UnityEditor;

    /// <summary>
    /// A popup window for adjusting the grid size of a Grid2D object.
    /// </summary>
    public class GridSizePopupWindow : EditorWindow
    {
        private static bool isOpen = false;

        private int newWidth;
        private int newHeight;
        private Grid2DEditorBase gridEditor;

        // Warnings
        private bool showWarning = false;
        private string warningMessage = "";

        /// <summary>
        /// Opens the grid size adjustment window.
        /// </summary>
        public static void ShowWindow(Grid2DEditorBase editor, int currentWidth, int currentHeight)
        {
            if (isOpen) return;

            GridSizePopupWindow window = ScriptableObject.CreateInstance<GridSizePopupWindow>();
            window.titleContent = new GUIContent("Update Grid Size");
            window.gridEditor = editor;
            window.newWidth = currentWidth;
            window.newHeight = currentHeight;
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

            #region Width Input Field

            EditorGUILayout.BeginHorizontal();
            newWidth = EditorGUILayout.IntField("Width", newWidth, GUILayout.Height(EditorGUIUtility.singleLineHeight));

            EditorGUILayout.BeginHorizontal(GUILayout.Width(40));
            if (GUILayout.Button("▲", GUILayout.Width(20), GUILayout.Height(EditorGUIUtility.singleLineHeight)))
            {
                GUI.FocusControl(null);
                newWidth++;
            }
            if (GUILayout.Button("▼", GUILayout.Width(20), GUILayout.Height(EditorGUIUtility.singleLineHeight)))
            {
                GUI.FocusControl(null);
                newWidth = Mathf.Max(1, newWidth - 1);
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndHorizontal();

            #endregion

            #region Height Input Field

            EditorGUILayout.BeginHorizontal();
            newHeight = EditorGUILayout.IntField("Height", newHeight, GUILayout.Height(EditorGUIUtility.singleLineHeight));

            EditorGUILayout.BeginHorizontal(GUILayout.Width(40));
            if (GUILayout.Button("▲", GUILayout.Width(20), GUILayout.Height(EditorGUIUtility.singleLineHeight)))
            {
                GUI.FocusControl(null);
                newHeight++;
            }
            if (GUILayout.Button("▼", GUILayout.Width(20), GUILayout.Height(EditorGUIUtility.singleLineHeight)))
            {
                GUI.FocusControl(null);
                newHeight = Mathf.Max(1, newHeight - 1);
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndHorizontal();

            #endregion

            #region Validation Logic - Warnings

            showWarning = false;
            warningMessage = "";

            if (newWidth < 1)
            {
                showWarning = true;
                warningMessage = "Width cannot be less than 1.";
            }
            else if (newWidth > 100)
            {
                showWarning = true;
                warningMessage = "Width cannot be greater than 100.";
            }

            if (newHeight < 1)
            {
                showWarning = true;
                warningMessage = "Height cannot be less than 1.";
            }
            else if (newHeight > 100)
            {
                showWarning = true;
                warningMessage = "Height cannot be greater than 100.";
            }

            GUILayout.Space(15);

            if (showWarning)
            {
                EditorGUILayout.HelpBox(warningMessage, MessageType.Warning);
            }

            #endregion

            #region Buttons

            EditorGUI.BeginDisabledGroup(showWarning);
            if (GUILayout.Button("Confirm", GUILayout.Height(25)))
            {
                gridEditor.ResizeGridAndPreserveValues(newWidth, newHeight);
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
                newWidth = 3;
                newHeight = 3;
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
