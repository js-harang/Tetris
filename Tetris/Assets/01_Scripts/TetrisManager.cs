using AYellowpaper.SerializedCollections;
using Unity.VisualScripting;
using UnityEngine;

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
            currentTetrominoData.transform.position += Vector3.left;

            if (CheckBlockCollision()) currentTetrominoData.transform.position -= Vector3.left;
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            currentTetrominoData.transform.position += Vector3.right;

            if (CheckBlockCollision()) currentTetrominoData.transform.position -= Vector3.right;
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            currentTetrominoData.transform.Rotate(new Vector3(0, 0, -90));

            if (CheckBlockCollision()) currentTetrominoData.transform.Rotate(new Vector3(0, 0, 90));
        }

        currentDropTime -= Time.deltaTime;

        if (currentDropTime <= 0)
        {
            currentTetrominoData.transform.position += Vector3.down;

            if (CheckBlockFinishMove())
            {
                currentTetrominoData.transform.position -= Vector3.down;
                SpawnTetromino();
            }

            currentDropTime = dropTime;
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
            if (block.position.x < leftLimitPosition) return true;

            if (block.position.x > rightLimitPosition) return true;
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
