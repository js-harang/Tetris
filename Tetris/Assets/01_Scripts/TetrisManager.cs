using AYellowpaper.SerializedCollections;
using System;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public enum TetrominoType : byte
{
    None,
    I,
    O,
    Z,
    S,
    J,
    L,
    T,
    Max
}

public class TetrisManager : Singleton<TetrisManager>
{
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private SerializedDictionary<TetrominoType, string> tetrominoDatas;
    [SerializeField] private float dropTime = 1;
    [SerializeField] private float leftLimitPosition = -5;
    [SerializeField] private float rightLimitPosition = 5;
    [SerializeField] private float bottomLimitPosition = -9;

    private TetrominoData currentTetrominoData;
    private float currentDropTime = 0;

    private int[][] grid = null;

    public override void OnAwake()
    {
        base.OnAwake();

        grid = new int[25][];

        for (int i = 0; i < grid.Length; i++)
        {
            grid[i] = new int[10];
        }
    }

    private void Start()
    {
        currentDropTime = dropTime;
        SpawnTetromino();
    }

    private void Update()
    {
        if (currentTetrominoData.IsUnityNull()) return;

        if (Input.GetKeyDown(KeyCode.A))
        {
            SetGridState(0);

            currentTetrominoData.transform.position += Vector3.left;

            SetGridState(1);

            if (CheckBlockCollision())
            {
                SetGridState(0);

                currentTetrominoData.transform.position -= Vector3.left;

                SetGridState(1);
            }
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            SetGridState(0);

            currentTetrominoData.transform.position += Vector3.right;

            SetGridState(1);

            if (CheckBlockCollision())
            {
                SetGridState(0);

                currentTetrominoData.transform.position -= Vector3.right;

                SetGridState(1);
            }
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            SetGridState(0);

            currentTetrominoData.transform.Rotate(new Vector3(0, 0, -90));

            SetGridState(1);

            if (CheckBlockCollision())
            {
                SetGridState(0);

                currentTetrominoData.transform.Rotate(new Vector3(0, 0, 90));

                SetGridState(1);
            }
        }

        currentDropTime -= Time.deltaTime;

        if (currentDropTime <= 0)
        {
            SetGridState(0);

            currentTetrominoData.transform.position += Vector3.down;


            if (GridOverlapCheck())
            {
                currentTetrominoData.transform.position -= Vector3.down;

                SetGridState(1);
                SpawnTetromino();
            }
            else
            {
                SetGridState(1);

                var (minX, minY, maxX, maxY) = GetGridState();

                if (minY == -1)
                {
                    SetGridState(0);

                    currentTetrominoData.transform.position -= Vector3.down;

                    SetGridState(1);
                    SpawnTetromino();
                }
            }

            currentDropTime = dropTime;
        }
    }

    private (int, int, int, int) GetGridState()
    {
        int minX = Int32.MaxValue;
        int minY = Int32.MaxValue;
        int maxX = Int32.MinValue;
        int maxY = Int32.MinValue;

        foreach (var block in currentTetrominoData.Blocks)
        {
            int x = (int)(block.transform.position.x + 5);
            int y = (int)(block.transform.position.y + 9);
            minX = Mathf.Min(minX, x);
            minY = Mathf.Min(minY, y);
            maxX = Mathf.Max(maxX, x);
            maxY = Mathf.Max(maxY, y);
        }

        return (minX, maxY, minX, maxX);
    }

    private bool GridOverlapCheck()
    {
        foreach (var block in currentTetrominoData.Blocks)
        {
            int y = (int)(block.transform.position.y + 9);
            int x = (int)(block.transform.position.x + 5);

            if (y >= 0 && x >= 0 && grid[y][x] == 1) return true;
        }

        return false;
    }

    private void SetGridState(int state)
    {
        foreach (var block in currentTetrominoData.Blocks)
        {
            int y = (int)(block.transform.position.y + 9);
            int x = (int)(block.transform.position.x + 5);

            if (y >= 0 && x >= 0)
                grid[y][x] = state;
        }
    }

    private void SpawnTetromino()
    {
        TetrominoType nextBlockIndex = (TetrominoType)Random.Range(0, 7) + 1;
        GameObject tetromino_Prefab = Resources.Load<GameObject>($"Block_Prefabs/{tetrominoDatas[nextBlockIndex]}");

        GameObject spawndTetromino = Instantiate(tetromino_Prefab, spawnPoint.position, Quaternion.identity);
        spawndTetromino.TryGetComponent(out currentTetrominoData);
    }

    private bool CheckBlockCollision()
    {
        foreach (var block in currentTetrominoData.Blocks)
        {
            int y = (int)(block.transform.position.y + 9);
            int x = (int)(block.transform.position.x + 5);

            if (x >= 0 && x < grid[0].Length) return false;
        }

        return false;
    }

    private bool CheckBlockFinishMove()
    {
        foreach (var block in currentTetrominoData.Blocks)
        {
            if (block.position.y < bottomLimitPosition) return true;
        }

        return false;
    }
}
