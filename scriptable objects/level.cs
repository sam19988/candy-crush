using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="world",menuName ="Level")]
public class level : ScriptableObject
{
    [Header("Board dimentions")]
    public int width;
    public int height;

    [Header("Starting Tiles")]
    public TileType [] boardLayout;

    [Header("Available Dots")]
    public GameObject[] dots;

    [Header("Score Goals")]
    public int[] scoreGoals;

    [Header("End Game Req")]
    public endGameRequirments EGR;
    public BlankGoal[] levelGoals;
}
