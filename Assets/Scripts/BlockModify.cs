using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BlockModify : MonoBehaviour
{
    public Main main;
    public Tilemap collidableBlock;
    public Tilemap nonCollidableBlock;

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
        Block block = Main.blockList[blockCode];

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

    public bool SetBlock(Vector2Int position, short blockCode)
    {
        Block block = Main.blockList[blockCode];

        if (block.type == BlockType.Tile)
        {
            collidableBlock.SetTile((Vector3Int)position, block.tile);
            main.world.planet[0].map.map[position.x, position.y] = blockCode;
            return true;
        }

        BuildingState buildingState = block.building[block.defaultStateCode];

        // code -1 : a block is placed but it is not the main part of the
        // block and any matter or block cannot be placed at that position

        foreach (Vector2Int pos in block.requiredFloor)
        {
            Vector2Int floorPos = position + (pos - block.BuildPoint);
            if (main.world.planet[0].map.map[floorPos.x, floorPos.y] == 0 || main.world.planet[0].map.map[floorPos.x, floorPos.y] <= -20000)
            {
                return false;
            }
        }

        foreach (BuildingTile buildingTile in buildingState.buildingParts)
        {
            Vector2Int pos = position + (buildingTile.pos - block.BuildPoint);
            if (main.world.planet[0].map.map[pos.x, pos.y] != 0)
            {
                return false;
            }
        }

        foreach (BuildingTile buildingTile in buildingState.buildingParts)
        {
            Vector2Int relativePos = buildingTile.pos - block.BuildPoint;
            Vector2Int implicitPos = position + relativePos;

            if (buildingTile.pos == block.BuildPoint) {
                main.world.planet[0].map.map[implicitPos.x, implicitPos.y] = (short)((10 * block.id) + block.defaultStateCode);
            }
            else
            {
                main.world.planet[0].map.map[implicitPos.x, implicitPos.y] = (short)(-20000 - (( (relativePos.x > 0) ? 0 : 1)* 1000) - (Mathf.Abs(relativePos.x) * 100) - (( (relativePos.y > 0) ? 0 : 1 ) * 10) -(relativePos.y));
            }
            
            if (buildingTile.isCollidable)
            {
                collidableBlock.SetTile((Vector3Int)implicitPos, buildingTile.tile);
            }
            else
            {
                nonCollidableBlock.SetTile((Vector3Int)implicitPos, buildingTile.tile);
            }
        }

        return true;
    }

    public void DeleteBlock(Vector2Int position)
    {
        Block block = Main.blockList[main.world.planet[0].map.map[position.x, position.y]];
        
        if (block.type == BlockType.Tile)
        {
            collidableBlock.SetTile((Vector3Int)position, null);
            main.world.planet[0].map.map[position.x, position.y] = 0;
            return;
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
        if (main.world.planet[0].map.map[position.x, position.y] >= 0)
        {
            Debug.LogError("BlockModify.GetTerrainTileHardness: this position is not a matter");
            return -1;
        }
        return Main.matterList[main.world.planet[0].map.map[position.x, position.y] * (-1)].hardness;
    }

    public short GetBlockHardness(Vector2Int position)
    {
        if (main.world.planet[0].map.map[position.x, position.y] <= 0)
        {
            Debug.LogError("BlockModify.GetBlockHardness: this position is not a block");
            return -1;
        }
        return Main.blockList[main.world.planet[0].map.map[position.x, position.y]].destructionTime;
    }


}
