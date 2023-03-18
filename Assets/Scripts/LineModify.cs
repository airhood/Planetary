using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum LineModifyMode
{
    None, LiquidPipe, GasPipe, ConveyerBelt
}

public class LineModify : MonoBehaviour
{
    [SerializeField] Tilemap liquidPipeTilemap;
    [SerializeField] Tilemap gasPipeTilemap;
    [SerializeField] Tilemap conveyerBeltTilemap;

    [Header("Electricity Wire")]
    public Tile E_defaultTile;
    public Tile E_horizontalTile;
    public Tile E_verticalTile;
    public Tile E_URTile;
    public Tile E_RDTile;
    public Tile E_DLTile;
    public Tile E_LUTile;
    public Tile E_URDTile;
    public Tile E_RDLTile;
    public Tile E_DLUTile;
    public Tile E_LURTile;
    public Tile E_URDLTile;

    [Header("Liquid Pipe")]
    public Tile W_defaultTile;
    public Tile W_UTile;
    public Tile W_RTile;
    public Tile W_DTile;
    public Tile W_LTile;
    public Tile W_URTile;
    public Tile W_RDTile;
    public Tile W_DLTile;
    public Tile W_LUTile;
    public Tile W_URDTile;
    public Tile W_RDLTile;
    public Tile W_DLUTile;
    public Tile W_LURTile;
    public Tile W_URDLTile;

    [Header("Gas Pipe")]
    public Tile G_defaultTile;
    public Tile G_UTile;
    public Tile G_RTile;
    public Tile G_DTile;
    public Tile G_LTile;
    public Tile G_URTile;
    public Tile G_RDTile;
    public Tile G_DLTile;
    public Tile G_LUTile;
    public Tile G_URDTile;
    public Tile G_RDLTile;
    public Tile G_DLUTile;
    public Tile G_LURTile;
    public Tile G_URDLTile;
    
    [Header("Conveyer Belt")]
    public Tile C_defaultTile;
    public Tile C_horizontalTile;
    public Tile C_verticalTile;
    public Tile C_URTile;
    public Tile C_RDTile;
    public Tile C_DLTile;
    public Tile C_LUTile;
    public Tile C_URDTile;
    public Tile C_RDLTile;
    public Tile C_DLUTile;
    public Tile C_LURTile;
    public Tile C_URDLTile;

    public Vector2Int lastPos;

    public LineModifyMode lineModifyMode;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float x = Input.GetAxis("Mouse X");
        float y = Input.GetAxis("Mouse Y");

        Vector2 worldPos = (Vector2)(Camera.main.ScreenToWorldPoint(new Vector2(x, y)));

        Vector2Int tilemapPos;

        if (lineModifyMode == LineModifyMode.LiquidPipe)
        {
            tilemapPos = (Vector2Int)liquidPipeTilemap.WorldToCell(worldPos);
        }
        else if (lineModifyMode == LineModifyMode.GasPipe)
        {
            tilemapPos = (Vector2Int)gasPipeTilemap.WorldToCell(worldPos);
        }
        else if (lineModifyMode == LineModifyMode.ConveyerBelt)
        {
            tilemapPos = (Vector2Int)conveyerBeltTilemap.WorldToCell(worldPos);
        }
        else return;

        if (lastPos != null)
        {
            Vector2Int deltaPos = tilemapPos - lastPos;
            if (deltaPos == Vector2.up)
            {

            }
            else if (deltaPos == Vector2.right)
            {

            }
            else if (deltaPos == Vector2.down)
            {

            }
            else if (deltaPos == Vector2.left)
            {

            }
            else
            {

            }
        }
    }
}
