 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public enum Direction
{
    Up, Down, Left, Right, Null
}

[System.Serializable]
public class MovingItem
{
    public MovingItem(GameObject gameObject, Direction movingDirection, Matter item)
    {
        this.gameObject = gameObject;
        this.movingDirection = movingDirection;
        this.item = item;
    }

    public GameObject gameObject;
    public Direction movingDirection;
    public Matter item;
}

[System.Serializable]
public class ConveyerBelt
{
    public Direction dir;
    public Vector2Int bridgeEndPoint;

    public ConveyerBelt(Direction dir)
    {
        this.dir = dir;
    }
}

public class Conveyer : MonoBehaviour
{
    public Main main;

    public int amount;

    int index;
    int tickDuration;

    //public Direction movingDirection;
    public float movingSpeed;

    public Transform solid;

    public GameObject conveyerCart;

    // Start is called before the first frame update
    void Start()
    {
        if (amount > 100000) return;

        
        for (int i = 0; i < 5; i++)
        {
            main.world.planet[0].map.conveyerSystem.conveyerOverlayMap[i, 5] = new ConveyerBelt(Direction.Right);
        }

        for (int i = 5; i > 0; i--)
        {
            main.world.planet[0].map.conveyerSystem.conveyerOverlayMap[5, i] = new ConveyerBelt(Direction.Down);
        }

        for (int i = 5; i > 0; i--)
        {
            main.world.planet[0].map.conveyerSystem.conveyerOverlayMap[i, 0] = new ConveyerBelt(Direction.Left);
        }

        for (int i = 0; i < 5; i++)
        {
            main.world.planet[0].map.conveyerSystem.conveyerOverlayMap[0, i] = new ConveyerBelt(Direction.Up);
        }

        //print(CheckConveyerItemPerfectArrive(new Vector2(11.0f, 5.0f)));

        SpawnItem(new Vector2Int(0, 5), 0);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void CalculateConveyer()
    {
        MoveItem();
    }

    private void SpawnItem(Vector2Int position, short itemID)
    {

        GameObject obj = Instantiate(conveyerCart, (Vector3Int)position, Quaternion.identity);
        obj.AddComponent<Image>().sprite = Main.data.itemList[itemID].image;
        obj.tag = "conveyer_solid_item";
        obj.transform.parent = solid;
        main.world.planet[0].map.conveyerSystem.movingItemMemory.Add(new MovingItem(obj, Direction.Null, Main.data.matterList[itemID]));

        for (int i = 0; i < amount; i++)
        {
            
        }
    }

    private void MoveItem()
    {
        //GameObject[] gameobjects = GameObject.FindGameObjectsWithTag("conveyer_solid_item");

        for(int i = 0; i < main.world.planet[0].map.conveyerSystem.movingItemMemory.Count; i++)
        {
            //print(movingItemMemory[i].gameObject.transform.position.y);
            if (CheckConveyerItemPerfectArrive(main.world.planet[0].map.conveyerSystem.movingItemMemory[i].gameObject.transform.position))
            {
                print("go");
                Vector2Int pos = new Vector2Int((int)Mathf.Floor(main.world.planet[0].map.conveyerSystem.movingItemMemory[i].gameObject.transform.position.x), (int)Mathf.Floor(main.world.planet[0].map.conveyerSystem.movingItemMemory[i].gameObject.transform.position.y));
                ConveyerBelt conveyerBelt = main.world.planet[0].map.conveyerSystem.conveyerOverlayMap[pos.x, pos.y];
                //print(conveyerBelt);
                if (conveyerBelt == null)
                {
                    main.world.planet[0].map.conveyerSystem.movingItemMemory[i].movingDirection = Direction.Null;
                    return;
                }
                else
                {
                    main.world.planet[0].map.conveyerSystem.movingItemMemory[i].movingDirection = conveyerBelt.dir;
                }
                
            }

            switch (main.world.planet[0].map.conveyerSystem.movingItemMemory[i].movingDirection)
            {
                case Direction.Up:
                    main.world.planet[0].map.conveyerSystem.movingItemMemory[i].gameObject.transform.position += new Vector3(0, movingSpeed, 0);
                    break;
                case Direction.Down:
                    main.world.planet[0].map.conveyerSystem.movingItemMemory[i].gameObject.transform.position += new Vector3(0, (-1) * movingSpeed, 0);
                    break;
                case Direction.Left:
                    main.world.planet[0].map.conveyerSystem.movingItemMemory[i].gameObject.transform.position += new Vector3((-1) * movingSpeed, 0, 0);
                    break;
                case Direction.Right:
                    main.world.planet[0].map.conveyerSystem.movingItemMemory[i].gameObject.transform.position += new Vector3(movingSpeed, 0, 0);
                    break;
                case Direction.Null:
                    break;
            }
        }
    }

    private bool CheckConveyerItemPerfectArrive(Vector2 pos)
    {
        if ((Mathf.Abs(pos.x - Mathf.Floor(pos.x)) < 0.1f) && (Mathf.Abs(pos.y - Mathf.Floor(pos.y)) < 0.1f))
        {
            return true;
        }

        return false;
    }
}
