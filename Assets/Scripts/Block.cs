using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum PortType
{
    ElectricWire, LiquidPipe, GasPipe, ConveyerBelt
}

public enum FlowDirection
{
    NotSet,Input, Output
}

public enum BlockType
{
    Tile, Building
}

public enum Blocks
{

}

[System.Serializable]
public struct Port
{
    public string portName;
    public PortType type;
    public Vector2Int relativePos;
    public FlowDirection flow;
}

[System.Serializable]
public class BuildingTile
{
    public Vector2Int pos;
    public Tile tile;
    public bool isCollidable;
}

[System.Serializable]
public enum Rotation
{
    None, UD, RL, URDL
}

[System.Serializable]
public class BuildingState
{
    public string stateName;
    public List<BuildingTile> buildingParts = new List<BuildingTile>();
}

[CreateAssetMenu(menuName = "Block", fileName = "new Block", order = 31)]
public class Block : ScriptableObject
{
    [Header("Info")]
    public short id;
    public string name;
    public string description;
    public BlockType type;

    public short destructionTime;

    public short itemID;

    [Header("Tile")]
    public RuleTile tile;

    [Header("Building")]
    public Vector2Int BuildPoint;
    public Rotation rotation;
    public bool buildOnAirAvailable;
    public bool hasInstance;
    public byte defaultStateCode;
    public List<BuildingState> building = new List<BuildingState>();

    [Header("Port")]
    public List<Port> ports = new List<Port>();
}