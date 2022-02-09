using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public enum gameState
{
    wait,move,win,lose,pause
}

public enum TileKind
{
    Breakable, Blank , Normal ,Lock ,concrete , Slime
}
[System.Serializable]
public class TileType
{
    public int x;
    public int y;
    public TileKind tileKind;
}

[System.Serializable]
public class MatchType
{
    public int type;
    public string color;
}


public class Board : MonoBehaviour
{
    // the board dimentions 
    [Header("Board Dimentions")]
    public int width;
    public int height;
    public int slideInOffSet;
    
    [Header("Board Prefabs")]
    public GameObject tilePrefab;
    public GameObject[] Dots;
    public GameObject[,] allDots;
    public Dot currentDot;
    public GameObject destroyEffect;
    public GameObject lockTilePrefab;
    public GameObject concreteTilePrefab;
    public GameObject slimeTilePrefab;

    [Header("Layout")]
    public TileType[] boardLayout;
    public GameObject breakabletilePref;
    public int basePieceValue = 5;
    private int streakValue = 1;
    public gameState currentState = gameState.move;
    public float refillDelay = 0.5f;
    public int[] scoareGoals;
    public BackgroundTile[,] lockTiles;
    public BackgroundTile[,] concreteTiles;
    public BackgroundTile[,] slimeTiles; 

    [Header("Scriptableobjects items")]
    public world World;
    public int level;

    public MatchType matchtype;
    private bool[,] blankSpaces;
    private BackgroundTile[,] breakableTiles;
    private findMatches findmatches;
    private ScoreManager ScoreManager;
    private soundManager soundManger;
    private GoalManager goalManager;
    private bool makeSlime = true;

    private void Awake()
    {
        if(PlayerPrefs.HasKey("Current Level"))
        {
            level = PlayerPrefs.GetInt("Current Level");
        }

        if(World != null)
        {
            if(level <World.levels.Length && level >= 0)
            {
                if (World.levels[level] != null)
                {
                    width = World.levels[level].width;
                    height = World.levels[level].height;
                    Dots = World.levels[level].dots;
                    scoareGoals = World.levels[level].scoreGoals;
                    boardLayout = World.levels[level].boardLayout;
                }
            }
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        goalManager = FindObjectOfType<GoalManager>();
        soundManger = FindObjectOfType<soundManager>();
        ScoreManager = FindObjectOfType<ScoreManager>();
        blankSpaces = new bool[width, height];
        breakableTiles = new BackgroundTile[width, height];
        lockTiles = new BackgroundTile[width, height];
        concreteTiles = new BackgroundTile[width, height];
        slimeTiles = new BackgroundTile[width, height];
        allDots = new GameObject[width, height];
        findmatches = FindObjectOfType<findMatches>();
        setUp();
        currentState = gameState.pause;
    }

    public void generateBlankSpaces()
    {
        for(int i=0; i< boardLayout.Length; i++)
        {
            if(boardLayout[i].tileKind== TileKind.Blank)
            {
                blankSpaces[boardLayout[i].x, boardLayout[i].y] = true;
            }
        }
    }

    public void generateBreakableTiles()
    {
        // look at all tiles in the layout
        for(int i = 0; i < boardLayout.Length; i++)
        {
            // if a tile is a jelly tile
            if(boardLayout[i].tileKind== TileKind.Breakable)
            {
                // create a jelly tile at this position
                Vector2 tempPosition = new Vector2(boardLayout[i].x, boardLayout[i].y);
                GameObject tile = Instantiate(breakabletilePref, tempPosition, Quaternion.identity);
                breakableTiles[boardLayout[i].x, boardLayout[i].y] = tile.GetComponent<BackgroundTile>();
            }
        }
    }

    private void generateLockTiles()
    {
        // look at all tiles in the layout
        for (int i = 0; i < boardLayout.Length; i++)
        {
            // if a tile is a lock tile
            if (boardLayout[i].tileKind == TileKind.Lock)
            {
                // create a Lock tile at this position
                Vector2 tempPosition = new Vector2(boardLayout[i].x, boardLayout[i].y);
                GameObject tile = Instantiate(lockTilePrefab, tempPosition, Quaternion.identity);
                lockTiles[boardLayout[i].x, boardLayout[i].y] = tile.GetComponent<BackgroundTile>();
            }
        }
    }

    private void generateConcreteTiles()
    {
        // look at all tiles in the layout
        for (int i = 0; i < boardLayout.Length; i++)
        {
            // if a tile is a concrete tile
            if (boardLayout[i].tileKind == TileKind.concrete)
            {
                // create a jelly tile at this position
                Vector2 tempPosition = new Vector2(boardLayout[i].x, boardLayout[i].y);
                GameObject tile = Instantiate(concreteTilePrefab, tempPosition, Quaternion.identity);
                concreteTiles[boardLayout[i].x, boardLayout[i].y] = tile.GetComponent<BackgroundTile>();
            }
        }
    }

    private void generateSlimeTiles()
    {
        // look at all tiles in the layout
        for (int i = 0; i < boardLayout.Length; i++)
        {
            // if a tile is a slime tile
            if (boardLayout[i].tileKind == TileKind.Slime)
            {
                // create a slime tile at this position
                Vector2 tempPosition = new Vector2(boardLayout[i].x, boardLayout[i].y);
                GameObject tile = Instantiate(slimeTilePrefab, tempPosition, Quaternion.identity);
                slimeTiles[boardLayout[i].x, boardLayout[i].y] = tile.GetComponent<BackgroundTile>();
            }
        }
    }

    private void setUp()
    {
        generateSlimeTiles();
        generateConcreteTiles();
        generateLockTiles();
        generateBlankSpaces();
        generateBreakableTiles();

        for(int i=0;i< width; i++)
        {
            for(int j=0;j < height; j++)
            {
                // making the background for each tile in the right position
                // offset of the initial position of the creation
                if(!blankSpaces[i,j] && !concreteTiles[i,j]&& !slimeTiles[i,j])
                {
                    Vector2 tempPosition = new Vector2(i, j + slideInOffSet);
                    Vector2 tilePosition = new Vector2(i, j);
                    GameObject backgroundTile = Instantiate(tilePrefab, tilePosition, Quaternion.identity);
                    backgroundTile.transform.parent = this.transform;
                    backgroundTile.transform.name = "( " + i + ", " + j + " )";

                    // making the dots themsleves 
                    int dotToUse = Random.Range(0, Dots.Length);

                    int maxIteration = 0;
                    while (matchesAt(i, j, Dots[dotToUse]) && maxIteration < 100)
                    {
                        dotToUse = Random.Range(0, Dots.Length);
                        maxIteration++;
                    }
                    maxIteration = 0;

                    GameObject dot = Instantiate(Dots[dotToUse], tempPosition, Quaternion.identity);
                    // to make the dots slide down 
                    dot.GetComponent<Dot>().row = j;
                    dot.GetComponent<Dot>().column = i;

                    dot.transform.parent = this.transform;
                    dot.transform.name = "( " + i + ", " + j + " )" + " Dot";
                    allDots[i, j] = dot;
                }    
            }
        }
    }

    // to make no auto-matches at the beginning of generating the board 
    private bool matchesAt(int column, int row, GameObject piece)
    {
        if(column > 1 && row > 1)
        {
            if(allDots[column-1,row]!=null && allDots[column - 2,row] != null)
            {
                if (allDots[column - 1, row].tag == piece.tag && allDots[column - 2, row].tag == piece.tag)  // check one and tow to the down dot
                {
                    return true;
                }
            }
           if(allDots[column,row-1]!=null&& allDots[column, row - 2] != null)
            {
                if (allDots[column, row - 1].tag == piece.tag && allDots[column, row - 2].tag == piece.tag)  // check one and tow to the left dot
                {
                    return true;
                }
            }
           
        }else if(column<= 1 || row<= 1)
        {
            
                if (row > 1)  // first condition is met
                {
                if (allDots[column, row - 1] != null && allDots[column, row - 2] != null)
                {
                    if (allDots[column, row - 1].tag == piece.tag && allDots[column, row - 2].tag == piece.tag)
                        return true;
                }
                }
            
            
                if (column > 1)  // second condition is met
                {
                if (allDots[column - 1, row] != null && allDots[column - 2, row] != null)
                {
                    if (allDots[column - 1, row].tag == piece.tag && allDots[column - 2, row].tag == piece.tag)
                        return true;
                }    
                }
        }
        return false;
    }

    private MatchType columnOrRow()
    {
        // make a copy of the currentMatches
        List<GameObject> matchCopy = findmatches.currentMatches as List<GameObject>;

        matchtype.type = 0;
        matchtype.color = "";
        // cycle through all of matchCopy and decide if a bomb needs to be made
        for(int i=0; i<matchCopy.Count; i++)
        {
            // store the current piece
            Dot thisDot = matchCopy[i].GetComponent<Dot>();
            string color = matchCopy[i].tag;
            int column = thisDot.column;
            int row = thisDot.row;
            int columnMatchs = 0;
            int rowMatchs = 0;

            // cycle through the rest of the pieces and compare to thisDot
            for(int j = 0; j < matchCopy.Count; j++)
            {
                // store the next piece
                Dot nextDot = matchCopy[j].GetComponent<Dot>();
                if(nextDot== thisDot)
                {
                    // in case we get the current dot (we eterate throught the same list )
                    continue;
                }
                if(nextDot.column == thisDot.column && nextDot.tag == color)
                {
                    columnMatchs++;
                }
                if (nextDot.row == thisDot.row && thisDot.tag == color)
                {
                    rowMatchs++;
                }
            }

            // return 3 if row or column bomb
            // return 2 if adjacent bomb
            // return 1 if color bomb 
            if(columnMatchs == 4 || rowMatchs == 4)
            {
                matchtype.type = 1;
                matchtype.color = color;
                return matchtype;
            }
           else if(columnMatchs== 2 && rowMatchs == 2)
            {
                matchtype.type = 2;
                matchtype.color = color;
                return matchtype;
            }
           else if(columnMatchs ==3|| rowMatchs == 3)
            {
                matchtype.type = 3;
                matchtype.color = color;
                return matchtype;
            }
        }
        matchtype.type = 0;
        matchtype.color = "";
        return matchtype;
    }

    private void chekcToMakeBombs()
    {
        // how many objects are in findmatches currentmatches
        if (findmatches.currentMatches.Count > 3)
        {
            MatchType typeOfMatch = columnOrRow();

            if (typeOfMatch.type == 1)
            {
                // make a color bomb
                // is the current dot matched ?
                if (currentDot != null && currentDot.isMatched && currentDot.tag == typeOfMatch.color)
                {
                    currentDot.isMatched = false;
                    currentDot.makeColorbomb();
                }
                else
                {
                    if (currentDot.otherDot != null)
                    {
                        Dot otherDot = currentDot.otherDot.GetComponent<Dot>();
                        if (otherDot.isMatched && otherDot.tag == typeOfMatch.color)
                        {
                            otherDot.isMatched = false;
                            otherDot.makeColorbomb();
                        }
                    }
                }
            }
            else if (typeOfMatch.type == 2)
            {
                // make an adjacent bomb
                // is the current dot matched ?
                if (currentDot != null && currentDot.isMatched && currentDot.tag == typeOfMatch.color)
                {
                    currentDot.isMatched = false;
                    currentDot.makeAdjacentBomb();
                }
                else if (currentDot.otherDot != null)
                {
                    Dot otherDot = currentDot.otherDot.GetComponent<Dot>();
                    if (otherDot.isMatched && otherDot.tag == typeOfMatch.color)
                    {
                            otherDot.isMatched = false;
                            otherDot.makeAdjacentBomb();    
                    }
                }
            }
            else if (typeOfMatch.type == 3)
            {
                findmatches.checkBombs(typeOfMatch);
            }
        }

    } 

    public void bombRow(int row)
    {
        for(int i = 0; i < width; i++)
        {
            if(concreteTiles[i,row] != null)
            {
                concreteTiles[i , row].takeDamage(1);
                if (concreteTiles[i , row].hitPoints <= 0)
                    concreteTiles[i , row] = null;
            }
        }
    }

    public void bombColumn(int column)
    {
        for (int i = 0; i < width; i++)
        {
            if (concreteTiles[column, i] != null)
            {
                concreteTiles[column, i].takeDamage(1);
                if (concreteTiles[column, i].hitPoints <= 0)
                    concreteTiles[column, i] = null;
            }
        }
    }

    private void destroyingMatchesAt(int column , int row)
    {
        if (allDots[column, row].GetComponent<Dot>().isMatched)
        {
            // does a tile need to break ?
            if (breakableTiles[column, row] != null)
            {
                // if it does , give 1 damage
                breakableTiles[column, row].takeDamage(1);
                if (breakableTiles[column, row].hitPoints <= 0)
                    breakableTiles[column, row] = null;
            }

            // is it a lock tile ?
            if (lockTiles[column, row] != null)
            {
                // if it does , give 1 damage
                lockTiles[column, row].takeDamage(1);
                if (lockTiles[column, row].hitPoints <= 0)
                    lockTiles[column, row] = null;
            }

            // destroy concrete tiles
            damageConcrete(column,row);

            // destroy slime tiles
            damageSlime(column, row);

            // check the goal manager 
            if (goalManager != null)
            {
                goalManager.compareToGoal(allDots[column, row].tag);
                goalManager.updateGoals();
            }

            // check the sound manager 
            if(soundManger != null)
            {
                soundManger.playRandomDestroySound();
            }
           GameObject particle= Instantiate(destroyEffect, allDots[column, row].transform.position, Quaternion.identity);
            Destroy(particle, 0.5f);
            allDots[column, row].GetComponent<Dot>().popAnimation();
            Destroy(allDots[column, row],0.5f);
            ScoreManager.increaseScore(basePieceValue * streakValue);
            allDots[column, row] = null;
        }
    }

    public void destroyMatches()
    {
        // how many pieces are matched ?
        if (findmatches.currentMatches.Count >= 4)
        {
            chekcToMakeBombs();
        }
        findmatches.currentMatches.Clear();

        for (int i = 0; i < width; i++)
        {
            for(int j = 0; j < height; j++)
            {
                if (allDots[i, j] != null)
                {
                    destroyingMatchesAt(i, j);
                }
            }
        }
        StartCoroutine(shiftRowWithBlankSpaces());
    }

    private void damageConcrete(int column , int row)
    {
        // destroy concrete to the left
        if (column > 0)
        {
            if (concreteTiles[column - 1, row] !=null)
            {
                concreteTiles[column -1, row].takeDamage(1);
                if (concreteTiles[column-1, row].hitPoints <= 0)
                    concreteTiles[column-1, row] = null;
            }
        }

        // destroy concrete to the right
        if (column < width-1)
        {
            if (concreteTiles[column +1, row] != null)
            {
                concreteTiles[column+1, row].takeDamage(1);
                if (concreteTiles[column +1, row].hitPoints <= 0)
                    concreteTiles[column +1, row] = null;
            }
        }

        // destroy concrete below
        if (row > 0)
        {
            if (concreteTiles[column , row-1] != null)
            {
                concreteTiles[column, row-1].takeDamage(1);
                if (concreteTiles[column , row-1].hitPoints <= 0)
                    concreteTiles[column , row-1] = null;
            }
        }

        // destroy concrete above
        if (row < height-1)
        {
            if (concreteTiles[column , row+1] != null)
            {
                concreteTiles[column, row+1].takeDamage(1);
                if (concreteTiles[column , row+1].hitPoints <= 0)
                    concreteTiles[column , row+1] = null;
            }
        }
    }

    private void damageSlime(int column, int row)
    {
        // destroy slime to the left
        if (column > 0)
        {
            if (slimeTiles[column - 1, row] != null)
            {
                slimeTiles[column - 1, row].takeDamage(1);
                if (slimeTiles[column - 1, row].hitPoints <= 0)
                {
                    slimeTiles[column - 1, row] = null;
                }
                makeSlime = false;
            }
        }

        // destroy slime to the right
        if (column < width - 1)
        {
            if (slimeTiles[column + 1, row] != null)
            {
                slimeTiles[column + 1, row].takeDamage(1);
                if (slimeTiles[column + 1, row].hitPoints <= 0)
                {
                    slimeTiles[column + 1, row] = null;
                }
                makeSlime = false;
            }
        }

        // destroy slime below
        if (row > 0)
        {
            if (slimeTiles[column, row - 1] != null)
            {
                slimeTiles[column, row - 1].takeDamage(1);
                if (slimeTiles[column, row - 1].hitPoints <= 0)
                {
                    slimeTiles[column, row - 1] = null;
                }
                makeSlime = false;
            }
        }

        // destroy slime above
        if (row < height - 1)
        {
            if (slimeTiles[column, row + 1] != null)
            {
                slimeTiles[column, row + 1].takeDamage(1);
                if (slimeTiles[column, row + 1].hitPoints <= 0)
                {
                    slimeTiles[column, row + 1] = null;

                }
                makeSlime = false;
            }
        }
    }

    private IEnumerator shiftRowWithBlankSpaces()
    {
        for(int i = 0; i < width; i++)
        {
            for (int j=0; j < height; j++)
            {
                // if the current spot isn't blank and is empty
                if(! blankSpaces[i,j] && allDots[i, j] == null && !concreteTiles[i, j] &&
                    !slimeTiles[i, j])
                {
                  
                    // loop from the space above to the top of the column
                    for(int k = j + 1; k < height; k++)
                    {
                        // if that dot is found
                        if (allDots[i, k] != null)
                        {
                            // move that dot to this empty space
                            allDots[i, k].GetComponent<Dot>().row = j;
                            // set that spot to be null
                            allDots[i, k] = null;
                            break;
                        }
                    }
                }
            }
        }
        yield return new WaitForSeconds((refillDelay *0.5f));
        StartCoroutine(fillBoardCo());
    }

    private void refillBoard()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] == null&& !blankSpaces[i,j] 
                    &&(currentState!=gameState.win || currentState!= gameState.lose)
                    && !concreteTiles[i, j] && !slimeTiles[i, j])
                {
                    Vector2 tempPosition = new Vector2(i, j+slideInOffSet);
                    int dotToUse = Random.Range(0, Dots.Length);

                    // not to allow pieces to be fill in where there is a match
                    int maxIterations = 0;
                    while (matchesAt(i, j, Dots[dotToUse]) && maxIterations <100)
                    {
                        maxIterations++;
                        dotToUse = Random.Range(0, Dots.Length);
                    }
                    maxIterations = 0;

                    GameObject piece = Instantiate(Dots[dotToUse], tempPosition, Quaternion.identity);
                    allDots[i, j] = piece;
                    piece.GetComponent<Dot>().column = i;
                    piece.GetComponent<Dot>().row = j;

                }
            }
        }
    }

    private IEnumerator fillBoardCo()
    {
        yield return new WaitForSeconds(refillDelay);
        refillBoard();
        yield return new WaitForSeconds(refillDelay);  
        while (matchesOnBoard())
        {
            streakValue++;
            destroyMatches();
            yield break;  // to exit from the coRoutine entirely 
        }
        currentDot = null;

        checkToMakeSlime();
        if (isDeadLock())
        {
            shufflingBoard();
        }
        yield return new WaitForSeconds(refillDelay);
        if(currentState!= gameState.pause)
            currentState = gameState.move;

        makeSlime = true;
        streakValue = 1;
    }


    private bool matchesOnBoard()
    {
        findmatches.findAllMatches();
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] != null)
                {
                    if (allDots[i, j].GetComponent<Dot>().isMatched)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }


    private void switchPieces(int column, int row , Vector2 direction)
    {
        if(allDots[column + (int)direction.x, row + (int)direction.y] != null)
        {
            // take the second piece and save it into an holder
            GameObject holder = allDots[column + (int)direction.x, row + (int)direction.y] as GameObject;
            //switiching the first dot to be the second position
            allDots[column + (int)direction.x, row + (int)direction.y] = allDots[column, row];
            // set the first dot to be the second dot
            allDots[column, row] = holder;
        }
    }

    private bool checkForMatches()
    {
        for(int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] != null)
                {
                    // make sure one and two to the right are inside the board
                    if (i < width - 2)
                    {
                        // check the right dots 
                        if (allDots[i + 1, j] != null && allDots[i + 2, j] != null)
                        {
                            if (allDots[i + 1, j].tag == allDots[i, j].tag &&
                                allDots[i + 2, j].tag == allDots[i, j].tag)
                            {
                                return true;
                            }
                        }
                    }

                    // make sure the upper dots are inside the board
                    if (j < height - 2)
                    {
                        // check the upper dots
                        if (allDots[i, j + 1] != null && allDots[i, j + 2] != null)
                        {
                            if (allDots[i, j + 1].tag == allDots[i, j].tag &&
                                allDots[i, j + 2].tag == allDots[i, j].tag)
                            {
                                return true;
                            }
                        }
                    }
                  
                }
            }
        }

        return false;
    }

    public bool switchAndScheck(int column, int row, Vector2 direction)
    {
        switchPieces(column, row, direction);
        if (checkForMatches())
        {
            switchPieces(column, row, direction);
            return true;
        }
        switchPieces(column, row, direction);
        return false;
    }

    private bool isDeadLock()
    {
        for(int i = 0; i < width; i++)
        {
            for(int j = 0; j < height; j++)
            {
                if (allDots[i, j] != null)
                {
                    // check matches to the right
                    if (i < width - 2)
                    {
                        if(switchAndScheck(i, j, Vector2.right))
                        {
                            return false;
                        }
                    }

                    //check up or matches
                    if (j < height - 2)
                    {
                        if(switchAndScheck(i, j, Vector2.up))
                        {
                            return false;
                        }
                    }
                }
            }
        }
        return true;
    }

    private void shufflingBoard()
    {
        // create a list of gameobjects
        List<GameObject> board = new List<GameObject>();
        // add all pieces to this list 
        for(int i = 0; i < width; i++)
        {
            for(int j = 0; j < height; j++)
            {
                if (allDots[i, j] != null)
                {
                    board.Add(allDots[i, j]);
                }
            }
        }

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                // if this spot shouldn't be a blank or concrete or slime
                if (!blankSpaces[i, j] && !concreteTiles[i, j] && !slimeTiles[i, j])
                {
                    // pick a random spot
                    int pieceToUse = Random.Range(0, board.Count);
                    // to avoid making a board with a match
                    int maxIteration = 0;
                    while (matchesAt(i, j,board[pieceToUse]) && maxIteration < 100)
                    {
                        pieceToUse = Random.Range(0, board.Count);
                        maxIteration++;
                    }
                    maxIteration = 0;

                    // container for the piece 
                    Dot piece = board[pieceToUse].GetComponent<Dot>();
                    // assign the values to the piece's values 
                    piece.column = i;
                    piece.row = j;
                    allDots[i, j] = board[pieceToUse];
                    board.Remove(board[pieceToUse]);
                }
            }
        }
        // check if it's still deadlock board 
        if (isDeadLock())
        {
            shufflingBoard();
        }
    }

    private Vector2 checkForAdjacent(int column , int row)
    {
        if(  column < width - 1 && allDots[column+1,row] !=null )
        {
            return Vector2.right;
        }
        if (  column > 0 && allDots[column -1, row] != null)
        {
            return Vector2.left;
        }
        if (   row < height - 1 && allDots[column, row+1] != null)
        {
            return Vector2.up;
        }
        if ( row > 0 && allDots[column, row-1] != null)
        {
            return Vector2.down;
        }
        return Vector2.zero;
    }

    private void makeNewSlime()
    {
        bool slime = false;
        int loops = 0;
        while (!slime && loops <200)
        {
            // choose random position on the board
            int newX = Random.Range(0,width);
            int newY = Random.Range(0,height);
            // check if there is a smile tile in the board
            if (slimeTiles[newX, newY] !=null)
            {
                // check if there is an adjacent tile available
                Vector2 adjacent = checkForAdjacent(newX, newY);
                if(adjacent != Vector2.zero)
                {
                    Destroy(allDots[newX+(int) adjacent.x, newY+(int)adjacent.y]);
                    Vector2 tempPositon = new Vector2(newX + (int)adjacent.x, newY + (int)adjacent.y);
                    // create a new smile at the position of the adjacnet destroyed tile
                    GameObject tile = Instantiate(slimeTilePrefab, tempPositon, Quaternion.identity);
                    slimeTiles[newX + (int)adjacent.x, newY + (int)adjacent.y] = tile.GetComponent<BackgroundTile>();
                    slime = true;
                }
            }
            loops++;
        }
    }

    private void checkToMakeSlime()
    {
        // check the slime tile array
        for(int i=0; i < width; i++)
        {
            for(int j = 0; j < height; j++)
            {
                if(slimeTiles[i,j]!=null && makeSlime)
                {
                    makeNewSlime();
                    return;                }
            }
        }
    }
}
