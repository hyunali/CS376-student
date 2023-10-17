using System;
using UnityEngine;

public class Bomb : MonoBehaviour {
    public float ThresholdImpulse = 5;
    public GameObject ExplosionPrefab;

    public BoxCollider2D boxCollider;
    public SpriteRenderer spriteRenderer;
    public PointEffector2D pointEffector;
    public Bomb bomb;
    

    private void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        pointEffector = GetComponent<PointEffector2D>();
        bomb = GetComponent<Bomb>();
    }

    void Destruct()
    {
        Destroy(gameObject);
    }

    void Boom()
    {
        pointEffector.enabled = true;
        spriteRenderer.enabled = false;
        Instantiate(ExplosionPrefab, transform.position, Quaternion.identity, transform.parent);
        Invoke(nameof(Destruct), 0.1f);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        foreach (ContactPoint2D hit in collision.contacts)
        {
            if (hit.normalImpulse > ThresholdImpulse)
            {
                Boom();
            }
        }
    }
}


