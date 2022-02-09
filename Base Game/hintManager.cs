using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hintManager : MonoBehaviour
{
    private Board board;
    private float hintDelayTimer;


    public float hintDelay;
    public GameObject hintParticle;
    public GameObject currentHint;
    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<Board>();
        hintDelayTimer = hintDelay;
    }

    // Update is called once per frame
    void Update()
    {
        hintDelayTimer -= Time.deltaTime;
        if (hintDelayTimer <= 0&& currentHint== null)
        {
            markHint();
            hintDelayTimer = hintDelay;
        }
    }

    // find all possible matches
    List<GameObject> findAllMatches()
    {
        List<GameObject> possibleMoves = new List<GameObject>();
        for (int i = 0; i < board.width; i++)
        {
            for (int j = 0; j < board.height; j++)
            {
                if (board.allDots[i, j] != null)
                {
                    // check matches to the right
                    if (i < board.width - 1)
                    {
                        if (board.switchAndScheck(i, j, Vector2.right))
                        {
                            possibleMoves.Add(board.allDots[i, j]);
                        }
                    }

                    //check up or matches
                    if (j < board.height - 1)
                    {
                        if (board.switchAndScheck(i, j, Vector2.up))
                        {
                            possibleMoves.Add(board.allDots[i, j]);

                        }
                    }
                }
            }
        }
        return possibleMoves;
    }

    // pick a random gameobject of these matches 
    GameObject pickMatch()
    {
        List<GameObject> possibleMoves = new List<GameObject>();
        possibleMoves = findAllMatches();
        if (possibleMoves.Count > 0)
        {
            int pieceToUse = Random.Range(0, possibleMoves.Count);
            return possibleMoves[pieceToUse];
        }
        return null;
    }

    // create the hint 
    void markHint()
    {
        GameObject move = pickMatch();
        if(move != null)
        {
            currentHint = Instantiate(hintParticle, move.transform.position, Quaternion.identity);
        }
    }

    // destory the hint
    public void destroyHint()
    {
        if (currentHint != null)
        {
            Destroy(currentHint);
            currentHint = null;
            hintDelayTimer = hintDelay;
        }
    }
}
