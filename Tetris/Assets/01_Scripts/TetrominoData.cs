using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TetrominoData : MonoBehaviour
{
    private List<Transform> blocks = new();
    public List<Transform> Blocks { get => blocks; }

    private void Start()
    {
        blocks = GetComponentsInChildren<Transform>().ToList();
    }
}
