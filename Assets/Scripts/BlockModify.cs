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
    public Tilemap ladder;
    public BlockInstanceManager blockInstanceManager;

    public bool CheckSetBlockAvailable(Vector2Int position, short blockID, Rotation rotation)
    {
        Block block = Main.blockList[blockID];

        BuildingState buildingState = block.building[block.defaultStateCode];

        byte? rotationID = FindRotationID(buildingState, rotation);
        if (rotationID == null)
        {
            Log.LogError($"Block rotation not found. BlockID: {blockID}");
            return false;
        }

        foreach (BuildingTile buildingTile in buildingState.buildingRotations[(byte)rotationID].buildingParts)
        {
            Vector2Int pos = position + buildingTile.pos;
            if (main.world.planet[0].map.map[pos.x, pos.y] != 0)
            {
                return false;
            }
        }

        return true;
    }

    private byte? FindRotationID(BuildingState buildingState, Rotation rotation)
    {
        byte? rotationID = null;
        bool rotationFound = false;

        foreach (BuildingRotation buildingRotation in buildingState.buildingRotations)
        {
            if (buildingRotation.rotation == rotation)
            {
                rotationID = (byte)rotation;
                rotationFound = true;
            }
        }

        if (!rotationFound || rotationID == null)
        {
            return null;
        }

        return rotationID;
    }

    public bool SetBlock(Vector2Int position, short blockID, Rotation rotation)
    {
        Block block = Main.blockList[blockID];

        if (block.type == BlockType.Tile)
        {
            if (block.isLadder) ladder.SetTile((Vector3Int)position, block.tile);
            else if (block.isCollidable) collidableBlock.SetTile((Vector3Int)position, block.tile);
            else nonCollidableBlock.SetTile((Vector3Int)position, block.tile);
            main.world.planet[0].map.map[position.x, position.y] = blockID;
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

        byte? rotationID = FindRotationID(buildingState, rotation);
        if (rotationID == null)
        {
            Log.LogError($"BlockModify.SetBlock: Block rotation not found. BlockID: {blockID}");
            return false;
        }

        foreach (BuildingTile buildingTile in buildingState.buildingRotations[(byte)rotationID].buildingParts)
        {
            Vector2Int pos = position + (buildingTile.pos - block.BuildPoint);
            if (main.world.planet[0].map.map[pos.x, pos.y] != 0)
            {
                return false;
            }
        }

        foreach (BuildingTile buildingTile in buildingState.buildingRotations[(byte)rotationID].buildingParts)
        {
            Vector2Int relativePos = buildingTile.pos - block.BuildPoint;
            Vector2Int implicitPos = position + relativePos;

            if (buildingTile.pos == block.BuildPoint) {
                main.world.planet[0].map.map[implicitPos.x, implicitPos.y] = block.id;
            }
            else
            {
                main.world.planet[0].map.map[implicitPos.x, implicitPos.y] = (short)(-20000 - (( (relativePos.x >= 0) ? 0 : 1)* 1000) - (Mathf.Abs(relativePos.x) * 100) - (( (relativePos.y >= 0) ? 0 : 1 ) * 10) -(relativePos.y));
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
        blockInstanceManager.AddBlockInstance(position, blockID, rotation);

        return true;
    }

    public (short, Vector2Int?) DeleteBlock(Vector2Int position)
    {
        short blockID = main.world.planet[0].map.map[position.x, position.y];
        Block block;
        sbyte type = -1;
        if (blockID > 0)
        {
            block = Main.blockList[blockID];

            if (block.type == BlockType.Tile) type = 0;
            else if (block.type == BlockType.Building) type = 1;
        }

        if (type == 0)
        {
            block = Main.blockList[blockID];

            if (block.type == BlockType.Tile)
            {
                collidableBlock.SetTile((Vector3Int)position, null);
                if (block.isLadder) ladder.SetTile((Vector3Int)position, null);
                main.world.planet[0].map.map[position.x, position.y] = 0;
                return (blockID, position);
            }
        }
        else if (type == 1 || blockID <= -20000)
        {
            Vector2Int ? _blockMainPos = null;
            if (blockID > 0)
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
            short mainBlockCode = main.world.planet[0].map.map[blockMainPos.x, blockMainPos.y];
            bool deleteBlockPartsResult = deleteBlockParts(blockMainPos);
            if (!deleteBlockPartsResult)
            {
                Log.LogError("BlockModify.DeleteBlock: Error occured at BlockModify.deleteBlockParts(blockMainPos)");
            }
            (short, Vector2Int?) returnValue = (mainBlockCode, blockMainPos);
            blockInstanceManager.RemoveBlockInstance(blockMainPos);
            return returnValue;
        }

        return (0, null);
    }

    public Vector2Int? BlockRelativePosToBlockMainPos(Vector2Int position)
    {
        string _blockID = Math.Abs(main.world.planet[0].map.map[position.x, position.y]).ToString();
        char[] blockIDDevided = _blockID.ToCharArray();

        bool xSign;
        int xRelativePos;
        bool ySign;
        int yRelativePos;

        try
        {
            xSign = blockIDDevided[1] == '0';
            xRelativePos = int.Parse(blockIDDevided[2].ToString());
            ySign = blockIDDevided[3] == '0';
            yRelativePos = int.Parse(blockIDDevided[4].ToString());
        }
        catch (Exception ex)
        {
            Log.LogError($"Cannot convert position ({position.x},{position.y}) block code to relative blockCode. Error Code: {ex}");
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
        char[] blockID;
        byte blockState;
        try
        {
            blockID = main.world.planet[0].map.map[position.x, position.y].ToString().ToCharArray();
            blockState = (byte)(byte.Parse(blockID[0].ToString()) - 1);
        }
        catch (Exception ex)
        {
            Log.LogError($"BlockModify.deleteBlockParts: Cannot extract blockState from block main position ({position.x},{position.y}). Error code: {ex}");
            return false;
        }
        Block block = Main.blockList[main.world.planet[0].map.map[position.x, position.y]];
        BuildingState buildingState = block.building[blockState];

        IBlockInstance? blockInstance = blockInstanceManager.GetBlockInstance(position);
        if (blockInstance == null)
        {
            Log.LogError($"BlockModify.deleteBlockParts: BlockInstance not found. Position: {position}");
            return false;
        }

        byte? rotationID = FindRotationID(buildingState, blockInstance.rotation);
        if (rotationID == null)
        {
            Log.LogError($"BlockModify.deleteBlockParts: Block rotation not found. BlockID: {blockID}");
        }

        foreach(BuildingTile buildingTile in buildingState.buildingRotations[(byte)rotationID].buildingParts)
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

    /*
    public short MainBlockWorldMapCodeToBlockCode(short worldMapBlockID)
    {
        print($"worldMapCode: {worldMapBlockID}");
        char[] _worldMapCode;
        char[] blockCode = new char[4];
        try
        {
            _worldMapCode = worldMapBlockID.ToString().ToCharArray();
            for(int i = 0; i < 4; i++)
            {
                blockCode[i] = _worldMapCode[i + 1];
            }
        }
        catch (Exception ex)
        {
            print($"Error while converting MainBlockWorldMapCode to BlockCode. Error code: {ex}");
        }
        string _blockID = string.Empty;
        foreach (char c in blockCode)
        {
            _blockID += c;
        }
        print($"int.Parse(_blockCode): {int.Parse(_blockID)}");
        return (short)int.Parse(_blockID);
    }
    */

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
            Log.LogError("BlockModify.GetTerrainTileHardness: this position is not a matter");
            return -1;
        }
        return Main.matterList[main.world.planet[0].map.map[position.x, position.y] * (-1)].hardness;
    }

    public short GetBlockHardness(Vector2Int position)
    {
        if (main.world.planet[0].map.map[position.x, position.y] <= 0)
        {
            Log.LogError("BlockModify.GetBlockHardness: this position is not a block");
            return -1;
        }
        return Main.blockList[main.world.planet[0].map.map[position.x, position.y]].destructionTime;
    }
}
