using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class findMatches : MonoBehaviour
{
    private Board board;

    public List<GameObject> currentMatches = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<Board>();

    }

    private List<GameObject> isAdjacentBomb(Dot dot1, Dot dot2, Dot dot3)
    {
        List<GameObject> currentDots = new List<GameObject>();
        if (dot1.isAdjacentBomb)
        {
            currentMatches.Union(getAdjacentPieces(dot1.column,dot1.row));
        }
        if (dot2.isAdjacentBomb)
        {
            currentMatches.Union(getAdjacentPieces(dot2.column,dot2.row));
        }
        if (dot3.isAdjacentBomb)
        {
            currentMatches.Union(getAdjacentPieces(dot3.column,dot3.row));
        }
        return currentDots;
    }

    private List<GameObject> isRowBomb(Dot dot1,Dot dot2, Dot dot3)
    {
        List<GameObject> currentDots = new List<GameObject>();
        if (dot1.isRowBomb)
        {
            currentMatches.Union(getRowPiecs(dot1.row));
            board.bombRow(dot1.row);
        }
        if (dot2.isRowBomb)
        {
            currentMatches.Union(getRowPiecs(dot2.row));
            board.bombRow(dot2.row);

        }
        if (dot3.isRowBomb)
        {
            currentMatches.Union(getRowPiecs(dot3.row));
            board.bombRow(dot3.row);

        }
        return currentDots;
    }

    private List<GameObject> isColumnBomb(Dot dot1, Dot dot2, Dot dot3)
    {
        List<GameObject> currentDots = new List<GameObject>();
        if (dot1.isColumnbomb)
        {
            currentMatches.Union(getColumnPiecs(dot1.column));
            board.bombColumn(dot1.column);
        }
        if (dot2.isColumnbomb)
        {
            currentMatches.Union(getColumnPiecs(dot2.column));
            board.bombColumn(dot2.column);

        }
        if (dot3.isColumnbomb)
        {
            currentMatches.Union(getColumnPiecs(dot3.column));
            board.bombColumn(dot3.column);

        }
        return currentDots;
    }

    private void addToListAndMatch(GameObject dot)
    {
        if (!currentMatches.Contains(dot)) 
            currentMatches.Add(dot);
        dot.GetComponent<Dot>().isMatched = true;
    }

    private void getNearByPieces(GameObject dot1, GameObject dot2, GameObject dot3)
    {
        addToListAndMatch(dot1);
        addToListAndMatch(dot2);
        addToListAndMatch(dot3);
    }

    public void findAllMatches()
    {
        StartCoroutine(findAllMatchesCO());
    }

    private IEnumerator findAllMatchesCO()
    {
        yield return null;
        for (int i = 0; i < board.width; i++)
        {
            for (int j = 0; j < board.height; j++)
            {
                GameObject currentDot = board.allDots[i, j];     
                if (currentDot != null)
                {
                    Dot currentDotScript = currentDot.GetComponent<Dot>();

                    if (i > 0 && i < board.width - 1)
                    {
                        GameObject leftDot = board.allDots[i - 1, j];
                        GameObject rightDot = board.allDots[i + 1, j];

                        if (leftDot != null && rightDot != null)
                        {
                            Dot leftDotScript = leftDot.GetComponent<Dot>();
                            Dot rightDotScript = rightDot.GetComponent<Dot>();

                            if (leftDot.tag == currentDot.tag && rightDot.tag == currentDot.tag)
                            {
                                currentMatches.Union(isRowBomb(leftDotScript, currentDotScript, rightDotScript));

                                currentMatches.Union(isColumnBomb(leftDotScript, currentDotScript, rightDotScript));

                                currentMatches.Union(isAdjacentBomb(leftDotScript, currentDotScript, rightDotScript));

                                getNearByPieces(leftDot, currentDot, rightDot);
                            }
                        }
                    }
                    if (j > 0 && j < board.height - 1)
                    {
                        GameObject downDot = board.allDots[i, j - 1];
                        GameObject UpDot = board.allDots[i, j + 1];

                        if (downDot != null && UpDot != null)
                        {
                            Dot downDotScript = downDot.GetComponent<Dot>();
                            Dot upDotScript = downDot.GetComponent<Dot>();

                            if (downDot.tag == currentDot.tag && UpDot.tag == currentDot.tag)
                            {
                                  currentMatches.Union(isColumnBomb(upDotScript, currentDotScript, downDotScript));

                                  currentMatches.Union(isRowBomb(upDotScript, currentDotScript, downDotScript));

                                currentMatches.Union(isAdjacentBomb(upDotScript, currentDotScript, downDotScript));
                                getNearByPieces(UpDot, currentDot, downDot);
                            }
                        }
                    }
                }
            }
        }
        yield return null;
    }

    List<GameObject> getColumnPiecs(int column)
    {
        List<GameObject> dots = new List<GameObject>();
        for (int i = 0; i < board.height; i++)
        {
            if (board.allDots[column, i] != null)
            {
                Dot dot = board.allDots[column, i].GetComponent<Dot>();
                if (dot.isRowBomb)
                {
                    dots.Union(getRowPiecs(i)).ToList();
                }

                dots.Add(board.allDots[column, i]);
                dot.isMatched = true;
            }
        }
        return dots;
    }

    List<GameObject> getRowPiecs(int row)
    {
        List<GameObject> dots = new List<GameObject>();
        for (int i = 0; i < board.width; i++)
        {
            if (board.allDots[i, row] != null)
            {
                Dot dot = board.allDots[i, row].GetComponent<Dot>();
                if (dot.isColorBomb)
                {
                    dots.Union(getColumnPiecs(i)).ToList();
                }

                dots.Add(board.allDots[i, row]);
                dot.isMatched = true;
            }
        }
        return dots;
    }

    public void checkBombs(MatchType matchType)
    {
        //did the player move something?
        if (board.currentDot != null)
        {
            // is the piece they move matched?
            if (board.currentDot.isMatched && board.currentDot.tag == matchType.color)
            {
                // make it unmatched
                board.currentDot.isMatched = false;
                // decide what kind of bomb to make

                if ((board.currentDot.swipeAngle > -45 && board.currentDot.swipeAngle <= 45)
                    || (board.currentDot.swipeAngle < -135 || board.currentDot.swipeAngle >= 135))
                {
                    board.currentDot.makeRowbomb();
                }
                else
                {
                    board.currentDot.makeColumnBomb();
                }
            }
            // is the other piece matched?
            else if (board.currentDot.otherDot != null)
            {
                Dot otherDot = board.currentDot.otherDot.GetComponent<Dot>();
                // is the other dot matched?
                if (otherDot.isMatched && otherDot.tag == matchType.color)
                {
                    // make it unmatched
                    otherDot.isMatched = false;

              if ((board.currentDot.swipeAngle > -45 && board.currentDot.swipeAngle <= 45)
             || (board.currentDot.swipeAngle < -135 || board.currentDot.swipeAngle >= 135))
                    {
                        otherDot.makeRowbomb();
                    }
                    else
                    {
                        otherDot.makeColumnBomb();
                    }
                }
            }
        }
    }

    public void matchPiecesOfColor(string color)
    {
        for(int i = 0; i < board.width; i++)
        {
            for(int j = 0; j < board.height; j++)
            {
                // check if the piece exist
                if (board.allDots[i, j] != null)
                {
                    //check the tag on that dot
                    if (board.allDots[i, j].tag == color)
                    {
                        // set that dot to be matched
                        board.allDots[i, j].GetComponent<Dot>().isMatched = true;
                    }
                }
            }
        }
    }

    List<GameObject> getAdjacentPieces(int column , int row)
    {
        List<GameObject> dots = new List<GameObject>();

        for(int i=column-1; i <= column + 1; i++)
        {
            for(int j = row-1; j <= row + 1; j++)
            {
                // check if the piece is inside the board/ available 
                if(i>=0&&i <board.width && j>=0 && j < board.height)
                {
                    if (board.allDots[i, j] != null)
                    {
                        dots.Add(board.allDots[i, j]);
                        board.allDots[i, j].GetComponent<Dot>().isMatched = true;
                    }
                }
            }
        }
        return dots;
    }
}
