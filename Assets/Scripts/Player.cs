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
    [SerializeField] Main main;
    [SerializeField] Tilemap collidableBlock;
    [SerializeField] TerrainGeneration terrainGeneration;

    [Header("Movement")]
    [SerializeField] Rigidbody2D rb;
    public float moveSpeed;
    public float jumpSpeed;
    public bool jumpEnabled;
    public bool isGround;
    public bool isJumping;
    public Vector2 lastGroundTouchPos;
    public Vector2 lastGroundTouchPosFallDamageCalculation;
    public bool fallDamageCalculated;
    public bool inLadder;
    public float ladderSpeed;

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
    public PlayerInfo playerInfo = new PlayerInfo();
    int oxygenTick;
    public GameObject healthGage;
    public GameObject oxygenGage;
    int cureTickAmount;
    public GameObject damgeText;
    public short calories;

    [Header("UI")]
    public TextMesh nameDisplay;

    [Header("Interaction")]
    public BlockInstanceManager blockInstanceManager;
    public bool didInteract = false;

    void Awake()
    {
        jumpEnabled = true;
        isJumping = false;
        isModifyingTerrain = false;
        modifyTerrainTileTickRemain = 0;
        jetEnabled = false;
        jetFuelLeft = 0;
        inLadder = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<Rigidbody2D>().gravityScale = 0;
        this.GetComponent<Collider2D>().enabled = false;
        toolMode = ToolMode.None;
        nameDisplay.text = playerInfo.playerName;
        backpack.CloseInventory();
    }

    // Update is called once per frame
    void Update()
    {
        playerInfo.playerPosition = new Vector4(transform.position.x, transform.position.y, 0, 0);
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
        float x = Input.GetAxisRaw("Horizontal") * Time.deltaTime;
        float y = Input.GetAxisRaw("Vertical") * Time.deltaTime;

        if (y != 0) Ladder(y > 0);

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

                    if (main.world.planet[0].map.map[pos.x, pos.y] < 0 && main.world.planet[0].map.map[pos.x, pos.y] > -20000)
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
                        destructBlock(pos);
                    }
                    else if (main.world.planet[0].map.map[pos.x, pos.y] < 0 && main.world.planet[0].map.map[pos.x, pos.y] <= -20000)
                    {
                        destructBlock(pos);
                    }
                }
                else if (Input.GetKey(KeyCode.LeftShift))
                {
                    if (checkSetBlockAvailable(pos, Main.itemList[backpack.slots[backpack.index].itemID].placeableID))
                    {
                        if (backpack.slots[backpack.index].itemID != 0)
                        {
                            if (Main.itemList[backpack.slots[backpack.index].itemID].type == ItemType.Block)
                            {
                                if (backpack.slots[backpack.index].amount > 0)
                                {
                                    // TODO: custom rotation
                                    setBlock(pos, Rotation.None);
                                }
                            }
                        }
                    }
                }
                else
                {
                    // Block Interaction
                    if (!didInteract)
                    {
                        short blockID = main.world.planet[0].map.map[pos.x, pos.y];
                        if (blockID > 0)
                        {
                            if (Main.blockList[blockID].type == BlockType.Building)
                            {
                                blockInstanceManager.InteractBlock(pos);
                                didInteract = true;
                            }
                        }
                        else if (blockID <= -20000)
                        {
                            blockInstanceManager.InteractBlock((Vector2Int)blockModify.BlockRelativePosToBlockMainPos(pos));
                            didInteract = true;
                        }
                    }
                }
            }
        }
        else if (Input.GetMouseButtonDown(1))
        {
            backpack.UseItem();
        }
        else
        {
            isModifyingTerrain = false;
            modifyTerrainTileTickRemain = 0;
            currentModifyTerrainTileOriginalTick = 0;
            didInteract = false;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (isBackpackOpen)
            {
                backpack.CloseInventory();
                isBackpackOpen = false;
            }
            else if (!isBackpackOpen)
            {
                backpack.OpenInventory();
                isBackpackOpen = true;
            }
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
            backpack.ChangeHotBarPos(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            backpack.ChangeHotBarPos(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            backpack.ChangeHotBarPos(2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            backpack.ChangeHotBarPos(3);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            backpack.ChangeHotBarPos(4);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            backpack.ChangeHotBarPos(5);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            backpack.ChangeHotBarPos(6);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            backpack.ChangeHotBarPos(7);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            backpack.ChangeHotBarPos(8);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            backpack.ChangeHotBarPos(9);
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

    public void playerTick()
    {
        doOxygenTick();
        cureTick();
    }

    private void calculateAnimation()
    {
        if (modifyTerrainTileTickRemain > 0 && currentModifyTerrainTileOriginalTick > 0)
        {
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
        if (main.world.planet[0].map.map[position.x, position.y] < 0) return true;
        return false;
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
                    itemManager.spawnItem((Vector2)collidableBlock.CellToWorld((Vector3Int)position) + new Vector2(0.5f, 0.2f), new ItemStack(Main.matterList[main.world.planet[0].map.map[position.x, position.y] * (-1)].itemID, 32));
                    blockModify.ModifyTerrain(position, 0);
                    modifyTerrainTileTickRemain = 0;
                    currentModifyTerrainTileOriginalTick = 0;
                    isModifyingTerrain = false;
                    return;
                }
                modifyTerrainTileTickRemain -= digAmount;
            }
            else
            {
                diggingPos = position;
                currentModifyTerrainTileOriginalTick = (short)(blockModify.GetTerrainTileHardness(position) * digTickMultiply);
                modifyTerrainTileTickRemain = currentModifyTerrainTileOriginalTick;
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
        ItemStack result = backpack.AddItemToBackpack(droppedItemInstance.itemStack);
        int entityID = droppedItemInstance.entityID;
        if (result.amount == 0)
        {
            main.world.planet[0].map.entitySystem.visibleEntities.RemoveAt(entityID);
            main.world.planet[0].map.entitySystem.spawnedEntityGameObject.RemoveAt(droppedItemInstance.spawnedItemGameObjectID);
            main.world.planet[0].map.entitySystem.droppedItemSystem.droppedItemData.RemoveAt(droppedItemInstance.droppedItemDataID);
            Destroy(itemObject);
        }
        else
        {
            if (main.world.planet[0].map.entitySystem.visibleEntities[entityID].type == EntityType.DroppedItem)
            {
                if (main.world.planet[0].map.entitySystem.droppedItemSystem.droppedItemData[
                    main.world.planet[0].map.entitySystem.visibleEntities[entityID].relatedID].itemStack.amount - result.amount > 0)
                {
                    main.world.planet[0].map.entitySystem.droppedItemSystem.droppedItemData[
                    main.world.planet[0].map.entitySystem.visibleEntities[entityID].relatedID].itemStack.amount -= result.amount;
                }
            }
        }
        itemManager.updateEntityRelatedID();
        itemManager.updateDroppedItemInstance(entityID);
    }

    public void doOxygenTick()
    {
        if (!terrainGeneration.isLoaded) return;

        if (playerInfo.oxygen > 0)
        {
            oxygenTick++;
        }
        if (oxygenTick == 60)
        {
            playerInfo.oxygen--;
            oxygenTick = 0;
        }
        updateGage();
    }

    public void damage(short amount)
    {
        if (playerInfo.health - amount > 0)
        {
            playerInfo.health -= amount;
        }
        else
        {
            playerInfo.health = 0;
            print("Player Died");
        }
        Instantiate(damgeText, transform.position + new Vector3(0, 2, 0), Quaternion.identity).GetComponent<DamageText>().text.text = "-" + amount.ToString();
        updateGage();
    }

    public void cure(short amount)
    {
        if (playerInfo.health + amount > playerInfo.maxHealth)
        {
            playerInfo.health = playerInfo.maxHealth;
        }
        else
        {
            playerInfo.health += amount;
        }
        updateGage();
    }

    private void cureTick()
    {
        cureTickAmount++;
        if (cureTickAmount == 20 * 10)
        {
            cureTickAmount = 0;
            if (playerInfo.health >= 90) cure(1);
        }
    }

    private void updateGage()
    {
        healthGage.transform.GetChild(0).localScale = new Vector3(1.9f * (playerInfo.health / (float)playerInfo.maxHealth), healthGage.transform.GetChild(0).localScale.y, healthGage.transform.GetChild(0).localScale.z);
        oxygenGage.transform.GetChild(0).localScale = new Vector3(1.9f * (playerInfo.oxygen / (float)playerInfo.maxOxygen), oxygenGage.transform.GetChild(0).localScale.y, oxygenGage.transform.GetChild(0).localScale.z);
    }

    private void calculateFallDamage()
    {
        if (lastGroundTouchPosFallDamageCalculation.y - transform.position.y > 2)
        {
            if (isGround)
            {
                damage((short)Mathf.FloorToInt((lastGroundTouchPosFallDamageCalculation.y - transform.position.y - 2) * 10));
                fallDamageCalculated = true;
            }
            else
            {
                fallDamageCalculated = false;
            }
        }
    }

    private void setBlock(Vector2Int position, Rotation rotation)
    {
        bool result = blockModify.SetBlock(position, Main.itemList[backpack.slots[backpack.index].itemID].placeableID, rotation);
        if (result)
        {
            backpack.RemoveItem(backpack.index, 1);
        }
    }

    private bool checkSetBlockAvailable(Vector2Int position, short blockID)
    {
        if (main.world.planet[0].map.map[position.x, position.y] != 0) return false;

        Block block = Main.blockList[blockID];

        if (block.isLadder) return true;
        if (block.type == BlockType.Tile && !block.isCollidable) return true;

        Vector2Int playerTilePos = (Vector2Int)collidableBlock.WorldToCell(this.transform.position);
        switch (block.type)
        {
            case BlockType.Tile:
                if (position != playerTilePos && position != playerTilePos + new Vector2Int(0, 1))
                {
                    return true;
                }
                break;
            case BlockType.Building:
                foreach (var buildingPart in block.building[block.defaultStateCode].buildingRotations[block.building[block.defaultStateCode].defaultRotation].buildingParts)
                {
                    if (!buildingPart.isCollidable) continue;

                    var pos = position + buildingPart.pos;

                    if (pos != playerTilePos && pos != playerTilePos + new Vector2Int(0, 1)) continue;
                    return false;
                }
                return true;
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
                    (short, Vector2Int?) result = blockModify.DeleteBlock(position);
                    itemManager.spawnItem((Vector2)collidableBlock.CellToWorld((Vector3Int)result.Item2) + new Vector2(0.5f, 0.2f), new ItemStack(Main.blockList[result.Item1].itemID, 1));
                    isDestructingBlock = false;
                    destructBlockTickRemain = 0;
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision == null) return;

        if (collision.gameObject.layer == 7)
        {
            inLadder = true;
            isJumping = false;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 7) LadderOut();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision == null) return;
        
        if (collision.gameObject.layer == 7)
        {
            inLadder = true;
            rb.velocity = new Vector2(0, 0);
            rb.gravityScale = 0;
            isJumping = false;
        }
    }

    private void Ladder(bool state)
    {
        if (inLadder)
        {
            rb.velocity = new Vector2(0, 0);
            rb.gravityScale = 0; // default: 2
            transform.Translate(0, (state ? ladderSpeed : (-ladderSpeed)) * Time.deltaTime, 0);
            lastGroundTouchPosFallDamageCalculation = transform.position;
            isJumping = false;
        }
    }

    private void LadderOut()
    {
        rb.gravityScale = 2;
        gameObject.layer = 10;
        inLadder = false;
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
