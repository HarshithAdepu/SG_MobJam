namespace SmartGridsToolkit
{
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    /// A popup window for adjusting the cell size of a Grid2D object.
    /// </summary>
    public class CellSizePopupWindow : EditorWindow
    {
        private static bool isOpen = false;

        private int defaultCellWidth;
        private int defaultCellHeight;

        private int newCellWidth;
        private int newCellHeight;
        private Grid2DEditorBase gridEditor;

        // Warnings
        private bool showWarning = false;
        private string warningMessage = "";

        /// <summary>
        /// Cell size adjustment window.
        /// </summary>
        public static void ShowWindow(Grid2DEditorBase editor, int currentCellWidth, int currentCellHeight, int defaultWidth, int defaultHeight)
        {
            if (isOpen) return;

            CellSizePopupWindow window = ScriptableObject.CreateInstance<CellSizePopupWindow>();
            window.titleContent = new GUIContent("Change Cell Size");
            window.gridEditor = editor;
            window.newCellWidth = currentCellWidth;
            window.newCellHeight = currentCellHeight;
            window.defaultCellWidth = defaultWidth;
            window.defaultCellHeight = defaultHeight;
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
            newCellWidth = EditorGUILayout.IntField("Cell Width", newCellWidth);

            EditorGUILayout.BeginHorizontal(GUILayout.Width(40)); // Adjust width as needed
            if (GUILayout.Button("▲", GUILayout.Width(20), GUILayout.Height(EditorGUIUtility.singleLineHeight)))
            {
                GUI.FocusControl(null);
                newCellWidth++;
            }

            if (GUILayout.Button("▼", GUILayout.Width(20), GUILayout.Height(EditorGUIUtility.singleLineHeight)))
            {
                GUI.FocusControl(null);
                newCellWidth--;
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndHorizontal();

            #endregion

            #region Cell Height

            EditorGUILayout.BeginHorizontal();
            newCellHeight = EditorGUILayout.IntField("Cell Height", newCellHeight);

            EditorGUILayout.BeginHorizontal(GUILayout.Width(40));
            if (GUILayout.Button("▲", GUILayout.Width(20), GUILayout.Height(EditorGUIUtility.singleLineHeight)))
            {
                GUI.FocusControl(null);
                newCellHeight++;
            }

            if (GUILayout.Button("▼", GUILayout.Width(20), GUILayout.Height(EditorGUIUtility.singleLineHeight)))
            {
                GUI.FocusControl(null);
                newCellHeight--;
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndHorizontal();

            #endregion

            #region Validation Logic - Warnings

            showWarning = false;
            warningMessage = "";

            if (newCellWidth < 20)
            {
                showWarning = true;
                warningMessage = "Width cannot be less than 20.";
            }
            else if (newCellWidth > 100)
            {
                showWarning = true;
                warningMessage = "Width cannot be greater than 100.";
            }

            if (newCellHeight < 20)
            {
                showWarning = true;
                warningMessage = "Height cannot be less than 20.";
            }
            else if (newCellHeight > 100)
            {
                showWarning = true;
                warningMessage = "Height cannot be greater than 50.";
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
                gridEditor.SetCellSize(newCellWidth, newCellHeight);
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
                newCellWidth = defaultCellWidth;
                newCellHeight = defaultCellHeight;
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

