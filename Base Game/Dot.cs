using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dot : MonoBehaviour
{
    [Header("Board variables")]
    public int column;
    public int row;
    public int previousColumn;
    public int previousRow;
    public int targetX;
    public int targetY;
    public bool isMatched = false;

    public GameObject otherDot;
    private Board board;
    private Vector2 firstTouchPositon = Vector2.zero;
    private Vector2 finalTouchPositon = Vector2.zero;
    private Vector2 tempPosition;
    private findMatches findmatches;
    private hintManager hintmanger;
    private EndGameManager EGM;
    private Animator anim;
    private float shineDelay;
    private float shineDelaySeconds;

    [Header("Swipe Stuff")]
    public float swipeAngle = 0;
    public float swipeResist = 1f;


    [Header("PowerUp stuff")]
    public bool isColorBomb;
    public bool isRowBomb;
    public bool isColumnbomb;
    public bool isAdjacentBomb;
    public GameObject adjacentBomb;
    public GameObject rowArrow;
    public GameObject columnArrow;
    public GameObject colorBomb;

    // Start is called before the first frame update
    void Start()
    {
        isColumnbomb = false;
        isRowBomb = false;
        isColorBomb = false;
        isAdjacentBomb = false;
        // board = FindObjectOfType<Board>();
        board = GameObject.FindGameObjectWithTag("Board").GetComponent<Board>();
        findmatches = FindObjectOfType<findMatches>();
        hintmanger = FindObjectOfType<hintManager>();
        EGM = FindObjectOfType<EndGameManager>();
        anim = GetComponent<Animator>();
        shineDelay = Random.Range(3f, 6f);
        shineDelaySeconds = shineDelay;
    }


    // for testing purpose
    private void OnMouseOver()
    {

        if (Input.GetMouseButtonDown(1))
        {
            isAdjacentBomb = true;
            GameObject adjacent = Instantiate(adjacentBomb, transform.position, Quaternion.identity);
            adjacent.transform.parent = this.transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        shineDelaySeconds -= Time.deltaTime;
        if (shineDelaySeconds <= 0)
        {
            shineDelaySeconds = shineDelay;
            StartCoroutine(startShineCo());
        }
        targetX = column;
        targetY = row;
        if (Mathf.Abs(targetX - transform.position.x) > 0.1)
        {
            // move to the target horizontally
            tempPosition = new Vector2(targetX, transform.position.y);
            transform.position = Vector2.Lerp(transform.position, tempPosition, 0.6f);

            if (board.allDots[column, row] != this.gameObject)
            {
                board.allDots[column, row] = this.gameObject;
                findmatches.findAllMatches();
            }
        }
        else
        {
            // directly set position
            tempPosition = new Vector2(targetX, transform.position.y);
            transform.position = tempPosition;
        }


        if (Mathf.Abs(targetY - transform.position.y) > 0.1)
        {
            // move to the target vertically
            tempPosition = new Vector2(transform.position.x, targetY);
            transform.position = Vector2.Lerp(transform.position, tempPosition, 0.6f);
            if (board.allDots[column, row] != this.gameObject)
            {
                board.allDots[column, row] = this.gameObject;
                findmatches.findAllMatches();
            }
        }
        else
        {
            // directly set position
            tempPosition = new Vector2(transform.position.x, targetY);
            transform.position = tempPosition;
        }
    }

    public IEnumerator checkMoveCo()
    {
        if (isColorBomb)
        {
            // this piece is color bomb , and the other piece is the color to be destroyed
            findmatches.matchPiecesOfColor(otherDot.tag);
            isMatched = true;
        } else if (otherDot.GetComponent<Dot>().isColorBomb)
        {
            // the other piece is color bomb , and this piece is the color to be destroyed
            findmatches.matchPiecesOfColor(this.gameObject.tag);
            otherDot.GetComponent<Dot>().isMatched = true;
        }
        yield return new WaitForSeconds(0.5f);
        if (otherDot != null)
        {
            if (!isMatched && !otherDot.GetComponent<Dot>().isMatched)
            {
                otherDot.GetComponent<Dot>().row = row;
                otherDot.GetComponent<Dot>().column = column;
                row = previousRow;
                column = previousColumn;
                yield return new WaitForSeconds(0.5f);
                board.currentDot = null;
                board.currentState = gameState.move;
            }
            else
            {
                if (EGM != null)
                {
                    if(EGM.requirments.GameType == gameType.Moves)
                    {
                        EGM.decreaseCounter();
                    }
                }
                board.destroyMatches();
            }
        }
    }

    private void OnMouseDown()
    {
        if(anim != null)
        {
            anim.SetBool("Touched", true);
        }

        // destroy the hint
        if (hintmanger != null)
            hintmanger.destroyHint();


        if (board.currentState == gameState.move)
        {
            firstTouchPositon = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }

    }

    private void OnMouseUp()
    {
        anim.SetBool("Touched", false);
        if (board.currentState == gameState.move)
        {
            finalTouchPositon = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            calculateAngle();
        }

    }

    void calculateAngle()
    {
        if (Mathf.Abs(finalTouchPositon.y - firstTouchPositon.y) > swipeResist ||
          Mathf.Abs(finalTouchPositon.x - firstTouchPositon.x) > swipeResist) {
            board.currentState = gameState.wait;
            swipeAngle = Mathf.Atan2(finalTouchPositon.y - firstTouchPositon.y, finalTouchPositon.x - firstTouchPositon.x) * 180 / Mathf.PI;
            movePieces();
            board.currentDot = this;
        }
        else
        {
            board.currentState = gameState.move;
        }

    }

    void movePiecesSupp(Vector2 direction)
    {
        otherDot = board.allDots[column + (int)direction.x, row + (int)direction.y];
        previousColumn = column;
        previousRow = row;
        if(board.lockTiles[column,row] == null && board.lockTiles[column+(int)direction.x,
            row+(int)direction.y] == null)
        {
            if (otherDot != null)
            {
                otherDot.GetComponent<Dot>().column += -1 * (int)direction.x;
                otherDot.GetComponent<Dot>().row += -1 * (int)direction.y;
                this.column += (int)direction.x;
                this.row += (int)direction.y;
                StartCoroutine(checkMoveCo());
            }
            else
            {
                board.currentState = gameState.move;
            }
        }
        else
        {
            board.currentState = gameState.move;
        }
    }

    void movePieces()
    {
        if(swipeAngle> -45 && swipeAngle <= 45&& column< board.width-1)
        {        
            // right swipe
            movePiecesSupp(Vector2.right);
        }
        else if (swipeAngle > 45 && swipeAngle <= 135 && row <board.height-1)
        {
            // Up swipe
            movePiecesSupp(Vector2.up);
        }
        else if ((swipeAngle > 135 || swipeAngle <= -135) && column >0)
        {
            // left swipe
            movePiecesSupp(Vector2.left);
        }
        else if (swipeAngle < -45 && swipeAngle >= -135 && row >0)
        {
            // down swipe
            movePiecesSupp(Vector2.down);
        }else
        board.currentState = gameState.move;
    }


    public void makeRowbomb()
    {
        if(!isColorBomb && !isColumnbomb && !isAdjacentBomb)
        {
            isRowBomb = true;
            GameObject arrow = Instantiate(rowArrow, transform.position, Quaternion.identity);
            arrow.transform.parent = this.transform;
        }
    }

    public void makeColumnBomb()
    {
        if(!isRowBomb && !isColorBomb && !isAdjacentBomb)
        {
            isColumnbomb = true;
            GameObject arrow = Instantiate(columnArrow, transform.position, Quaternion.identity);
            arrow.transform.parent = this.transform;
        }
    }

    public void makeColorbomb()
    {
        if(!isColumnbomb && !isRowBomb && !isAdjacentBomb)
        {
            isColorBomb = true;
            GameObject colour = Instantiate(colorBomb, transform.position, Quaternion.identity);
            colour.transform.parent = this.transform;
            this.gameObject.tag = "color";
        }
    }

    public void makeAdjacentBomb()
    {
        if(!isColorBomb && !isColumnbomb && !isRowBomb)
        {
            isAdjacentBomb = true;
            GameObject adjacent = Instantiate(adjacentBomb, transform.position, Quaternion.identity);
            adjacent.transform.parent = this.transform;
        }
    }

    public IEnumerator startShineCo()
    {
        anim.SetBool("Shine", true);
        yield return null; // it will make the code skip one frame and then continue from here 
        anim.SetBool("Shine", false);
    }

    public void popAnimation()
    {
        anim.SetBool("Popped", true);
    }
}
