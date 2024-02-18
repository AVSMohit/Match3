using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundTile : MonoBehaviour
{

    public int hitPoints;
    SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if(hitPoints <= 0)
        {
            Destroy(this.gameObject);
        }
    }
    public void TakeDamage(int damage)
    {
        hitPoints -= damage;
        MakeTileLighter();
    }

    void MakeTileLighter()
    {
        Color color = spriteRenderer.color;

        float newAlpha = color.a * .5f;

        spriteRenderer.color = new Color(color.r, color.g, color.b, newAlpha);
    }

}
