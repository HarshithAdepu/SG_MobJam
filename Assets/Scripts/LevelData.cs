using UnityEngine;
using SmartGridsToolkit;

[CreateAssetMenu(fileName = "LevelData", menuName = "ScriptableObjects/LevelData", order = 1)]
public class LevelData : ScriptableObject
{
    public string levelName;
    public Tower towerPrefab;
    public NPC npcPrefab;
    public Guard guardPrefab;
    public BlockController unitBlockPrefab;
    public bool npcDieOnKill = false;
    public int npcsPerBlock;
    public GameObject wallPrefab;
    public GameObject exitPrefab;
    public Vector2Int exitModifierRange;
    public Grid2DString gridLayout;
    public float timeLimit;
    public int towerHealth;
    public Vector2Int guardGrid;

    public int gridWidth => gridLayout.WidthCount;
    public int gridHeight => gridLayout.HeightCount;
}