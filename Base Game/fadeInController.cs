using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fadeInController : MonoBehaviour
{
    public Animator panelAnim;
    public Animator gameInfoAnim;



    public void OK()
    {
        if(panelAnim!=null && gameInfoAnim != null)
        {
            panelAnim.SetBool("Out", true);
            gameInfoAnim.SetBool("Out", true);
            StartCoroutine(gameStartCo());
        }
    }

    IEnumerator gameStartCo()
    {
        yield return new WaitForSeconds(1);
        Board board = FindObjectOfType<Board>();
        board.currentState = gameState.move;
    }

    public void gameOver()
    {
        panelAnim.SetBool("Out", false);
        panelAnim.SetBool("Game Over", true);

    }
}
