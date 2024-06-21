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

    private enum eStoneState // �΂̏��
    {
        EMPTY, // �΂���
        WHITE, // �΂̏オ��
        BLACK, // �΂̏オ��
    }

    void Start()
    {
        startTurnFlag = true;
        // 8x8�̃{�[�h�𐶐�����
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
                Debug.Log("�p�X");
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
            // �΂̐����J�E���g
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

        // �����̈ʒu�ɐ΂�ݒu����
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

        // �΂̐����J�E���g
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
            // �΂̍��W��x��z�̒l���l�̌ܓ�����
            int x = (int)Math.Round(stone.transform.position.x, 0, MidpointRounding.AwayFromZero);
            int z = (int)Math.Round(stone.transform.position.z, 0, MidpointRounding.AwayFromZero);
            
            // �{�[�h�̍��W�̐΂̍��W����v������΂��폜����
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

    // �΂�ݒu����
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

                    // �X�V���boardState��currentBoardState�ɃR�s�[
                    for (int i = 0; i < BOARD_MAXCOUNT_X; i++)
                    {
                        for (int j = 0; j < BOARD_MAXCOUNT_Z; j++)
                        {
                            currentBoardState[i, j] = boardState[i, j];
                        }
                    }

                    // startTurnFlag��L���ɂ���
                    startTurnFlag = true;
                }
            }
        }
    }

    int counter = 0;

    // �Ђ����肩������΂�Ԃ�
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

    // ��łЂ�����Ԃ���΂����݂��邩�`�F�b�N
    private List<Vector2Int> UpStornCheck(int x, int y)
    {
        List<Vector2Int> flippableStones = new List<Vector2Int>();
        if (y == (BOARD_MAXCOUNT_Z - 1)) return flippableStones; // �ŏ�s�ł������牽�����Ȃ�

        int currentY = y + 1;
        bool canFlip = false; // �Ђ�����Ԃ��邩�ǂ���
        // ����̐΂�������܂ŏ�ɐi��
        while (currentY < BOARD_MAXCOUNT_Z && boardState[x, currentY] != eStoneState.EMPTY)
        {
            eStoneState opponentStone = blackTurn ? eStoneState.WHITE : eStoneState.BLACK;
            eStoneState myStone = blackTurn ? eStoneState.BLACK : eStoneState.WHITE;

            if (boardState[x, currentY] == myStone)
            {
                if (canFlip) // �����̐΂ɓ��B���A���̊Ԃɑ���̐΂��������ꍇ
                {
                    for (int flipY = y + 1; flipY < currentY; flipY++)
                    {
                        flippableStones.Add(new Vector2Int(x, flipY));
                    }
                    return flippableStones; // �΂�u�����������s
                }
                else
                {
                    break; // �A�����鑊��̐΂��Ȃ��ꍇ�͒u���Ȃ�
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

    // ���łЂ�����Ԃ���΂����݂��邩�`�F�b�N
    private List<Vector2Int> DownStornCheck(int x, int y)
    {
        List<Vector2Int> flippableStones = new List<Vector2Int>();
        if (y == 0) return flippableStones; // �ŉ��s�ł������牽�����Ȃ�

        int currentY = y - 1;
        bool canFlip = false; // �Ђ�����Ԃ��邩�ǂ���
        // ����̐΂�������܂ŉ��ɐi��
        while (currentY >= 0 && boardState[x, currentY] != eStoneState.EMPTY)
        {
            eStoneState opponentStone = blackTurn ? eStoneState.WHITE : eStoneState.BLACK;
            eStoneState myStone = blackTurn ? eStoneState.BLACK : eStoneState.WHITE;

            if (boardState[x, currentY] == myStone)
            {
                if (canFlip) // �����̐΂ɓ��B���A���̊Ԃɑ���̐΂��������ꍇ
                {
                    for (int flipY = y - 1; flipY > currentY; flipY--)
                    {
                        flippableStones.Add(new Vector2Int(x, flipY));
                    }
                    return flippableStones; // �΂�u�����������s
                }
                else
                {
                    break; // �A�����鑊��̐΂��Ȃ��ꍇ�͒u���Ȃ�
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

    // �E�łЂ�����Ԃ���΂����݂��邩�`�F�b�N
    private List<Vector2Int> RightStornCheck(int x, int y)
    {
        List<Vector2Int> flippableStones = new List<Vector2Int>();
        if (x == BOARD_MAXCOUNT_X - 1) return flippableStones; // �ŉE�s�ł������牽�����Ȃ�
        
        int currentX = x + 1;
        bool canFlip = false; // �Ђ�����Ԃ��邩�ǂ���
  
        // ����̐΂�������܂ŉE�ɐi��
        while (currentX < BOARD_MAXCOUNT_X && boardState[currentX, y] != eStoneState.EMPTY)
        {
            eStoneState opponentStone = blackTurn ? eStoneState.WHITE : eStoneState.BLACK;
            eStoneState myStone = blackTurn ? eStoneState.BLACK : eStoneState.WHITE;

            if (boardState[currentX, y] == myStone)
            {
                if (canFlip) // �����̐΂ɓ��B���A���̊Ԃɑ���̐΂��������ꍇ
                {
                    for (int flipX = x + 1; flipX < currentX; flipX++)
                    {
                        flippableStones.Add(new Vector2Int(flipX, y));
                    }
                    return flippableStones; // �΂�u�����������s
                }
                else
                {
                    break; // �A�����鑊��̐΂��Ȃ��ꍇ�͒u���Ȃ�
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

    // ���łЂ�����Ԃ���΂����݂��邩�`�F�b�N
    private List<Vector2Int> LeftStornCheck(int x, int y)
    {
        List<Vector2Int> flippableStones = new List<Vector2Int>();
        if (x == 0) return flippableStones; // �ō��s�ł������牽�����Ȃ�

        int currentX = x - 1;
        bool canFlip = false; // �Ђ�����Ԃ��邩�ǂ���

        // ����̐΂�������܂ō��ɐi��
        while (currentX >= 0 && boardState[currentX, y] != eStoneState.EMPTY)
        {
            eStoneState opponentStone = blackTurn ? eStoneState.WHITE : eStoneState.BLACK;
            eStoneState myStone = blackTurn ? eStoneState.BLACK : eStoneState.WHITE;

            if (boardState[currentX, y] == myStone)
            {
                if (canFlip) // �����̐΂ɓ��B���A���̊Ԃɑ���̐΂��������ꍇ
                {
                    for (int flipX = x - 1; flipX > currentX; flipX--)
                    {
                        flippableStones.Add(new Vector2Int(flipX, y));
                    }
                    return flippableStones; // �΂�u�����������s
                }
                else
                {
                    break; // �A�����鑊��̐΂��Ȃ��ꍇ�͒u���Ȃ�
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

    // �E��łЂ�����Ԃ���΂����݂��邩�`�F�b�N
    private List<Vector2Int> UpRightStornCheck(int x, int y)
    {
        List<Vector2Int> flippableStones = new List<Vector2Int>();
        if (x == (BOARD_MAXCOUNT_X - 1)) return flippableStones; // �ŉE�s�ł������牽�����Ȃ�
        if (y == (BOARD_MAXCOUNT_Z - 1)) return flippableStones; // �ŏ�s�ł������牽�����Ȃ�

        int currentX = x + 1;
        int currentY = y + 1;
        bool canFlip = false; // �Ђ�����Ԃ��邩�ǂ���

        // ����̐΂�������܂ŉE��ɐi��
        while (currentX < BOARD_MAXCOUNT_X && currentY < BOARD_MAXCOUNT_Z && boardState[currentX, currentY] != eStoneState.EMPTY)
        {
            eStoneState opponentStone = blackTurn ? eStoneState.WHITE : eStoneState.BLACK;
            eStoneState myStone = blackTurn ? eStoneState.BLACK : eStoneState.WHITE;

            if (boardState[currentX, currentY] == myStone)
            {
                if (canFlip) // �����̐΂ɓ��B���A���̊Ԃɑ���̐΂��������ꍇ
                {
                    int flipX = x + 1;
                    int flipY = y + 1;
                    while (flipX < currentX && flipY < currentY)
                    {
                        flippableStones.Add(new Vector2Int(flipX, flipY));
                        flipX++;
                        flipY++;
                    }
                    return flippableStones; // �΂�u�����������s
                }
                else
                {
                    break; // �A�����鑊��̐΂��Ȃ��ꍇ�͒u���Ȃ�
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

    // �E���łЂ�����Ԃ���΂����݂��邩�`�F�b�N
    private List<Vector2Int> DownRightStornCheck(int x, int y)
    {
        List<Vector2Int> flippableStones = new List<Vector2Int>();
        if (x == (BOARD_MAXCOUNT_X - 1)) return flippableStones; // �ŉE�s�ł������牽�����Ȃ�
        if (y == 0) return flippableStones; // �ŉ��s�ł������牽�����Ȃ�

        int currentX = x + 1;
        int currentY = y - 1;
        bool canFlip = false; // �Ђ�����Ԃ��邩�ǂ���

        while (currentX < BOARD_MAXCOUNT_X && currentY >= 0 && boardState[currentX, currentY] != eStoneState.EMPTY)
        {
            eStoneState opponentStone = blackTurn ? eStoneState.WHITE : eStoneState.BLACK;
            eStoneState myStone = blackTurn ? eStoneState.BLACK : eStoneState.WHITE;

            if (boardState[currentX, currentY] == myStone)
            {
                if (canFlip) // �����̐΂ɓ��B���A���̊Ԃɑ���̐΂��������ꍇ
                {
                    int flipX = x + 1;
                    int flipY = y - 1;
                    while (flipX < currentX && flipY > currentY)
                    {

                        flippableStones.Add(new Vector2Int(flipX, flipY));
                        flipX++;
                        flipY--;
                    }
                    return flippableStones; // �΂�u�����������s
                }
                else
                {
                    break; // �A�����鑊��̐΂��Ȃ��ꍇ�͒u���Ȃ�
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

    // ����łЂ�����Ԃ���΂����݂��邩�`�F�b�N
    private List<Vector2Int> UpLeftStornCheck(int x, int y)
    {
        List<Vector2Int> flippableStones = new List<Vector2Int>();
        if (x == 0) return flippableStones; // �ŉE�s�ł������牽�����Ȃ�
        if (y == (BOARD_MAXCOUNT_Z - 1)) return flippableStones; // �ŉ��s�ł������牽�����Ȃ�

        int currentX = x - 1;
        int currentY = y + 1;
        bool canFlip = false; // �Ђ�����Ԃ��邩�ǂ���
        // ����̐΂�������܂ō���ɐi��
        while (currentX >= 0 && currentY < BOARD_MAXCOUNT_Z && boardState[currentX, currentY] != eStoneState.EMPTY)
        {
            eStoneState opponentStone = blackTurn ? eStoneState.WHITE : eStoneState.BLACK;
            eStoneState myStone = blackTurn ? eStoneState.BLACK : eStoneState.WHITE;

            if (boardState[currentX, currentY] == myStone)
            {
                if (canFlip) // �����̐΂ɓ��B���A���̊Ԃɑ���̐΂��������ꍇ
                {
                    int flipX = x - 1;
                    int flipY = y + 1;
                    while (flipX > currentX && flipY < currentY)
                    {
                        flippableStones.Add(new Vector2Int(flipX, flipY));
                        flipX--;
                        flipY++;
                    }
                    return flippableStones; // �΂�u�����������s
                }
                else
                {
                    break; // �A�����鑊��̐΂��Ȃ��ꍇ�͒u���Ȃ�
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

    // �����łЂ�����Ԃ���΂����݂��邩�`�F�b�N
    private List<Vector2Int> DownLeftStornCheck(int x, int y)
    {
        List<Vector2Int> flippableStones = new List<Vector2Int>();
        if (x == 0) return flippableStones; // �ō��s�ł������牽�����Ȃ�
        if (y == 0) return flippableStones; // �ŉ��s�ł������牽�����Ȃ�

        int currentX = x - 1;
        int currentY = y - 1;
        bool canFlip = false; // �Ђ�����Ԃ��邩�ǂ���
        // ����̐΂�������܂ō���ɐi��
        while (currentX >= 0 && currentY >= 0 && boardState[currentX, currentY] != eStoneState.EMPTY)
        {
            eStoneState opponentStone = blackTurn ? eStoneState.WHITE : eStoneState.BLACK;
            eStoneState myStone = blackTurn ? eStoneState.BLACK : eStoneState.WHITE;

            if (boardState[currentX, currentY] == myStone)
            {
                if (canFlip) // �����̐΂ɓ��B���A���̊Ԃɑ���̐΂��������ꍇ
                {
                    int flipX = x - 1;
                    int flipY = y - 1;
                    while (flipX > currentX && flipY > currentY)
                    {
                        flippableStones.Add(new Vector2Int(flipX, flipY));
                        flipX--;
                        flipY--;
                    }
                    return flippableStones; // �΂�u�����������s
                }
                else
                {
                    break; // �A�����鑊��̐΂��Ȃ��ꍇ�͒u���Ȃ�
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

    // ���΂̍��v���J�E���g
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

    // ���΂̍��v���J�E���g
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

    // �Q�[���I������
    private bool GameEndCheck()
    {
        // ���̐΂�0���`�F�b�N
        if (0 != BlackStoneCount()) return false;

        // ���̐΂�0���`�F�b�N
        if (0 != WhiteStoneCount()) return false;

        // ��̃}�X�����݂��邩�`�F�b�N
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

        // 8�������`�F�b�N
        // ����A��A�E��A���A�E�A�����A���A�E��
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

    // �S�Ă̈ʒu���`�F�b�N����֐�
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
