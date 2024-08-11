using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer.Internal;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class Board : MonoBehaviour
{
    public Tile[] tiles;
    public Tile[] tiles2;
    public Tile[] tiles3;
    private Tilemap tilemap;

    public Next next;
    public Score score;
    public VolumeBar volumeBar;

    private Vector3Int spawnPos = Vector3Int.zero;
    private bool isBlock = false;
    private bool canClick = true;
    private bool usedEff = false;

    //                    <pos, color, amount>
    private List<Tuple<Vector3Int, int, int>> setBlockList = new List<Tuple<Vector3Int, int, int>>();
    private HashSet<Vector3Int> eraseBlockList = new HashSet<Vector3Int>();

    private bool isFreeze = false;
    private int freezeCnt = 1;
    private int freezeWaves = 2;

    private float scoringPitch = 1;
    private float scoringDuration = 0.05f;

    private int rand;
    private int[] nextID = new int[8];
    public static int[] scoreArray = new int[7];

    private int[] dx = { 1, -1, 0, 0 };
    private int[] dy = { 0, 0, -1, 1 };

    void Awake()
    {
        tilemap = GetComponentInChildren<Tilemap>();
    }

    void Start()
    {
        canClick = true;
        MakeWave();
        SpawnWave();
        MakeWave();
        next.SpawnNext(nextID);
        for (int i = 0; i < 7; i++)
        {
            scoreArray[i] = 0;
        }
    }

    void Update()
    {
        if (Time.timeScale != 0f)
        {
            if (canClick)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    canClick = false;
                    isBlock = false;
                    scoringPitch = 1;
                    eraseBlockList.Clear();
                    setBlockList.Clear();
                    eraseBlockList.Clear();

                    EraseBlock();
                    if (isBlock)
                    {
                        StartCoroutine(ProcessEraseAndSetBlock());
                    }
                }
            }
        }
    }

    private void MakeWave()
    {
        for (int i = 0; i < 7; i++)
        {
            nextID[i] = UnityEngine.Random.Range(0, 7);
        }
    }

    private void SpawnWave()
    {
        for (int i = 0; i < 7; i++)
        {
            spawnPos.x = i;
            tilemap.SetTile(spawnPos, tiles[nextID[i]]);
        }
    }

    private void MoveWave()
    {
        for (int i = 0; i < 7; i++)
        {
            for (int j = 10; j >= 0; j--)
            {
                Vector3Int checkPos = new Vector3Int(i, j, 0);
                Tile saveTile = tilemap.GetTile(checkPos) as Tile;
                if (saveTile != null)
                {
                    if (checkPos.y == 10)
                    {
                        GameOver();
                        return;
                    }
                    tilemap.SetTile(checkPos, null);
                    checkPos.y += 1;
                    tilemap.SetTile(checkPos, saveTile);
                }
            }
        }
    }

    private KeyValuePair<int, int> CheckTypeBlock(Tile tile)
    {
        for (int i = 0; i < 7; i++)
        {
            if (tile == tiles[i])
            {
                return new KeyValuePair<int, int>(1, i);
            }
        }
        for (int i = 0; i < 7; i++)
        {
            if (tile == tiles2[i])
            {
                return new KeyValuePair<int, int>(2, i);
            }
        }
        for (int i = 0; i < 7; i++)
        {
            if (tile == tiles3[i])
            {
                return new KeyValuePair<int, int>(3, i);
            }
        }
        return new KeyValuePair<int, int>(-1, -1);
    }

    private void SetEffectBlock(Vector3Int pos, int color, int cnt)
    {
        if (cnt >= 3)
        {
            tilemap.SetTile(pos, tiles3[color]);
        }
        else if (cnt >= 2)
        {
            tilemap.SetTile(pos, tiles2[color]);
        }
    }

    private void EraseBlock()
    {
        usedEff = false;

        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int erasePos = tilemap.WorldToCell(mouseWorldPos);
        erasePos.z = 0;

        if (erasePos.y >= 0 && erasePos.y < 11 && erasePos.x >= 0 && erasePos.x < 7 && tilemap.GetTile(erasePos) != null)
        {
            isBlock = true;
            Tile eraseTile = tilemap.GetTile(erasePos) as Tile;
            KeyValuePair<int, int> tmp = CheckTypeBlock(eraseTile);
            int color = tmp.Value;
            int type = tmp.Key;
            int eraseAmount = EraseAdjBlock(erasePos, color);

            scoreArray[color] += eraseAmount;

            if (usedEff == false)
            {
                /*SetEffectBlock(erasePos, color, eraseAmount);*/
                setBlockList.Add(new Tuple<Vector3Int, int, int>(erasePos, color, eraseAmount));
            }

        }
        else
        {
            canClick = true;
        }
    }

    private int EraseAdjBlock(Vector3Int erasePos, int color)
    {
        int cnt = 0;
        Queue<Vector3Int> q = new Queue<Vector3Int>();
        q.Enqueue(erasePos);
        KeyValuePair<int, int> tmp;
        Vector3Int checkPos, topQueue;
        Tile checkTile;
        while (q.Count > 0)
        {
            topQueue = q.Dequeue();
            if (eraseBlockList.Contains(topQueue)) continue;
            checkTile = tilemap.GetTile(topQueue) as Tile;
            if (checkTile == null) continue;
            tmp = CheckTypeBlock(checkTile);

            eraseBlockList.Add(topQueue);
            /*tilemap.SetTile(topQueue, null);
            ScoringSound();*/

            if (tmp.Key == 1)
            {
                cnt++;
            }
            else if (tmp.Key == 2)
            {
                usedEff = true;
                ++cnt;
                cnt += EraseLine(topQueue, color);
            }
            else if (tmp.Key == 3)
            {
                usedEff = true;
                ++cnt;
                int sameColorAmount = EraseSameColor(erasePos, color);
                cnt += sameColorAmount;
            }
            for (int k = 0; k < 4; k++)
            {
                int _x = topQueue.x + dx[k];
                int _y = topQueue.y + dy[k];
                if (_x < 0 || _x >= 7 || _y < 0 || _y >= 11) continue;
                checkPos = new Vector3Int(_x, _y, 0);
                if (eraseBlockList.Contains(checkPos)) continue;
                checkTile = tilemap.GetTile(checkPos) as Tile;
                if (checkTile != null)
                {
                    tmp = CheckTypeBlock(checkTile);
                    if (tmp.Value == color)
                    {
                        q.Enqueue(checkPos);
                    }
                }
            }
        }
        return cnt;
    }

    private int EraseLine(Vector3Int erasePos, int color)
    {
        KeyValuePair<int, int> tmp = new KeyValuePair<int, int>(0, 0);
        Tile checkTile = null;
        Vector3Int checkPos;
        int cnt = 0;
        for (int i = 0; i < 7; i++)
        {
            checkPos = new Vector3Int(i, erasePos.y, 0);
            if (eraseBlockList.Contains(checkPos)) continue;
            checkTile = tilemap.GetTile(checkPos) as Tile;
            if (checkTile != null)
            {
                eraseBlockList.Add(checkPos);
                /*tilemap.SetTile(checkPos, null);
                ScoringSound();*/

                tmp = CheckTypeBlock(checkTile);
                if (tmp.Key == 3)
                {
                    int sameColorAmout = EraseSameColor(checkPos, tmp.Value);
                    ++sameColorAmout;
                    if (tmp.Value == color)
                    {
                        cnt += sameColorAmout;
                    }
                    else
                    {
                        scoreArray[tmp.Value] += sameColorAmout;
                    }
                    setBlockList.Add(new Tuple<Vector3Int, int, int>(checkPos, tmp.Value, sameColorAmout));
                    /*SetEffectBlock(checkPos, tmp.Value, sameColorAmout);*/
                }
                else
                {
                    if (tmp.Value == color)
                    {
                        cnt++;
                    }
                    else
                    {
                        scoreArray[tmp.Value]++;
                    }
                }
            }
        }
        return cnt;
    }

    private int EraseSameColor(Vector3Int erasePos, int color)
    {
        int cnt = 0;
        Tile checkTile = null;
        Vector3Int checkPos;
        KeyValuePair<int, int> tmp = new KeyValuePair<int, int>(0, 0);
        for (int i = 0; i < 7; i++)
        {
            for (int j = 0; j < 11; j++)
            {
                checkPos = new Vector3Int(i, j, 0);
                if (eraseBlockList.Contains(checkPos)) continue;
                checkTile = tilemap.GetTile(checkPos) as Tile;
                tmp = CheckTypeBlock(checkTile);
                if (checkTile != null && tmp.Value == color)
                {
                    ++cnt;
                    eraseBlockList.Add(checkPos);
                    /*tilemap.SetTile(checkPos, null);
                    ScoringSound();*/
                }
            }
        }
        return cnt;
    }

    private IEnumerator ProcessEraseAndSetBlock()
    {
        foreach (Vector3Int pos in eraseBlockList)
        {
            tilemap.SetTile(pos, null);
            ScoringSound();
            yield return new WaitForSeconds(scoringDuration);
        }
        foreach (Tuple<Vector3Int, int, int> i in setBlockList)
        {
            SetEffectBlock(i.Item1, i.Item2, i.Item3);
        }

        if (isFreeze)
        {
            --freezeWaves;
            if (freezeWaves == 0)
            {
                isFreeze = false;
                next.FreezeBlock(isFreeze);
                freezeWaves = 2;
            }
        }
        if (scoreArray[1] >= freezeCnt * 10)
        {
            isFreeze = true;
            freezeCnt = scoreArray[1] / 10 + 1;
            AudioManager.FreezingSound();
            next.FreezeBlock(isFreeze);
        }

        StartCoroutine(DropBlockCoroutine());
    }

    private IEnumerator DropBlockCoroutine()
    {
        bool anyBlockDropped;
        do
        {
            anyBlockDropped = false;
            for (int i = 0; i < 7; i++)
            {
                for (int j = 1; j < 11; j++)
                {
                    Vector3Int checkPos = new Vector3Int(i, j, 0);
                    Tile saveTile = tilemap.GetTile(checkPos) as Tile;
                    if (saveTile != null)
                    {
                        Vector3Int belowPos = new Vector3Int(i, j - 1, 0);
                        if (tilemap.GetTile(belowPos) == null)
                        {
                            yield return StartCoroutine(MoveTileCoroutine(checkPos, belowPos, saveTile));
                            anyBlockDropped = true;
                        }
                    }
                }
            }
        } while (anyBlockDropped);

        score.ChangeScore(scoreArray);
        if (!isFreeze)
        {
            yield return new WaitForSeconds(0.1f);
            MoveWave();
            yield return new WaitForSeconds(0.1f);
            SpawnWave();
            MakeWave();
            next.SpawnNext(nextID);
        }
        canClick = true;
    }

    private IEnumerator MoveTileCoroutine(Vector3Int fromPos, Vector3Int toPos, Tile tile)
    {
        float elapsedTime = 0f;
        float duration = 0.05f;
        Vector3 fromWorldPos = tilemap.CellToWorld(fromPos);
        Vector3 toWorldPos = tilemap.CellToWorld(toPos);

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            Vector3 currentPos = Vector3.Lerp(fromWorldPos, toWorldPos, t);
            tilemap.SetTile(tilemap.WorldToCell(currentPos), tile);
            yield return null;
            elapsedTime += Time.deltaTime;
            tilemap.SetTile(tilemap.WorldToCell(currentPos), null);
        }

        tilemap.SetTile(toPos, tile);
    }

    private void GameOver()
    {
        AudioManager.GameOverSound();
        SceneManager.LoadScene("GameOver");
    }

    private void ScoringSound()
    {
        AudioManager.Scoring(scoringPitch);
        scoringPitch += 0.1f;
    }

}