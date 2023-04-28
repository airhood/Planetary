using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Game
{
    public void LoadWorld(string worldName)
    {

    }
}

public struct GameSettings
{
    public int resolution;
    public int volume;
}

[System.Serializable]
public class World
{
    public WorldInfo worldInfo;
    public PlayerInfoData playerinfoData;
    public Planet[] planet = new Planet[1];
}

[System.Serializable]
public struct WorldInfo
{
    public string WorldName;
    public GameTime GameTime;
}

[System.Serializable]
public class Planet
{
    public WorldMap map = new WorldMap();
}

[System.Serializable]
public class WorldMap
{
    public short[,] map = new short[700, 400];
    public ConveyerSystem conveyerSystem = new ConveyerSystem();
    public LiquidPipeSystem liquidPipeSystem = new LiquidPipeSystem();
    public GasPipeSystem gasPipeSystem = new GasPipeSystem();
    public EntitySystem entitySystem = new EntitySystem();
}

[System.Serializable]
public class EntitySystem
{
    public List<EntityData> visibleEntities = new List<EntityData>();
    public List<EntityData> hiddenEntities = new List<EntityData>();
    public List<GameObject> spawnedEntityGameObject = new List<GameObject>();
    public DroppedItemSystem droppedItemSystem = new DroppedItemSystem();
}

[System.Serializable]
public class DroppedItemSystem
{
    public List<DroppedItemData> droppedItemData = new List<DroppedItemData>();
}

[System.Serializable]
public class GameTime
{
    public int Year;
    public int Day;
    public int hour;
}

[System.Serializable]
public class ConveyerSystem
{
    public List<MovingItem> movingItemMemory = new List<MovingItem>();
    public ConveyerBelt[,] conveyerOverlayMap = new ConveyerBelt[100, 100];
}

[System.Serializable]
public class LiquidPipeSystem
{

}

[System.Serializable]
public class GasPipeSystem
{

}

[System.Serializable]
public class DroppedItem
{
    public Matter matter;
    public Item item;
}

public class PlayerInfoData
{
    public Vector2 playerPos;
    public byte hotBarPos;
    public Slot[] backpack = new Slot[40];
    public int ResearchPoint;
    public PlayerInfo playerInfo;
}

[System.Serializable]
public class CustomArray<T>
{
    public List<int> indexAvailable = new List<int>();
    public T[] array;
    public int length;
    public int amount;

    public T this[int index] { get { return array[index]; } set { array[index] = value; } }
    public CustomArray(int length)
    {
        this.length = length;
        array = new T[length];
        for(int i = 0; i < length; i++)
        {
            indexAvailable.Add(i);
        }
        amount = 0;
    }

    public int Add(T item)
    {
        int index = indexAvailable[0];
        array[indexAvailable[0]] = item;
        indexAvailable.RemoveAt(0);
        amount++;
        return index;
    }

    public void Remove(int index)
    {
        array[index] = default(T);
        indexAvailable.Add(index);
        amount--;
    }
}

public class Main : MonoBehaviour
{
    public World world = new World();

    [SerializeField] Conveyer conveyer;

    public static List<Block> blockList = new List<Block>();
    public static List<Matter> matterList = new List<Matter>();
    public static List<Item> itemList = new List<Item>();

    public List<Block> _blockList = new List<Block>();
    public List<Matter> _matterList = new List<Matter>();
    public List<Item> _itemList = new List<Item>();

    public Player player;
    public EntityHandler entityHandler;

    bool tick;

    void Awake()
    {
        Application.targetFrameRate = 80;
        blockList = _blockList;
        matterList = _matterList;
        itemList = _itemList;
        world = new World();
        world.planet[0] = new Planet();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate()
    {
        if (tick)
        {
            Tick();
        }
        tick = !tick;
    }

    private void Tick()
    {
        // electricWire calculate
        //conveyer.CalculateConveyer();
        // liquidPipe calculate
        // gasPipe calculate
        // machine calculate
        player.playerTick();
        entityHandler.EntityTick();
    }
}
