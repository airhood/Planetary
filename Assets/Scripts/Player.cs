using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Tilemaps;

public enum HandMode
{
    Normal, Set, Break
}

public enum ViewMode
{
    Normal, LiquidPipe, GasPipe, Electricity
}

public enum HotKeyMode
{
    Compass,
}

public enum ToolMode
{
    None, Drill, Destruction
}

[System.Serializable]
public class Player : MonoBehaviour
{
    [Header("World")]
    public Main main;
    public Tilemap collidableBlock;
    public TerrainGeneration terrainGeneration;

    [Header("Movement")]
    public Rigidbody2D rb;
    public float moveSpeed;
    public float jumpSpeed;
    public bool jumpEnabled;
    public bool isGround;
    public bool isJumping;
    public Vector2 lastGroundTouchPos;
    public Vector2 lastGroundTouchPosFallDamageCalculation;
    public bool fallDamageCalculated;

    [Header("Animation")]
    public Animator animator;
    public SpriteRenderer spriteRenderer;


    [Header("Backpack")]
    public Backpack backpack;
    public bool isBackpackOpen;
    public int ResearchPoint;
    public float itemCollectDistance;
    public short itemCollectTick;

    [Header("Tool")]
    public ToolMode toolMode;

    [Header("Dig")]
    public bool isModifyingTerrain;
    public short currentModifyTerrainTileOriginalTick;
    public short modifyTerrainTileTickRemain;
    public Vector2Int diggingPos;
    public BlockModify blockModify;
    public short digAmount = 1;
    public short digTickMultiply;
    public Sprite[] matterDiggingAnimationImage = new Sprite[9];
    public GameObject matterDigging;
    public SpriteRenderer matterDiggingAnimation;
    public ItemManager itemManager;

    [Header("Destruction")]
    public bool isDestructingBlock;
    public short currentDestructingBlockOriginalTick;
    public short destructBlockTickRemain;
    public Vector2Int destructingPos;
    public short destructAmount;
    public short destructMultiply;

    [Header("Jet")]
    public bool jetEnabled;
    public float jetFuelLeft;

    [Header("Gizmos")]
    public List<short> gizmosList = new List<short>();

    Vector3 worldPosition;
    Vector2Int pos;

    [Header("Player Property")]
    public int maxHealth;
    public int health;
    public int maxOxygen;
    public int oxygen;
    int oxygenTick;
    public GameObject healthGage;
    public GameObject oxygenGage;

    [Header("UI")]
    public GameObject sidePopUp;

    void Awake()
    {
        jumpEnabled = true;
        isJumping = false;
        isModifyingTerrain = false;
        modifyTerrainTileTickRemain = 0;
        jetEnabled = false;
        jetFuelLeft = 0;
    }

    // Start is called before the first frame update
    void Start()
    {
        toolMode = ToolMode.None;
    }

    // Update is called once per frame
    void Update()
    {
        MovePlayer();
        CheckInput();
        calculateAnimation();
        itemCollecting();
        if (terrainGeneration.isLoaded)
        {
            calculateFallDamage();
        }
    }

    private void MovePlayer()
    {
        float x = Input.GetAxisRaw("Horizontal");

        if (x > 0)
        {
            spriteRenderer.flipX = false;
        }
        else if (x < 0)
        {
            spriteRenderer.flipX = true;
        }

        rb.velocity = new Vector2(x * moveSpeed, rb.velocity.y);

        if (x != 0)
        {
            animator.SetBool("Walk", true);
        }
        else animator.SetBool("Walk", false);

        isGround = Physics2D.OverlapCircle((Vector2)transform.position, 0.03f, 1 << LayerMask.NameToLayer("Ground"));
        //isGround = checkGround();

        if (Input.GetKeyDown(KeyCode.Space) && isGround) Jump();
        else if (!Input.GetKeyDown(KeyCode.Space) && isGround)
        {
            isJumping = false;
            lastGroundTouchPos = transform.position;
            if (fallDamageCalculated)
            {
                lastGroundTouchPosFallDamageCalculation = lastGroundTouchPos;
            }
        }

        animator.SetBool("Jump", isJumping);
    }

    private void Jump()
    {
        rb.velocity = Vector2.zero;
        rb.AddForce(Vector2.up * jumpSpeed);
        isJumping = true;
    }

    private void OnDrawGizmos()
    {
        foreach(int mode in gizmosList)
        {
            switch(mode)
            {
                case 0:
                    Gizmos.DrawLine(this.transform.position, worldPosition);
                    break;
                case 1:
                    Gizmos.DrawWireCube(collidableBlock.CellToWorld((Vector3Int)pos) + new Vector3(0.5f, 0.5f, 0), new Vector3(1, 1, 0));
                    break;
            }
        }

        gizmosList.Clear();
    }

    private void CheckInput()
    {

        Vector2 mousePosition = (Vector2)Input.mousePosition;

        if (Input.GetMouseButton(0))
        {
            worldPosition = Camera.main.ScreenToWorldPoint((Vector3)mousePosition);
            pos = (Vector2Int)collidableBlock.WorldToCell(worldPosition);

            if (pos.x >= 0 && pos.y >= 0)
            {
                if (toolMode == ToolMode.Drill)
                {
                    gizmosList.Add(0);

                    if (main.world.planet[0].map.map[pos.x, pos.y] < 0)
                    {
                        gizmosList.Add(1);

                        if (checkDigAvailable(pos) || checkDigAvailable(pos + new Vector2Int(0, 1)))
                        {
                            Dig(pos);
                        }
                    }
                    else
                    {
                        isModifyingTerrain = false;
                        modifyTerrainTileTickRemain = 0;
                        currentModifyTerrainTileOriginalTick = 0;
                    }
                }
                else if (toolMode == ToolMode.Destruction)
                {
                    if (main.world.planet[0].map.map[pos.x, pos.y] > 0)
                    {
                        if (Main.blockList[main.world.planet[0].map.map[pos.x, pos.y]].type == BlockType.Tile)
                        {
                            destructBlock(pos);
                        }
                    }
                }
                else
                {
                    if (checkSetBlockAvailable(pos))
                    {
                        if (backpack.slots[backpack.index].itemID != 0)
                        {
                            if (Main.itemList[backpack.slots[backpack.index].itemID].type == ItemType.Block)
                            {
                                setBlock(pos);
                            }
                        }
                    }
                }
            }
        }
        if (!Input.GetMouseButton(0))
        {
            isModifyingTerrain = false;
            modifyTerrainTileTickRemain = 0;
            currentModifyTerrainTileOriginalTick = 0;
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (isBackpackOpen) closeBackpack();
            else if (!isBackpackOpen) openBackpack();
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            if (toolMode == ToolMode.Drill)
            {
                toolMode = ToolMode.None;
            }
            else
            {
                toolMode = ToolMode.Drill;
            }
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            if (toolMode == ToolMode.Destruction)
            {
                toolMode = ToolMode.None;
            }
            else
            {
                toolMode = ToolMode.Destruction;
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            backpack.changeHotBarPos(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            backpack.changeHotBarPos(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            backpack.changeHotBarPos(2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            backpack.changeHotBarPos(3);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            backpack.changeHotBarPos(4);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            backpack.changeHotBarPos(5);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            backpack.changeHotBarPos(6);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            backpack.changeHotBarPos(7);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            backpack.changeHotBarPos(8);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            backpack.changeHotBarPos(9);
        }
        else if (Input.GetKeyDown(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Alpha1))
        {

        }
        else if (Input.GetKeyDown(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Alpha2))
        {

        }
        else if (Input.GetKeyDown(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Alpha3))
        {

        }
        else if (Input.GetKeyDown(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Alpha4))
        {

        }
        else if (Input.GetKeyDown(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Alpha5))
        {

        }
        else if (Input.GetKeyDown(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Alpha6))
        {

        }
        else if (Input.GetKeyDown(KeyCode.M))
        {
            if (jetFuelLeft != 0)
            {
                if (!jetEnabled)
                {
                    jetEnabled = true;
                }

                jetFuelLeft -= 1;
            }
            else
            {
                if (jetEnabled)
                {
                    jetEnabled = false;
                }
            }
        }
        else if (Input.GetKeyDown(KeyCode.Q))
        {

        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {

        }
    }

    private void openBackpack()
    {
        sidePopUp.SetActive(true);
    }

    private void closeBackpack()
    {
        sidePopUp.SetActive(false);
    }

    private void calculateAnimation()
    {
        if (modifyTerrainTileTickRemain > 0 && currentModifyTerrainTileOriginalTick > 0)
        {
            //print((int)Mathf.Ceil(((currentModifyTerrainTileOriginalTick - modifyTerrainTileTickRemain) / (float)currentModifyTerrainTileOriginalTick) * 9));
            matterDiggingAnimation.sprite = matterDiggingAnimationImage[(int)Mathf.Ceil(((currentModifyTerrainTileOriginalTick - modifyTerrainTileTickRemain) / (float)currentModifyTerrainTileOriginalTick) * 9) - 1];
            matterDigging.transform.position = (Vector3)((Vector2)(Vector2Int)collidableBlock.WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition)) + new Vector2(0.5f, 0.5f));
            matterDigging.SetActive(true);
        }
        else
        {
            matterDigging.SetActive(false);
        }
    }

    private bool checkDigAvailable(Vector2Int position)
    {
        return true;
    }

    public void Dig(Vector2Int position)
    {
        if (isModifyingTerrain)
        {
            if (position == diggingPos)
            {
                if (modifyTerrainTileTickRemain <= 0)
                {
                    //backpack.addItemToBackpack(Main.matterList[main.world.planet[0].map.map[position.x, position.y]].itemID, 1);
                    itemManager.spawnItem(Main.matterList[main.world.planet[0].map.map[position.x, position.y] * (-1)].itemID, (Vector2)collidableBlock.CellToWorld((Vector3Int)position) + new Vector2(0.5f, 0.2f), 32);
                    blockModify.ModifyTerrain(position, 0);
                    isModifyingTerrain = false;
                    modifyTerrainTileTickRemain = 0;
                    currentModifyTerrainTileOriginalTick = 0;
                    return;
                }
                modifyTerrainTileTickRemain -= digAmount;
            }
            else
            {
                diggingPos = position;
                modifyTerrainTileTickRemain = (short)(blockModify.GetTerrainTileHardness(position) * digTickMultiply);
                modifyTerrainTileTickRemain -= digAmount;
            }
        }
        else
        {
            isModifyingTerrain = true;
            diggingPos = position;
            currentModifyTerrainTileOriginalTick = (short)(blockModify.GetTerrainTileHardness(position) * digTickMultiply);
            modifyTerrainTileTickRemain = currentModifyTerrainTileOriginalTick;
            modifyTerrainTileTickRemain -= digAmount;
        }
    }

    private void itemCollecting()
    {
        GameObject[] items = detectNearItems();

        for (int i = 0; i < items.Length; i++)
        {
            doItemCollectTick(items[i]);
        }
    }

    private GameObject[] detectNearItems()
    {
        Collider2D[] results = Physics2D.OverlapCircleAll(transform.position, itemCollectDistance);
        List<GameObject> items = new List<GameObject>();

        for(int i = 0; i < results.Length; i++)
        {
            if (results[i].gameObject.CompareTag("dropped_item"))
            {
                items.Add(results[i].gameObject);
            }
        }
        return items.ToArray();
    }

    private void doItemCollectTick(GameObject itemObject)
    {
        DroppedItemInstance droppedItemInstance = itemObject.GetComponent<DroppedItemInstance>();
        if (!droppedItemInstance.isBeingCollected)
        {
            droppedItemInstance.isBeingCollected = true;
            droppedItemInstance.collectTickLeft = itemCollectTick;
        }
        
        droppedItemInstance.collectTickLeft--;
        
        if (droppedItemInstance.collectTickLeft == 0)
        {
            collectItem(itemObject);
            return;
        }
    }

    private void collectItem(GameObject itemObject)
    {
        DroppedItemInstance droppedItemInstance = itemObject.GetComponent<DroppedItemInstance>();
        backpack.addItemToBackpack(droppedItemInstance.itemID, droppedItemInstance.amount);
        Destroy(itemObject);
    }

    public void doOxygenTick()
    {
        if (!terrainGeneration.isLoaded) return;

        if (oxygen > 0)
        {
            oxygenTick++;
        }
        if (oxygenTick == 60)
        {
            oxygen--;
            oxygenTick = 0;
        }
        updateGage();
    }

    public void damage(int amount)
    {
        if (health - amount > 0)
        {
            health -= amount;
        }
        else
        {
            health = 0;
            print("Player Died");
        }
        updateGage();
    }

    public void cure(int amount)
    {
        if (health + amount > maxHealth)
        {
            health = maxHealth;
        }
        else
        {
            health += amount;
        }
        updateGage();
    }

    private void updateGage()
    {
        healthGage.transform.GetChild(0).localScale = new Vector3(1.9f * (health / (float)maxHealth), healthGage.transform.GetChild(0).localScale.y, healthGage.transform.GetChild(0).localScale.z);
        oxygenGage.transform.GetChild(0).localScale = new Vector3(1.9f * (oxygen / (float)maxOxygen), oxygenGage.transform.GetChild(0).localScale.y, oxygenGage.transform.GetChild(0).localScale.z);
    }

    private void calculateFallDamage()
    {
        if (lastGroundTouchPosFallDamageCalculation.y - transform.position.y > 2)
        {
            if (isGround)
            {
                damage(Mathf.FloorToInt((lastGroundTouchPosFallDamageCalculation.y - transform.position.y - 2) * 10));
                fallDamageCalculated = true;
            }
            else
            {
                fallDamageCalculated = false;
            }
        }
    }

    private void setBlock(Vector2Int position)
    {
        blockModify.SetBlock(position, Main.itemList[backpack.slots[backpack.index].itemID].placeableID);
        backpack.removeItem(backpack.index, 1);
    }

    private bool checkSetBlockAvailable(Vector2Int position)
    {
        if (main.world.planet[0].map.map[position.x, position.y] == 0)
        {
            Vector2Int playerTilePos = (Vector2Int)collidableBlock.WorldToCell(this.transform.position);
            if (position != playerTilePos && position != playerTilePos + new Vector2Int(0, 1))
            {
                return true;
            }
        }

        return false;
    }

    private void destructBlock(Vector2Int position)
    {
        if (isDestructingBlock)
        {
            if (position == destructingPos)
            {
                if (destructBlockTickRemain <= 0)
                {
                    itemManager.spawnItem(Main.blockList[main.world.planet[0].map.map[position.x, position.y]].itemID, (Vector2)collidableBlock.CellToWorld((Vector3Int)position) + new Vector2(0.5f, 0.2f), 1);
                    blockModify.DeleteBlock(position);
                    isDestructingBlock = false;
                    destructBlockTickRemain = 0; ;
                    currentDestructingBlockOriginalTick = 0;
                    return;
                }
                destructBlockTickRemain -= destructAmount;
            }
            else
            {
                destructingPos = position;
                destructBlockTickRemain = (short)(blockModify.GetBlockHardness(position) * destructMultiply);
                destructBlockTickRemain -= destructAmount;
            }
        }
        else
        {
            isDestructingBlock = true;
            destructingPos = position;
            currentDestructingBlockOriginalTick = (short)(blockModify.GetBlockHardness(position) * destructMultiply);
            destructBlockTickRemain = currentDestructingBlockOriginalTick;
            destructBlockTickRemain -= destructAmount;
        }
    }

    private bool checkGround()
    {
        LayerMask layerMask = 1 << 6;
        RaycastHit2D hitInfo = Physics2D.Raycast(transform.position, Vector2.down, 10f, layerMask);

        if (hitInfo.collider == null) return false;

        if (hitInfo.distance > 0.1f)
        {
            return false;
        }

        return true;
    }
}
