using System;
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
                main.world.planet[0].map.map[implicitPos.x, implicitPos.y] = block.id;
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

    public (short, Vector2Int?) DeleteBlock(Vector2Int position)
    {
        short blockCode = main.world.planet[0].map.map[position.x, position.y];
        print($"blockCode: {blockCode}");
        Block block;
        sbyte type = -1;
        if (blockCode > 0)
        {
            block = Main.blockList[blockCode];

            if (block.type == BlockType.Tile) type = 0;
            else if (block.type == BlockType.Building) type = 1;
        }

        if (type == 0)
        {
            block = Main.blockList[blockCode];

            if (block.type == BlockType.Tile)
            {
                collidableBlock.SetTile((Vector3Int)position, null);
                main.world.planet[0].map.map[position.x, position.y] = 0;
                return (blockCode, position);
            }
        }
        else if (type == 1 || blockCode <= -20000)
        {
            print("hi");
            Vector2Int ? _blockMainPos = null;
            if (blockCode > 0)
            {
                _blockMainPos = position;
            }
            else
            {
                _blockMainPos = BlockRelativePosToBlockMainPos(position);
            }
            Vector2Int blockMainPos;
            if (_blockMainPos == null) return (0, null);
            blockMainPos = (Vector2Int)_blockMainPos;
            print($"playerMouseOnTilePos: {position}, blockMainPos: {blockMainPos}");
            short mainBlockCode = main.world.planet[0].map.map[blockMainPos.x, blockMainPos.y];
            print(deleteBlockParts(blockMainPos) ? "Delete block success" : "Delete block error");

            print($"mainBlockCode: {mainBlockCode}");
            (short, Vector2Int?) returnValue = (mainBlockCode, blockMainPos);
            return returnValue;
        }

        return (0, null);
    }

    public Vector2Int? BlockRelativePosToBlockMainPos(Vector2Int position)
    {
        string _blockCode = main.world.planet[0].map.map[position.x, position.y].ToString();
        print($"main.world.planet[0].map.map[position.x, position.y].ToString(); : {main.world.planet[0].map.map[position.x, position.y].ToString()}");
        char[] blockCodeDevided = _blockCode.ToCharArray();

        bool xSign;
        int xRelativePos;
        bool ySign;
        int yRelativePos;

        try
        {
            xSign = (blockCodeDevided[1] == 0 ? true : false);
            xRelativePos = int.Parse(blockCodeDevided[2].ToString());
            ySign = (blockCodeDevided[3] == 0 ? true : false);
            yRelativePos = int.Parse(blockCodeDevided[4].ToString());
        }
        catch (Exception ex)
        {
            print($"Cannot convert position ({position.x},{position.y}) block code to relative blockCode. Error Code: {ex}");
            return null;
        }

        int xFinalRelativePos = xSign ? xRelativePos : xRelativePos * (-1);
        int yFinalRelativePos = ySign ? yRelativePos : yRelativePos * (-1);

        Vector2Int finalRelativePos = new Vector2Int(xFinalRelativePos, yFinalRelativePos);
        Vector2Int blockMainPos = position - finalRelativePos;
        return blockMainPos;
    }

    private bool deleteBlockParts(Vector2Int position)
    {
        print($"main.world.planet[0].map.map[position.x, position.y]: {main.world.planet[0].map.map[position.x, position.y]}");
        char[] blockCode;
        byte blockState;
        try
        {
            blockCode = main.world.planet[0].map.map[position.x, position.y].ToString().ToCharArray();
            blockState = (byte)(byte.Parse(blockCode[0].ToString()) - 1);
        }
        catch (Exception ex)
        {
            print($"Cannot extract blockState from block main position ({position.x},{position.y}). Error code: {ex}");
            return false;
        }
        Block block = Main.blockList[main.world.planet[0].map.map[position.x, position.y]];
        BuildingState buildingState = block.building[blockState];

        foreach(BuildingTile buildingTile in buildingState.buildingParts)
        {
            Vector2Int pos = position + buildingTile.pos;
            main.world.planet[0].map.map[pos.x, pos.y] = 0;

            if (buildingTile.isCollidable)
            {
                collidableBlock.SetTile((Vector3Int)pos, null);
            }
            else
            {
                nonCollidableBlock.SetTile((Vector3Int)pos, null);
            }
        }

        return true;
    }

    public short MainBlockWorldMapCodeToBlockCode(short worldMapCode)
    {
        print($"worldMapCode: {worldMapCode}");
        char[] _worldMapCode;
        char[] blockCode = new char[4];
        try
        {
            _worldMapCode = worldMapCode.ToString().ToCharArray();
            for(int i = 0; i < 4; i++)
            {
                blockCode[i] = _worldMapCode[i + 1];
            }
        }
        catch (Exception ex)
        {
            print($"Error while converting MainBlockWorldMapCode to BlockCode. Error code: {ex}");
        }
        string _blockCode = string.Empty;
        foreach (char c in blockCode)
        {
            _blockCode += c;
        }
        print($"int.Parse(_blockCode): {int.Parse(_blockCode)}");
        return (short)int.Parse(_blockCode);
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

    public void UpdateBlockTilemap(Vector2Int position)
    {

    }

    public void UpdateBlockStateTilemap(Vector2Int position, byte state)
    {

    }
}
