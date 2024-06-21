using System;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    private const int BOARD_MAXCOUNT_X = 8;
    private const int BOARD_MAXCOUNT_Z = 8;

    public GameObject board1, board2;

    public GameObject stone;

    private bool blackTurn = true;

    public bool BlackTurn
    {
        get { return blackTurn; }
        private set { blackTurn = value; }
    }

    private bool startTurnFlag = false;

    private eStoneState[,] boardState = new eStoneState[BOARD_MAXCOUNT_X, BOARD_MAXCOUNT_Z];
    private eStoneState[,] currentBoardState = new eStoneState[BOARD_MAXCOUNT_X, BOARD_MAXCOUNT_Z];
    GameObject clickedGameObject;

    private List<GameObject> stornList = new List<GameObject>();

    private enum eStoneState // 石の状態
    {
        EMPTY, // 石が空
        WHITE, // 石の上が白
        BLACK, // 石の上が黒
    }

    void Start()
    {
        startTurnFlag = true;
        // 8x8のボードを生成する
        CreateBoad();

    }

    // Update is called once per frame
    void Update()
    {
        if (startTurnFlag)
        {
            eStoneState stoneState = blackTurn ? eStoneState.BLACK : eStoneState.WHITE;
            
            if (!HasValidMove(stoneState))
            {
                Debug.Log("パス");
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 15.0f))
            {
                clickedGameObject = hit.collider.gameObject;
                Board board = clickedGameObject.GetComponent<Board>();
                if (board)
                {
                    SetStorn(board);

                }
            }
            // 石の数をカウント
            GameManager.instance.textManager.BlackStoneCounter = BlackStoneCount();
            GameManager.instance.textManager.WhiteStoneCounter = WhiteStoneCount();

            bool endFlag = GameEndCheck();

            //Debug.Log(endFlag);
        }
    }

    public void CreateBoad()
    {
        for (int i = 0; i < BOARD_MAXCOUNT_X; i++)
        {
            for (int j = 0; j < BOARD_MAXCOUNT_Z; j++)
            {
                boardState[i, j] = eStoneState.EMPTY;

                bool isEvenI = i % 2 == 0;
                bool isEvenJ = j % 2 == 0;

                GameObject boardPlece = isEvenI ? (isEvenJ ? board1 : board2) : (isEvenJ ? board2 : board1);

                GameObject obj = Instantiate(boardPlece, new Vector3(i, 0, j), Quaternion.identity);
                SetPosition(obj, i , j);
            }
        }

        // 初期の位置に石を設置する
        boardState[3, 3] = eStoneState.WHITE;
        boardState[3, 4] = eStoneState.BLACK;
        boardState[4, 3] = eStoneState.BLACK;
        boardState[4, 4] = eStoneState.WHITE;

        for (int i = 0; i < BOARD_MAXCOUNT_X; i++)
        {
            for (int j = 0; j < BOARD_MAXCOUNT_Z; j++)
            {
                currentBoardState[i, j] = boardState[i, j];
                switch (boardState[i, j])
                {
                    case eStoneState.WHITE:
                         PlaceStone(i, j, 0.1f, Quaternion.Euler(0, 0, 0), eStoneState.WHITE);
                         break;
                    case eStoneState.BLACK:
                         PlaceStone(i, j, 0.2f, Quaternion.Euler(180, 0, 0), eStoneState.BLACK);
                         break;
                    default:
                         break;
                }
            }
        }

        // 石の数をカウント
        GameManager.instance.textManager.BlackStoneCounter = BlackStoneCount();
        GameManager.instance.textManager.WhiteStoneCounter = WhiteStoneCount();
    }

    private void ViewStone()
    {
        for (int i = 0; i < BOARD_MAXCOUNT_X; i++)
        {
            for (int j = 0; j < BOARD_MAXCOUNT_Z; j++)
            {
                if (currentBoardState[i, j] != boardState[i, j])
                {
                    switch (boardState[i, j])
                    {
                        case eStoneState.WHITE:
                            PlaceStone(i, j, 0.1f, Quaternion.Euler(0, 0, 0), eStoneState.WHITE);
                            break;
                        case eStoneState.BLACK:
                            PlaceStone(i, j, 0.2f, Quaternion.Euler(180, 0, 0), eStoneState.BLACK);
                            break;
                        default:
                            break;
                    }
                }
            }
        }
    }

    private void PlaceStone(int i, int j, float height, Quaternion rotation, eStoneState stoneType)
    {
        GameObject newStone = Instantiate(stone, new Vector3(i, height, j), rotation);
        RemoveExistingStonesAtPosition(i, j);
        stornList.Add(newStone);
    }

    private void RemoveExistingStonesAtPosition(int i, int j)
    {
        List<GameObject> toRemove = new List<GameObject>();

        foreach (GameObject stone in stornList)
        {
            // 石の座標のxとzの値を四捨五入する
            int x = (int)Math.Round(stone.transform.position.x, 0, MidpointRounding.AwayFromZero);
            int z = (int)Math.Round(stone.transform.position.z, 0, MidpointRounding.AwayFromZero);
            
            // ボードの座標の石の座標が一致したら石を削除する
            if ((x == i) && (z == j)) 
            {
                if (blackTurn)
                {
                    if (boardState[i, j] == eStoneState.BLACK) break;
                }
                else
                {
                    if (boardState[i, j] == eStoneState.WHITE) break;
                }
                toRemove.Add(stone);
            }
        }

        foreach (GameObject stone in toRemove)
        {
            stornList.Remove(stone);
            Destroy(stone);
        }
    }

    private void SetPosition(GameObject boardPlece, int i , int j)
    {
        Board boardScript = boardPlece.GetComponent<Board>();

        boardScript.BoardPositon = new Vector2(i, j);

    }

    // 石を設置する
    private void SetStorn(Board board)
    {
        if (board)
        {
            int x = (int)board.BoardPositon.x;
            int y = (int)board.BoardPositon.y;
            if (x >= 0 && x < BOARD_MAXCOUNT_X && y >= 0 && y < BOARD_MAXCOUNT_Z && boardState[x, y] == eStoneState.EMPTY)
            {

                List<Vector2Int> flippableStones = OpponentStorneAll(x, y);

                if (flippableStones.Count > 0)
                {
                    eStoneState newStoneState = blackTurn ? eStoneState.BLACK : eStoneState.WHITE;
                    boardState[x, y] = newStoneState;
                    foreach (Vector2Int pos in flippableStones)
                    {
                        boardState[pos.x, pos.y] = newStoneState;
                    }

                    blackTurn = !blackTurn;

                    ViewStone();

                    // 更新後のboardStateをcurrentBoardStateにコピー
                    for (int i = 0; i < BOARD_MAXCOUNT_X; i++)
                    {
                        for (int j = 0; j < BOARD_MAXCOUNT_Z; j++)
                        {
                            currentBoardState[i, j] = boardState[i, j];
                        }
                    }

                    // startTurnFlagを有効にする
                    startTurnFlag = true;
                }
            }
        }
    }

    int counter = 0;

    // ひっくりかえせる石を返す
    private List<Vector2Int> OpponentStorneAll(int x, int y)
    {
        List<Vector2Int> flippableStones = new List<Vector2Int>();

        flippableStones.AddRange(UpStornCheck(x, y));
        flippableStones.AddRange(DownStornCheck(x, y));
        flippableStones.AddRange(RightStornCheck(x, y));
        flippableStones.AddRange(LeftStornCheck(x, y));
        flippableStones.AddRange(UpRightStornCheck(x, y));
        flippableStones.AddRange(DownRightStornCheck(x, y));
        flippableStones.AddRange(UpLeftStornCheck(x, y));
        flippableStones.AddRange(DownLeftStornCheck(x, y));

        counter++;

        return flippableStones;
    }

    // 上でひっくり返せる石が存在するかチェック
    private List<Vector2Int> UpStornCheck(int x, int y)
    {
        List<Vector2Int> flippableStones = new List<Vector2Int>();
        if (y == (BOARD_MAXCOUNT_Z - 1)) return flippableStones; // 最上行であったら何もしない

        int currentY = y + 1;
        bool canFlip = false; // ひっくり返せるかどうか
        // 相手の石を見つけるまで上に進む
        while (currentY < BOARD_MAXCOUNT_Z && boardState[x, currentY] != eStoneState.EMPTY)
        {
            eStoneState opponentStone = blackTurn ? eStoneState.WHITE : eStoneState.BLACK;
            eStoneState myStone = blackTurn ? eStoneState.BLACK : eStoneState.WHITE;

            if (boardState[x, currentY] == myStone)
            {
                if (canFlip) // 自分の石に到達し、その間に相手の石があった場合
                {
                    for (int flipY = y + 1; flipY < currentY; flipY++)
                    {
                        flippableStones.Add(new Vector2Int(x, flipY));
                    }
                    return flippableStones; // 石を置く処理を実行
                }
                else
                {
                    break; // 連続する相手の石がない場合は置けない
                }
            }
            else if(boardState[x, currentY] == opponentStone)
            {
                canFlip = true;
            }
            currentY++;
        }
        return flippableStones;
    }

    // 下でひっくり返せる石が存在するかチェック
    private List<Vector2Int> DownStornCheck(int x, int y)
    {
        List<Vector2Int> flippableStones = new List<Vector2Int>();
        if (y == 0) return flippableStones; // 最下行であったら何もしない

        int currentY = y - 1;
        bool canFlip = false; // ひっくり返せるかどうか
        // 相手の石を見つけるまで下に進む
        while (currentY >= 0 && boardState[x, currentY] != eStoneState.EMPTY)
        {
            eStoneState opponentStone = blackTurn ? eStoneState.WHITE : eStoneState.BLACK;
            eStoneState myStone = blackTurn ? eStoneState.BLACK : eStoneState.WHITE;

            if (boardState[x, currentY] == myStone)
            {
                if (canFlip) // 自分の石に到達し、その間に相手の石があった場合
                {
                    for (int flipY = y - 1; flipY > currentY; flipY--)
                    {
                        flippableStones.Add(new Vector2Int(x, flipY));
                    }
                    return flippableStones; // 石を置く処理を実行
                }
                else
                {
                    break; // 連続する相手の石がない場合は置けない
                }
            }
            else if (boardState[x, currentY] == opponentStone)
            {
                canFlip = true;
            }
            currentY--;
        }
        return flippableStones;
    }

    // 右でひっくり返せる石が存在するかチェック
    private List<Vector2Int> RightStornCheck(int x, int y)
    {
        List<Vector2Int> flippableStones = new List<Vector2Int>();
        if (x == BOARD_MAXCOUNT_X - 1) return flippableStones; // 最右行であったら何もしない
        
        int currentX = x + 1;
        bool canFlip = false; // ひっくり返せるかどうか
  
        // 相手の石を見つけるまで右に進む
        while (currentX < BOARD_MAXCOUNT_X && boardState[currentX, y] != eStoneState.EMPTY)
        {
            eStoneState opponentStone = blackTurn ? eStoneState.WHITE : eStoneState.BLACK;
            eStoneState myStone = blackTurn ? eStoneState.BLACK : eStoneState.WHITE;

            if (boardState[currentX, y] == myStone)
            {
                if (canFlip) // 自分の石に到達し、その間に相手の石があった場合
                {
                    for (int flipX = x + 1; flipX < currentX; flipX++)
                    {
                        flippableStones.Add(new Vector2Int(flipX, y));
                    }
                    return flippableStones; // 石を置く処理を実行
                }
                else
                {
                    break; // 連続する相手の石がない場合は置けない
                }
            }
            else if (boardState[currentX, y] == opponentStone)
            {
                canFlip = true;
            }
            currentX++;
        }
        return flippableStones;
    }

    // 左でひっくり返せる石が存在するかチェック
    private List<Vector2Int> LeftStornCheck(int x, int y)
    {
        List<Vector2Int> flippableStones = new List<Vector2Int>();
        if (x == 0) return flippableStones; // 最左行であったら何もしない

        int currentX = x - 1;
        bool canFlip = false; // ひっくり返せるかどうか

        // 相手の石を見つけるまで左に進む
        while (currentX >= 0 && boardState[currentX, y] != eStoneState.EMPTY)
        {
            eStoneState opponentStone = blackTurn ? eStoneState.WHITE : eStoneState.BLACK;
            eStoneState myStone = blackTurn ? eStoneState.BLACK : eStoneState.WHITE;

            if (boardState[currentX, y] == myStone)
            {
                if (canFlip) // 自分の石に到達し、その間に相手の石があった場合
                {
                    for (int flipX = x - 1; flipX > currentX; flipX--)
                    {
                        flippableStones.Add(new Vector2Int(flipX, y));
                    }
                    return flippableStones; // 石を置く処理を実行
                }
                else
                {
                    break; // 連続する相手の石がない場合は置けない
                }
            }
            else if (boardState[currentX, y] == opponentStone)
            {
                canFlip = true;
            }
            currentX--;
        }
        return flippableStones;
    }

    // 右上でひっくり返せる石が存在するかチェック
    private List<Vector2Int> UpRightStornCheck(int x, int y)
    {
        List<Vector2Int> flippableStones = new List<Vector2Int>();
        if (x == (BOARD_MAXCOUNT_X - 1)) return flippableStones; // 最右行であったら何もしない
        if (y == (BOARD_MAXCOUNT_Z - 1)) return flippableStones; // 最上行であったら何もしない

        int currentX = x + 1;
        int currentY = y + 1;
        bool canFlip = false; // ひっくり返せるかどうか

        // 相手の石を見つけるまで右上に進む
        while (currentX < BOARD_MAXCOUNT_X && currentY < BOARD_MAXCOUNT_Z && boardState[currentX, currentY] != eStoneState.EMPTY)
        {
            eStoneState opponentStone = blackTurn ? eStoneState.WHITE : eStoneState.BLACK;
            eStoneState myStone = blackTurn ? eStoneState.BLACK : eStoneState.WHITE;

            if (boardState[currentX, currentY] == myStone)
            {
                if (canFlip) // 自分の石に到達し、その間に相手の石があった場合
                {
                    int flipX = x + 1;
                    int flipY = y + 1;
                    while (flipX < currentX && flipY < currentY)
                    {
                        flippableStones.Add(new Vector2Int(flipX, flipY));
                        flipX++;
                        flipY++;
                    }
                    return flippableStones; // 石を置く処理を実行
                }
                else
                {
                    break; // 連続する相手の石がない場合は置けない
                }
            }
            else if (boardState[currentX, currentY] == opponentStone)
            {
                canFlip = true;
            }
            currentX++;
            currentY++;
        }
        return flippableStones;
    }

    // 右下でひっくり返せる石が存在するかチェック
    private List<Vector2Int> DownRightStornCheck(int x, int y)
    {
        List<Vector2Int> flippableStones = new List<Vector2Int>();
        if (x == (BOARD_MAXCOUNT_X - 1)) return flippableStones; // 最右行であったら何もしない
        if (y == 0) return flippableStones; // 最下行であったら何もしない

        int currentX = x + 1;
        int currentY = y - 1;
        bool canFlip = false; // ひっくり返せるかどうか

        while (currentX < BOARD_MAXCOUNT_X && currentY >= 0 && boardState[currentX, currentY] != eStoneState.EMPTY)
        {
            eStoneState opponentStone = blackTurn ? eStoneState.WHITE : eStoneState.BLACK;
            eStoneState myStone = blackTurn ? eStoneState.BLACK : eStoneState.WHITE;

            if (boardState[currentX, currentY] == myStone)
            {
                if (canFlip) // 自分の石に到達し、その間に相手の石があった場合
                {
                    int flipX = x + 1;
                    int flipY = y - 1;
                    while (flipX < currentX && flipY > currentY)
                    {

                        flippableStones.Add(new Vector2Int(flipX, flipY));
                        flipX++;
                        flipY--;
                    }
                    return flippableStones; // 石を置く処理を実行
                }
                else
                {
                    break; // 連続する相手の石がない場合は置けない
                }
            }
            else if (boardState[currentX, currentY] == opponentStone)
            {
                canFlip = true;
            }
            currentX++;
            currentY--;
        }
        return flippableStones;
    }

    // 左上でひっくり返せる石が存在するかチェック
    private List<Vector2Int> UpLeftStornCheck(int x, int y)
    {
        List<Vector2Int> flippableStones = new List<Vector2Int>();
        if (x == 0) return flippableStones; // 最右行であったら何もしない
        if (y == (BOARD_MAXCOUNT_Z - 1)) return flippableStones; // 最下行であったら何もしない

        int currentX = x - 1;
        int currentY = y + 1;
        bool canFlip = false; // ひっくり返せるかどうか
        // 相手の石を見つけるまで左上に進む
        while (currentX >= 0 && currentY < BOARD_MAXCOUNT_Z && boardState[currentX, currentY] != eStoneState.EMPTY)
        {
            eStoneState opponentStone = blackTurn ? eStoneState.WHITE : eStoneState.BLACK;
            eStoneState myStone = blackTurn ? eStoneState.BLACK : eStoneState.WHITE;

            if (boardState[currentX, currentY] == myStone)
            {
                if (canFlip) // 自分の石に到達し、その間に相手の石があった場合
                {
                    int flipX = x - 1;
                    int flipY = y + 1;
                    while (flipX > currentX && flipY < currentY)
                    {
                        flippableStones.Add(new Vector2Int(flipX, flipY));
                        flipX--;
                        flipY++;
                    }
                    return flippableStones; // 石を置く処理を実行
                }
                else
                {
                    break; // 連続する相手の石がない場合は置けない
                }
            }
            else if (boardState[currentX, currentY] == opponentStone)
            {
                canFlip = true;
            }
            currentX--;
            currentY++;
        }
        return flippableStones;
    }

    // 左下でひっくり返せる石が存在するかチェック
    private List<Vector2Int> DownLeftStornCheck(int x, int y)
    {
        List<Vector2Int> flippableStones = new List<Vector2Int>();
        if (x == 0) return flippableStones; // 最左行であったら何もしない
        if (y == 0) return flippableStones; // 最下行であったら何もしない

        int currentX = x - 1;
        int currentY = y - 1;
        bool canFlip = false; // ひっくり返せるかどうか
        // 相手の石を見つけるまで左上に進む
        while (currentX >= 0 && currentY >= 0 && boardState[currentX, currentY] != eStoneState.EMPTY)
        {
            eStoneState opponentStone = blackTurn ? eStoneState.WHITE : eStoneState.BLACK;
            eStoneState myStone = blackTurn ? eStoneState.BLACK : eStoneState.WHITE;

            if (boardState[currentX, currentY] == myStone)
            {
                if (canFlip) // 自分の石に到達し、その間に相手の石があった場合
                {
                    int flipX = x - 1;
                    int flipY = y - 1;
                    while (flipX > currentX && flipY > currentY)
                    {
                        flippableStones.Add(new Vector2Int(flipX, flipY));
                        flipX--;
                        flipY--;
                    }
                    return flippableStones; // 石を置く処理を実行
                }
                else
                {
                    break; // 連続する相手の石がない場合は置けない
                }
            }
            else if (boardState[currentX, currentY] == opponentStone)
            {
                canFlip = true;
            }
            currentX--;
            currentY--;
        }
        return flippableStones;
    }

    // 黒石の合計をカウント
    public int BlackStoneCount()
    {
        int count = 0;
        for (int i = 0; i < currentBoardState.GetLength(0); i++)
        {
            for (int j = 0; j < currentBoardState.GetLength(1); j++)
            {
                if (eStoneState.BLACK == currentBoardState[i,j])
                {
                    count++;
                }
            }
        }

        return count;
    }

    // 白石の合計をカウント
    public int WhiteStoneCount()
    {
        int count = 0;
        for (int i = 0; i < currentBoardState.GetLength(0); i++)
        {
            for (int j = 0; j < currentBoardState.GetLength(1); j++)
            {
                if (eStoneState.WHITE == currentBoardState[i, j])
                {
                    count++;
                }
            }
        }

        return count;
    }

    // ゲーム終了判定
    private bool GameEndCheck()
    {
        // 黒の石が0個かチェック
        if (0 != BlackStoneCount()) return false;

        // 白の石が0個かチェック
        if (0 != WhiteStoneCount()) return false;

        // 空のマスが存在するかチェック
        for (int i = 0; i < currentBoardState.GetLength(0); i++)
        {
            for (int j = 0; j < currentBoardState.GetLength(1); j++)
            {
                if (eStoneState.EMPTY == currentBoardState[i, j])
                {
                    return false;
                }
            }
        }

        return true;
    }

    private bool CanPlaceStone(int x, int z, eStoneState player)
    {
        if (currentBoardState[x, z] != eStoneState.EMPTY)
        {
            return false;
        }

        // 8方向をチェック
        // 左上、上、右上、左、右、左下、下、右下
        int[] dx = { -1, 0, 1, -1, 1, -1,  0,  1};
        int[] dz = {  1, 1, 1,  0, 0, -1, -1, -1};
        for (int i = 0; i < 8; i++)
        {
            int nx = x + dx[i]; // 0 1 = 1
            int nz = z + dz[i]; // 0 1 = 1
            bool foundOpponent = false;

            while (nx >= 0 && nx < BOARD_MAXCOUNT_X && nz >= 0 && nz < BOARD_MAXCOUNT_Z)
            {
                
                if (currentBoardState[nx, nz] == eStoneState.EMPTY)
                {
                    break;
                }

                if (currentBoardState[nx, nz] == (player == eStoneState.BLACK ? eStoneState.WHITE : eStoneState.BLACK))
                {
                    foundOpponent = true;
                }
                else if(currentBoardState[nx, nz] == player)
                {
                    if (foundOpponent)
                    {
                        return true;
                    }
                    break;
                }
                else
                {
                    break;
                }

                nx += dx[i];
                nz += dz[i];
            }
        }

        return false;
    }

    // 全ての位置をチェックする関数
    private bool HasValidMove(eStoneState player)
    {
        for (int x = 0; x < BOARD_MAXCOUNT_X; x++)
        {
            for(int z = 0; z < BOARD_MAXCOUNT_Z; z++)
            {
                if (CanPlaceStone(x, z, player))
                {
                    return true;
                }
            }
        }
        return false;
    }

}
