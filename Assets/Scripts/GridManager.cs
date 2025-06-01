using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using SmartGridsToolkit;

public class GridManager : MonoBehaviour
{
    [ SerializeField ] private List<ColorData> gridColors;
    
    private Grid2DString gridState;
    private int gridWidth, gridHeight;
    private Dictionary<string, GameObject> blockGroups = new Dictionary<string, GameObject>();
    private Dictionary<string, GameObject> exitGroups = new Dictionary<string, GameObject>();
    private Dictionary<string, int> exitModifiers = new Dictionary<string, int>();
    private float cellSize = 1f;

    private LevelData currentLevelData;
    public GameManager gameManager;
    
    public void InitializeGrid(GameManager manager)
    {
        gameManager = manager;
        
        currentLevelData = gameManager.CurrentLevelData;
        
        gridWidth = currentLevelData.gridWidth;
        gridHeight = currentLevelData.gridHeight;
        
        gridState = new Grid2DString(gridWidth, gridHeight);
        for ( int x = 0; x < gridWidth; x++ )
        {
            for ( int y = 0; y < gridHeight; y++ )
            {
                gridState.SetCellValue(x, y, currentLevelData.gridLayout.GetCellValue( x, y ) );
            }
        }

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                string cell = gridState.GetCellValue(x, y);
                if (string.IsNullOrEmpty(cell)) continue;

                Vector3 position = new Vector3(x * cellSize, cellSize * 0.5f, y * cellSize);

                if (cell == "-")
                {
                    GameObject wall = Instantiate( currentLevelData.wallPrefab, position, Quaternion.identity );
                    wall.name = $"Wall_{x}_{y}";
                }
                else if (cell.Length >= 3)
                {
                    char colorChar = cell[0];
                    char typeChar = cell[1];
                    string id = cell.Substring(2);

                    if (!int.TryParse(id, out int idNum))
                    {
                        Debug.LogError($"Invalid cell ID: {id}");
                        continue;
                    }

                    Color color = GetColorFromChar(colorChar);
                    string type = GetTypeFromChar(typeChar);

                    if (type == null) continue;

                    string fullId = cell;

                    if (type == "exit")
                    {
                        if (!exitGroups.ContainsKey(fullId))
                        {
                            GameObject exitParent = new GameObject(fullId);
                            exitGroups[fullId] = exitParent;
                        }
                        
                        GameObject exitMarker = Instantiate( currentLevelData.exitPrefab, position + Vector3.down * cellSize * 0.5f, Quaternion.identity );
                        exitMarker.transform.parent = exitGroups[fullId].transform;
                        color.a = 0.75f;
                        exitMarker.GetComponent<Renderer>().material.color = color;
                        exitMarker.name = fullId;
                    }
                    else if (type == "block")
                    {
                        if (!blockGroups.ContainsKey(fullId))
                        {
                            GameObject blockParent = new GameObject(fullId);
                            blockGroups[fullId] = blockParent;
                        }

                        BlockController cube = Instantiate( currentLevelData.unitBlockPrefab, position , Quaternion.identity );
                        cube.transform.parent = blockGroups[fullId].transform;
                        cube.name = $"Cube_{x}_{y}";
                        cube.GetComponent<Renderer>().material.color = color;
                        cube.SetGridManager(this);
                    }
                }
            }
        }
        
        Camera.main.transform.position = new Vector3((gridWidth - 1) * cellSize * 0.5f, Camera.main.transform.position.y, Camera.main.transform.position.z);
        
        RefreshExitModifiers();
    }

    private void RefreshExitModifiers()
    {
        foreach ( KeyValuePair<string, GameObject> exit in exitGroups )
        {
            exitModifiers[ exit.Key ] = Random.Range(currentLevelData.exitModifierRange.x, currentLevelData.exitModifierRange.y);
        }
        
        gameManager.gameUIManager.UpdateExitValues(exitGroups, exitModifiers);
    }
    
    private Color GetColorFromChar(char c)
    {
        ColorData foundColor = gridColors.FirstOrDefault( x => x.colorCode == c );
        
        if (foundColor.colorCode == c)
        {
            return foundColor.color;
        }
        else
        {
            Debug.LogWarning($"{c} is not a valid char.");
            return Color.white;
        }
    }
    
    private string GetTypeFromChar(char c)
    {
        switch (c)
        {
            case 'b': return "block";
            case 'e': return "exit";
            default: Debug.LogError( $"Invalid Type char {c}" ); return null;
        }
    }

    public bool TryMoveBlock(GameObject blockParent, Vector2Int direction)
    {
        string blockName = blockParent.name;
        List<Vector2Int> blockPositions = new List<Vector2Int>();

        foreach (Transform t in blockParent.transform)
        {
            Vector3 pos = t.position;
            int x = Mathf.RoundToInt(pos.x / cellSize);
            int y = Mathf.RoundToInt(pos.z / cellSize);
            blockPositions.Add(new Vector2Int(x, y));
        }

        bool canMove = true;
        
        foreach (Vector2Int pos in blockPositions)
        {
            int newX = pos.x + direction.x;
            int newY = pos.y + direction.y;

            string newPosState = gridState.GetCellValue(newX, newY);

            if ( newPosState == "" )
            {
                continue;
            }
            
            if(newPosState[0] != blockName[0] )
            {
                return false;
            }

            if ( newPosState[ 1 ] == 'b' )
            {
                if ( int.TryParse( blockName.Substring( 2 ), out int blockID ) && int.TryParse( newPosState.Substring( 2 ), out int newBlockID ) )
                {
                    if ( blockID != newBlockID ) return false;
                }
                else
                {
                    Debug.LogError( $"ID parse failed! blockName: {blockName}, newBlockName: {newPosState} " );
                    return false;
                }
            }
            else if ( newPosState[ 1 ] == 'e' )
            {
                return TryExitBlock( blockParent, direction, newPosState );
            }
            else 
            {
                Debug.LogError( $"Invalid Block Name {newPosState}" );
                return false;
            }
        }

        if ( canMove )
        {
            MoveBlock( blockParent, direction );
        }

        return canMove;
    }

    private bool TryExitBlock( GameObject blockParent, Vector2Int direction, string exitName )
    {
        Vector2Int exitBoundsX = new Vector2Int( int.MaxValue, int.MinValue );
        Vector2Int exitBoundsY = new Vector2Int( int.MaxValue, int.MinValue );
        Vector2Int blockBoundsX = new Vector2Int( int.MaxValue, int.MinValue );
        Vector2Int blockBoundsY = new Vector2Int( int.MaxValue, int.MinValue );

        GameObject exitParent = exitGroups[exitName];
        List<Vector2Int> exitPositions = new List<Vector2Int>();
        
        foreach ( Transform t in exitParent.transform )
        {
            Vector3 pos = t.position;
            int x = Mathf.RoundToInt(pos.x / cellSize);
            int y = Mathf.RoundToInt(pos.z / cellSize);
            
            if(x < exitBoundsX.x) exitBoundsX.x = x;
            if(x > exitBoundsX.y) exitBoundsX.y = x;
            
            if(y < exitBoundsY.x) exitBoundsY.x = y;
            if(y > exitBoundsY.y) exitBoundsY.y = y;
            
            exitPositions.Add(new Vector2Int(x, y));
        }
        
        foreach (Transform t in blockParent.transform)
        {
            Vector3 pos = t.position;
            
            int x = Mathf.RoundToInt(pos.x / cellSize);
            int y = Mathf.RoundToInt(pos.z / cellSize);
            
            if(x < blockBoundsX.x) blockBoundsX.x = x;
            if(x > blockBoundsX.y) blockBoundsX.y = x;
            
            if(y < blockBoundsY.x) blockBoundsY.x = y;
            if(y > blockBoundsY.y) blockBoundsY.y = y;
        }
        
        if ( direction == Vector2Int.up || direction == Vector2Int.down )
        {
            if ( exitBoundsX.x <= blockBoundsX.x && exitBoundsX.y >= blockBoundsX.y )
            {
                ExitBlock( blockParent, direction, exitParent );
                return true;
            }

            return false;
        }
        else if ( direction == Vector2Int.left || direction == Vector2Int.right )
        {
            if ( exitBoundsY.x <= blockBoundsY.x && exitBoundsY.y >= blockBoundsY.y )
            {
                ExitBlock( blockParent, direction, exitParent );
                return true;
            }

            return false;
        }
        else
        {
            Debug.LogError( $"Invalid Direction {direction}" );
        }

        return false;
    }
    
    public void MoveBlock(GameObject blockParent, Vector2Int direction)
    {
        List<Vector2Int> oldPositions = new List<Vector2Int>();
        foreach (Transform cube in blockParent.transform)
        {
            Vector3 pos = cube.position;
            int x = Mathf.RoundToInt(pos.x / cellSize);
            int y = Mathf.RoundToInt(pos.z / cellSize);
            oldPositions.Add(new Vector2Int(x, y));
            gridState.SetCellValue( x, y, "" );
        }

        blockParent.transform.position += new Vector3(direction.x * cellSize, 0, direction.y * cellSize);

        foreach (Transform cube in blockParent.transform)
        {
            Vector3 pos = cube.position;
            int x = Mathf.RoundToInt(pos.x / cellSize);
            int y = Mathf.RoundToInt(pos.z / cellSize);
            gridState.SetCellValue(x, y, blockParent.name);
        }
    }

    public void ExitBlock(GameObject blockParent, Vector2Int direction, GameObject exitParent)
    {
        if ( !blockGroups.ContainsKey( blockParent.name ) ) return;
        
        blockGroups.Remove( blockParent.name );
        foreach (Transform cube in blockParent.transform)
        {
            Vector3 pos = cube.position;
            int x = Mathf.RoundToInt(pos.x / cellSize);
            int y = Mathf.RoundToInt(pos.z / cellSize);
            gridState.SetCellValue(x, y, "");
        }
        int exitModifier = exitModifiers[exitParent.name];
        GameManager.Instance.npcManager.SpawnNPCs( ( blockParent.transform.childCount * currentLevelData.npcsPerBlock ) + exitModifier, exitParent.transform.GetChild(0).position + new Vector3( direction.x, 0, direction.y ) * cellSize );
        RefreshExitModifiers();
        PlayExitTween(blockParent);
    }

    private void PlayExitTween(GameObject blockParent)
    {
        Sequence exitSequence = DOTween.Sequence();

        for ( int i = 0; i < blockParent.transform.childCount ; i++ )
        {
            exitSequence.Join( blockParent.transform.GetChild( i ).DOScale( 0f, 0.15f ).SetEase( Ease.InBack, overshoot:1f ).SetDelay( i * 0.05f ) );
        }
        
        exitSequence.Play();
        
        exitSequence.OnComplete( () => Destroy(blockParent) );
    }
}