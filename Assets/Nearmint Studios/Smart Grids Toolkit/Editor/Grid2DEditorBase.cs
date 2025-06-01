namespace SmartGridsToolkit
{
    using UnityEngine;
    using UnityEditor;
    using System;

    public class Grid2DEditorBase : PropertyDrawer
    {
        /// <summary>
        /// Class to hold copied grid data for pasting.
        /// </summary>
        private class CopiedGridData
        {
            public object[][] data;
            public System.Type gridType;
            public int width;
            public int height;

            public CopiedGridData(object[][] data, System.Type gridType, int width, int height)
            {
                this.data = data;
                this.gridType = gridType;
                this.width = width;
                this.height = height;
            }
        }

        private static CopiedGridData copiedGridData = null;

        // Variables for cell dimensions and padding
        private int CELL_HEIGHT;
        private int CELL_WIDTH;
        private int PROPERTY_PADDING_W;
        private int PROPERTY_PADDING_H;
        private const int BUTTON_HEIGHT = 25;
        private const int INITIAL_SPACING = 8;

        private SerializedProperty thisProperty;
        private SerializedProperty gridProperty;
        private SerializedProperty widthCountProperty;
        private SerializedProperty heightCountProperty;

        private int defaultCellWidth;
        private int defaultCellHeight;
        private int defaultPaddingWidth;
        private int defaultPaddingHeight;

        private bool isInitialized = false;
        //Grid Parent Foldout State
        private bool mainFoldout = true;

        // Scroll view variables
        private Vector2 scrollPosition = Vector2.zero;

        /// <summary>
        /// Sets the default visual properties (cell width, height, and padding) based on the grid type.
        /// </summary>
        private void SetDefaultVisualProperties(SerializedProperty property)
        {
            if (!isInitialized)
            {
                SerializedProperty customCellSizeSet = property.FindPropertyRelative("CustomCellSizeSet");
                SerializedProperty customPaddingSet = property.FindPropertyRelative("CustomPaddingSet");
                SerializedProperty cellWidthProperty = property.FindPropertyRelative("CellWidth");
                SerializedProperty cellHeightProperty = property.FindPropertyRelative("CellHeight");
                SerializedProperty widthPaddingProperty = property.FindPropertyRelative("PaddingW");
                SerializedProperty heightPaddingProperty = property.FindPropertyRelative("PaddingH");

                if (customCellSizeSet.boolValue)
                {
                    CELL_WIDTH = cellWidthProperty.intValue;
                    CELL_HEIGHT = cellHeightProperty.intValue;
                }
                else
                {
                    string gridType = property.type;
                    switch (gridType)
                    {
                        case "Grid2DString":
                            CELL_WIDTH = Grid2DString.DefaultCellWidth;
                            CELL_HEIGHT = Grid2DString.DefaultCellHeight;
                            break;
                        case "Grid2DBool":
                            CELL_WIDTH = Grid2DBool.DefaultCellWidth;
                            CELL_HEIGHT = Grid2DBool.DefaultCellHeight;
                            break;
                        case "Grid2DFloat":
                            CELL_WIDTH = Grid2DFloat.DefaultCellWidth;
                            CELL_HEIGHT = Grid2DFloat.DefaultCellHeight;
                            break;
                        case "Grid2DInt":
                            CELL_WIDTH = Grid2DInt.DefaultCellWidth;
                            CELL_HEIGHT = Grid2DInt.DefaultCellHeight;
                            break;
                        case string enumGridType when enumGridType.StartsWith("Grid2DEnum"):
                            CELL_WIDTH = Grid2DEnum<System.Enum>.DefaultCellWidth;
                            CELL_HEIGHT = Grid2DEnum<System.Enum>.DefaultCellHeight;
                            break;
                        default:
                            CELL_WIDTH = 30;
                            CELL_HEIGHT = 20;
                            break;
                    }

                    defaultCellWidth = CELL_WIDTH;
                    defaultCellHeight = CELL_HEIGHT;
                }

                if (customPaddingSet.boolValue)
                {
                    PROPERTY_PADDING_W = widthPaddingProperty.intValue;
                    PROPERTY_PADDING_H = heightPaddingProperty.intValue;
                }
                else
                {
                    string gridType = property.type;
                    switch (gridType)
                    {
                        case "Grid2DString":
                            PROPERTY_PADDING_W = Grid2DString.DefaultPaddingW;
                            PROPERTY_PADDING_H = Grid2DString.DefaultPaddingH;
                            break;
                        case "Grid2DBool":
                            PROPERTY_PADDING_W = Grid2DBool.DefaultPadding_W;
                            PROPERTY_PADDING_H = Grid2DBool.DefaultPadding_H;
                            break;
                        case "Grid2DFloat":
                            PROPERTY_PADDING_W = Grid2DFloat.DefaultPadding_W;
                            PROPERTY_PADDING_H = Grid2DFloat.DefaultPadding_H;
                            break;
                        case "Grid2DInt":
                            PROPERTY_PADDING_W = Grid2DInt.DefaultPadding_W;
                            PROPERTY_PADDING_H = Grid2DInt.DefaultPadding_H;
                            break;
                        case string enumGridType when enumGridType.StartsWith("Grid2DEnum"):
                            PROPERTY_PADDING_W = Grid2DEnum<System.Enum>.DefaultPadding_W;
                            PROPERTY_PADDING_H = Grid2DEnum<System.Enum>.DefaultPadding_H;
                            break;
                        default:
                            PROPERTY_PADDING_W = 5;
                            PROPERTY_PADDING_H = 5;
                            break;
                    }

                    defaultPaddingWidth = PROPERTY_PADDING_W;
                    defaultPaddingHeight = PROPERTY_PADDING_H;
                }
                isInitialized = true;
            }
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            thisProperty = property;

            gridProperty ??= property.FindPropertyRelative("Grid");
            widthCountProperty ??= property.FindPropertyRelative("WidthCount");
            heightCountProperty ??= property.FindPropertyRelative("HeightCount");

            // Sets visual properties based on the grid type
            SetDefaultVisualProperties(thisProperty);

            #region Foldout & DotsMenu

            EditorGUILayout.BeginHorizontal();
            mainFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(mainFoldout, label.text);

            if (GUILayout.Button(EditorGUIUtility.IconContent("_Menu"), EditorStyles.iconButton, GUILayout.Width(20)))
            {
                GUI.FocusControl(null);
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("Reset Values"), false, ResetGrid);

                // We don't allow the change of Bool Cell Sizes since they cannot be changed.
                if (thisProperty.type == "Grid2DBool")
                {
                    menu.AddDisabledItem(new GUIContent("Change Cell Size"));
                }
                else
                {
                    menu.AddItem(new GUIContent("Change Cell Size"), false, () =>
                    {
                        CellSizePopupWindow.ShowWindow(this, CELL_WIDTH, CELL_HEIGHT, defaultCellWidth, defaultCellHeight);
                    });
                }

                menu.AddItem(new GUIContent("Change Padding"), false, () =>
                {
                    PaddingPopupWindow.ShowWindow(this, PROPERTY_PADDING_W, PROPERTY_PADDING_H, defaultPaddingWidth, defaultPaddingHeight);
                });

                menu.ShowAsContext();
            }
            EditorGUILayout.EndHorizontal();

            #endregion

            // Logic when foldout menu is opened
            if (mainFoldout)
            {
                EditorGUILayout.Space(INITIAL_SPACING);

                bool gridChanged = false;

                #region Grid Draw

                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Width(EditorGUIUtility.currentViewWidth));

                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(20);
                EditorGUILayout.BeginVertical();

                for (int i = 0; i < heightCountProperty.intValue; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    SerializedProperty items = gridProperty.GetArrayElementAtIndex(i).FindPropertyRelative("items");

                    for (int j = 0; j < widthCountProperty.intValue; j++)
                    {
                        GUILayout.Space(PROPERTY_PADDING_W);
                        EditorGUI.BeginChangeCheck();

                        // Handling cell drawing based on the grid type
                        SerializedProperty cell = items.GetArrayElementAtIndex(j);
                        switch (cell.propertyType)
                        {
                            case SerializedPropertyType.String:
                                cell.stringValue = EditorGUILayout.TextField(cell.stringValue, GUILayout.Width(CELL_WIDTH), GUILayout.Height(CELL_HEIGHT));
                                break;
                            case SerializedPropertyType.Boolean:
                                cell.boolValue = EditorGUILayout.Toggle(cell.boolValue, GUILayout.Width(CELL_WIDTH), GUILayout.Height(CELL_HEIGHT));
                                break;
                            case SerializedPropertyType.Float:
                                cell.floatValue = EditorGUILayout.FloatField(cell.floatValue, GUILayout.Width(CELL_WIDTH), GUILayout.Height(CELL_HEIGHT));
                                break;
                            case SerializedPropertyType.Integer:
                                cell.intValue = EditorGUILayout.IntField(cell.intValue, GUILayout.Width(CELL_WIDTH), GUILayout.Height(CELL_HEIGHT));
                                break;
                            case SerializedPropertyType.Enum:
                                var enumType = fieldInfo.FieldType.GetGenericArguments()[0];
                                cell.enumValueIndex = EditorGUILayout.Popup(cell.enumValueIndex, Enum.GetNames(enumType), GUILayout.Width(CELL_WIDTH), GUILayout.Height(CELL_HEIGHT));
                                break;
                        }

                        if (EditorGUI.EndChangeCheck())
                        {
                            gridChanged = true;
                        }
                    }

                    EditorGUILayout.EndHorizontal();
                    GUILayout.Space(PROPERTY_PADDING_H);
                }

                EditorGUILayout.EndVertical();
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.EndScrollView();

                #endregion

                if (gridChanged)
                {
                    property.serializedObject.ApplyModifiedProperties();
                }

                #region UpdateGridButton

                GUILayout.Space(5);
                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();

                if (GUILayout.Button("Update Grid Size", GUILayout.Height(BUTTON_HEIGHT), GUILayout.MaxWidth(500)))
                {
                    GridSizePopupWindow.ShowWindow(this, widthCountProperty.intValue, heightCountProperty.intValue);
                }

                GUILayout.Space(10);
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();

                #endregion

                #region Copy & Paste Buttons

                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();

                if (GUILayout.Button("Copy", GUILayout.Height(BUTTON_HEIGHT), GUILayout.MaxWidth(248)))
                {
                    GUI.FocusControl(null);

                    System.Type gridType = fieldInfo.FieldType;

                    copiedGridData = new CopiedGridData(GetGridValues(), gridType, widthCountProperty.intValue, heightCountProperty.intValue);
                }

                bool canPaste = copiedGridData != null && copiedGridData.gridType == fieldInfo.FieldType;
                EditorGUI.BeginDisabledGroup(!canPaste);

                if (GUILayout.Button("Paste", GUILayout.Height(BUTTON_HEIGHT), GUILayout.MaxWidth(248)))
                {
                    if (canPaste)
                    {
                        GUI.FocusControl(null);
                        // Resize the grid if necessary
                        if (copiedGridData.width != widthCountProperty.intValue || copiedGridData.height != heightCountProperty.intValue)
                        {
                            InitNewGrid(copiedGridData.width, copiedGridData.height);
                        }

                        // Paste the data
                        PasteGridData(copiedGridData);
                    }
                }

                EditorGUI.EndDisabledGroup();

                GUILayout.Space(10);
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();

                #endregion

                #region End Horizontal Line

                GUILayout.Space(5);
                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();

                GUILayout.Box(GUIContent.none, GUILayout.Height(3), GUILayout.MaxWidth(500), GUILayout.ExpandWidth(true));

                GUILayout.Space(10);
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();

                #endregion
            }

            EditorGUILayout.EndFoldoutHeaderGroup();
            GUILayout.Space(10);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            heightCountProperty ??= property.FindPropertyRelative("HeightCount");

            if (mainFoldout)
            {
                return INITIAL_SPACING;
            }

            return INITIAL_SPACING;
        }


        /// <summary>
        /// Resets all grid values to their default based on the grid type.
        /// </summary>
        private void ResetGrid()
        {
            for (int i = 0; i < heightCountProperty.intValue; i++)
            {
                SerializedProperty items = gridProperty.GetArrayElementAtIndex(i).FindPropertyRelative("items");
                for (int j = 0; j < widthCountProperty.intValue; j++)
                {
                    SerializedProperty cell = items.GetArrayElementAtIndex(j);

                    switch (cell.propertyType)
                    {
                        case SerializedPropertyType.String:
                            cell.stringValue = string.Empty;
                            break;
                        case SerializedPropertyType.Boolean:
                            cell.boolValue = false;
                            break;
                        case SerializedPropertyType.Float:
                            cell.floatValue = 0f;
                            break;
                        case SerializedPropertyType.Integer:
                            cell.intValue = 0;
                            break;
                        case SerializedPropertyType.Enum:
                            cell.enumValueIndex = 0;
                            break;
                        default:
                            Debug.LogWarning("Unsupported property type in ResetGrid.");
                            break;
                    }
                }
            }

            thisProperty.serializedObject.ApplyModifiedProperties();
        }


        /// <summary>
        /// Sets the cell size.
        /// </summary>
        public void SetCellSize(int width, int height)
        {
            CELL_WIDTH = width;
            CELL_HEIGHT = height;

            SerializedProperty customCellSizeSet = thisProperty.FindPropertyRelative("CustomCellSizeSet");
            SerializedProperty cellWidthProperty = thisProperty.FindPropertyRelative("CellWidth");
            SerializedProperty cellHeightProperty = thisProperty.FindPropertyRelative("CellHeight");

            customCellSizeSet.boolValue = true;
            cellWidthProperty.intValue = width;
            cellHeightProperty.intValue = height;

            thisProperty.serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// Sets the padding.
        /// </summary>
        public void SetPadding(int widthPadding, int heightPadding)
        {
            PROPERTY_PADDING_W = widthPadding;
            PROPERTY_PADDING_H = heightPadding;

            SerializedProperty customPaddingSet = thisProperty.FindPropertyRelative("CustomPaddingSet");
            SerializedProperty widthPaddingProperty = thisProperty.FindPropertyRelative("PaddingW");
            SerializedProperty heightPaddingProperty = thisProperty.FindPropertyRelative("PaddingH");

            customPaddingSet.boolValue = true;
            widthPaddingProperty.intValue = widthPadding;
            heightPaddingProperty.intValue = heightPadding;

            thisProperty.serializedObject.ApplyModifiedProperties();
        }


        /// <summary>
        /// Retrieves the current grid values as a 2D array of objects.
        /// </summary>
        private object[][] GetGridValues()
        {
            object[][] arr = new object[heightCountProperty.intValue][];

            for (int i = 0; i < heightCountProperty.intValue; i++)
            {
                arr[i] = new object[widthCountProperty.intValue];

                for (int j = 0; j < widthCountProperty.intValue; j++)
                {
                    SerializedProperty cell = gridProperty.GetArrayElementAtIndex(i).FindPropertyRelative("items").GetArrayElementAtIndex(j);

                    switch (cell.propertyType)
                    {
                        case SerializedPropertyType.String:
                            arr[i][j] = cell.stringValue;
                            break;
                        case SerializedPropertyType.Boolean:
                            arr[i][j] = cell.boolValue;
                            break;
                        case SerializedPropertyType.Float:
                            arr[i][j] = cell.floatValue;
                            break;
                        case SerializedPropertyType.Integer:
                            arr[i][j] = cell.intValue;
                            break;
                        case SerializedPropertyType.Enum:
                            arr[i][j] = cell.enumDisplayNames[cell.enumValueIndex];
                            break;
                        default:
                            Debug.LogWarning("Unsupported property type in GetGridValues.");
                            arr[i][j] = null;
                            break;
                    }
                }
            }

            return arr;
        }


        /// <summary>
        /// Resizes the grid while preserving existing values.
        /// </summary>
        public void ResizeGridAndPreserveValues(int newWidth, int newHeight)
        {
            object[][] oldGrid = GetGridValues();
            int oldWidthCount = widthCountProperty.intValue;
            int oldHeightCount = heightCountProperty.intValue;

            InitNewGrid(newWidth, newHeight);

            for (int i = 0; i < newHeight; i++)
            {
                SerializedProperty row = GetRowAt(i);

                for (int j = 0; j < newWidth; j++)
                {
                    SerializedProperty cell = row.GetArrayElementAtIndex(j);

                    if (j < oldWidthCount && i < oldHeightCount)
                    {
                        SetValue(cell, oldGrid[i][j]);
                    }
                }
            }

            thisProperty.serializedObject.ApplyModifiedProperties();
        }


        /// <summary>
        /// Initializes a new grid with default values.
        /// </summary>
        private void InitNewGrid(int newWidthCount, int newHeightCount)
        {
            gridProperty.ClearArray();

            for (int i = 0; i < newHeightCount; i++)
            {
                gridProperty.InsertArrayElementAtIndex(i);
                SerializedProperty row = GetRowAt(i);
                row.ClearArray();

                for (int j = 0; j < newWidthCount; j++)
                {
                    row.InsertArrayElementAtIndex(j);
                    SerializedProperty cell = row.GetArrayElementAtIndex(j);
                    SetValue(cell, string.Empty);
                }
            }

            widthCountProperty.intValue = newWidthCount;
            heightCountProperty.intValue = newHeightCount;
        }


        /// <summary>
        /// Returns a row property at the specified index.
        /// </summary>
        private SerializedProperty GetRowAt(int index)
        {
            return gridProperty.GetArrayElementAtIndex(index).FindPropertyRelative("items");
        }


        /// <summary>
        /// Sets a cell value based on its type.
        /// </summary>
        private void SetValue(SerializedProperty cell, object obj)
        {
            switch (cell.propertyType)
            {
                case SerializedPropertyType.String:
                    cell.stringValue = obj as string ?? string.Empty;
                    break;
                case SerializedPropertyType.Boolean:
                    if (obj is bool boolValue)
                    {
                        cell.boolValue = boolValue;
                    }
                    break;
                case SerializedPropertyType.Float:
                    if (obj is float floatValue)
                    {
                        cell.floatValue = floatValue;
                    }
                    break;
                case SerializedPropertyType.Integer:
                    if (obj is int intValue)
                    {
                        cell.intValue = intValue;
                    }
                    break;
                case SerializedPropertyType.Enum:
                    if (obj is Enum enumValue)
                    {
                        // Convert the enum to its corresponding int value and assign it
                        cell.enumValueIndex = Convert.ToInt32(enumValue);
                    }
                    break;
                default:
                    Debug.LogWarning("Unsupported property type in SetValue.");
                    break;
            }
        }


        /// <summary>
        /// Paste copied data into this grid.
        /// </summary>
        private void PasteGridData(CopiedGridData copiedData)
        {
            for (int i = 0; i < copiedData.height; i++)
            {
                SerializedProperty items = gridProperty.GetArrayElementAtIndex(i).FindPropertyRelative("items");

                for (int j = 0; j < copiedData.width; j++)
                {
                    SerializedProperty cell = items.GetArrayElementAtIndex(j);

                    switch (cell.propertyType)
                    {
                        case SerializedPropertyType.String:
                            cell.stringValue = (string)copiedData.data[i][j];
                            break;
                        case SerializedPropertyType.Boolean:
                            cell.boolValue = (bool)copiedData.data[i][j];
                            break;
                        case SerializedPropertyType.Float:
                            cell.floatValue = (float)copiedData.data[i][j];
                            break;
                        case SerializedPropertyType.Integer:
                            cell.intValue = (int)copiedData.data[i][j];
                            break;
                        case SerializedPropertyType.Enum:
                            string enumStringValue = (string)copiedData.data[i][j];
                            int enumIndex = Array.IndexOf(cell.enumDisplayNames, enumStringValue);
                            if (enumIndex >= 0)
                            {
                                cell.enumValueIndex = enumIndex;
                            }
                            else
                            {
                                Debug.LogWarning($"Enum value '{enumStringValue}' not found in {cell.enumDisplayNames}.");
                            }
                            break;
                        default:
                            Debug.LogWarning("Unsupported property type in PasteGridData.");
                            break;
                    }
                }
            }

            thisProperty.serializedObject.ApplyModifiedProperties();
        }

    }
}