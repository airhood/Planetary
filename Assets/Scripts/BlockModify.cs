using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BlockModify : MonoBehaviour
{
    [Header("World")]
    public Main main;
    public Tilemap collidableBlock;
    public Tilemap nonCollidableBlock;

    [Header("BlockList")]
    public List<Block> blocks = new List<Block>();

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public bool CheckSetBlockAvailable(Vector2Int position, short blockCode)
    {
        Block block = blocks[blockCode];

        BuildingState buildingState = block.building[block.defaultStateCode];

        foreach (BuildingTile buildingTile in buildingState.buildingParts)
        {
            Vector2Int pos = position + buildingTile.pos;
            if (main.world.planet[0].map.map[pos.x, pos.y] != 0)
            {
                return false;
            }
        }

        return true;
    }

    public void SetBlock(Vector2Int position, short blockCode)
    {
        Block block = blocks[blockCode];

        BuildingState buildingState = block.building[block.defaultStateCode];

        main.world.planet[0].map.map[position.x, position.y] = -1;

        // code -1 : a block is placed but it is not the main part of the
        // block and any matter or block cannot be placed at that position

        foreach (BuildingTile buildingTile in buildingState.buildingParts)
        {
            Vector2Int pos = position + (buildingTile.pos - block.BuildPoint);

            if (pos == position) {
                main.world.planet[0].map.map[pos.x, pos.y] = (short)((10 * block.id) + block.defaultStateCode);
            }
            else
            {
                main.world.planet[0].map.map[pos.x, pos.y] = -30000;
            }
            collidableBlock.SetTile((Vector3Int)(position + pos), buildingTile.tile);
        }
    }

    public void ModifyTerrain(Vector2Int position, short matterCode)
    {
        if (matterCode == 0)
        {
            main.world.planet[0].map.map[position.x, position.y] = 0;
            collidableBlock.SetTile((Vector3Int)position, null);
        }
        else if (Main.matterList[matterCode].matterType == MatterType.Solid)
        {
            main.world.planet[0].map.map[position.x, position.y] = matterCode;
            collidableBlock.SetTile((Vector3Int)position, Main.matterList[matterCode].tile);
        }
    }

    public short GetTerrainTileHardness(Vector2Int position)
    {
        return Main.matterList[main.world.planet[0].map.map[position.x, position.y] * (-1)].hardness;
    }
}
