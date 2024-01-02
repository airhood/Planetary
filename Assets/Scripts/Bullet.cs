using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public Vector2 origin;

    public float attackDamage;
    public float attackRange;

    // Start is called before the first frame update
    void Start()
    {
        Invoke("DestroyBullet", 3);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void DestroyBullet()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        print($"trigger enter: {collision.gameObject.name}");
        if (collision.gameObject.CompareTag("mob"))
        {
            float f_damage = Mathf.Lerp(attackDamage, 0, Vector2.Distance(transform.position + new Vector3(0, 1f), collision.ClosestPoint(transform.position)) / (attackRange + 0.5f));
            if (f_damage < 1f)
            {
                f_damage = 1f;
            }
            int damage = Mathf.FloorToInt(f_damage);
            print($"damage:{damage}");
            collision.gameObject.GetComponent<Mob>().health -= damage;
        }
        Destroy(gameObject);
    }
}
