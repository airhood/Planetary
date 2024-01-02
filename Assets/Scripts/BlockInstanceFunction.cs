using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockInstanceFunction : MonoBehaviour
{
    public static void water_electrolysis__interact(BlockInstance blockInstance)
    {
        print("interacted " + blockInstance.block_name);
    }
}
