using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraColliderAutoTiling : MonoBehaviour
{
    public new BoxCollider2D collider;

    public void Till()
    {
        float cameraSize = Camera.main.orthographicSize;
        collider.size = new Vector2(cameraSize * 2 * 1.77f, cameraSize * 2);
    }
}
