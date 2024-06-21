using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextManager : MonoBehaviour
{
    public Text turnText;
    public Text whiteStoneCounterText;
    public Text blackStoneCounterText;

    [SerializeField] private GameObject pathText;

    [SerializeField] private GameObject resultPanel;
    [SerializeField] private Text resultWhiteCountText, resultBlackCountText, resultText;

    private int blackStoneCounter = 0;
    private int whiteStoneCounter = 0;
    public int BlackStoneCounter { 
        get { return blackStoneCounter; }
        set { blackStoneCounter = value; } 
    }

    public int WhiteStoneCounter {
        get { return whiteStoneCounter; }
        set { whiteStoneCounter = value; } 
    }

    private string BLACK_STR = "黒";
    private string WHITE_STR = "白";

    // Start is called before the first frame update
    void Start()
    {
        turnText.text = "手番：　" + BLACK_STR;

        blackStoneCounterText.text = BLACK_STR + ":" + BlackStoneCounter;
        whiteStoneCounterText.text = WHITE_STR + ":" + WhiteStoneCounter;

        pathText.SetActive(false);
        resultPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.boardManager.BlackTurn)
        {
            turnText.text = "手番：　" + BLACK_STR;
        }
        else
        {
            turnText.text = "手番：　" + WHITE_STR;
        }

        blackStoneCounterText.text = BLACK_STR + ":" + BlackStoneCounter;
        whiteStoneCounterText.text = WHITE_STR + ":" + WhiteStoneCounter;
    }

    public void ValidPathText()
    {
        pathText.SetActive(true);
    }

    public void InvalidPathText()
    {
        pathText.SetActive(false);
    }

    public void VeiwResult()
    {
        resultPanel.SetActive(true);
        resultBlackCountText.text = "黒：" + BlackStoneCounter;
        resultWhiteCountText.text = "白：" + WhiteStoneCounter;

        int result = BlackStoneCounter - WhiteStoneCounter;

        string str = "";
        if (0 < result)
        {
            str = "黒の勝ち";
        }
        else if(0 > result)
        {
            str = "白の勝ち";
        }
        else
        {
            str = "引き分け";
        }

        resultText.text = str;
    }
}
