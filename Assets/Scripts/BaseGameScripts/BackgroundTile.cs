using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundTile : MonoBehaviour
{

    public int hitPoints;
    SpriteRenderer spriteRenderer;
    GoalManager goalManager;

    private void Start()
    {
        goalManager = FindObjectOfType<GoalManager>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if(hitPoints <= 0)
        {
            if(goalManager != null)
            {
                goalManager.CompareGoal(this.gameObject.tag);
                goalManager.UpdateGoals();
            }
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
