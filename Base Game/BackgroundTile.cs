using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundTile : MonoBehaviour
{
    public int hitPoints;
    private SpriteRenderer SR;
    private GoalManager goalManager;

    public void takeDamage(int damage)
    {
        hitPoints -= damage;
        makeLighter();
    }
    private void Update()
    {
        if (hitPoints <= 0)
        {
            if (goalManager != null)
            {
                goalManager.compareToGoal(this.gameObject.tag);
                goalManager.updateGoals();
            }
            Destroy(this.gameObject);
        }
    }
    private void Start()
    {
        goalManager = FindObjectOfType<GoalManager>();
        SR = GetComponent<SpriteRenderer>();
    }

    private void makeLighter()
    {
        Color color = SR.color;
        float newAlpha = color.a * 0.5f;
        SR.color = new Color(color.r, color.g, color.b, newAlpha);
    }
}
