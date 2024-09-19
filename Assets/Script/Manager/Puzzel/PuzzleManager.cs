using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PuzzleData
{
    public string puzzleId; // ID của câu đố
    public List<BlockData> blocks; // Danh sách các khối trong câu đố
    public float resetTime = 30f; // Thời gian reset sau khi không có tác động
    public bool isSolved = false; // Trạng thái của câu đố
}
public class PuzzleManager : MonoBehaviour
{
    public PuzzleData puzzleData;
    private float interactionTimer;
    private bool puzzleActive = false;

    private void Update()
    {
        if (puzzleActive)
        {
            interactionTimer -= Time.deltaTime;
            if (interactionTimer <= 0f)
            {
                ResetPuzzle();
            }
        }
    }

    public void OnBlockInteraction(BlockManager blockManager)
    {
        if (!puzzleActive)
        {
            puzzleActive = true;
            interactionTimer = puzzleData.resetTime;
        }
        else
        {
            interactionTimer = puzzleData.resetTime;
        }

        UpdateAdjacentBlocks(blockManager);
        CheckPuzzleSolved();
    }

    private void UpdateAdjacentBlocks(BlockManager blockManager)
    {
        // Xác định vị trí của blockManager trong puzzleData.blocks
        int index = puzzleData.blocks.FindIndex(b => b.block == blockManager.gameObject);
        if (index == -1) return;

        // Tăng số đèn cho block hiện tại và các khối bên cạnh
        UpdateBlockState(index);
    }

    private void UpdateBlockState(int index)
    {
        if (index >= 0 && index < puzzleData.blocks.Count)
        {
            var currentBlock = puzzleData.blocks[index];
            BlockManager blockManager = currentBlock.block.GetComponent<BlockManager>();

            if (blockManager.currentState == BlockManager.LightState.ThreeLights)
            {
                blockManager.SetState(BlockManager.LightState.Off);
            }
            else
            {
                blockManager.SetState(blockManager.currentState + 1);
            }

            // Cập nhật trạng thái các khối bên cạnh
            if (index > 0) UpdateAdjacentBlock(index - 1); // Khối trước
            if (index < puzzleData.blocks.Count - 1) UpdateAdjacentBlock(index + 1); // Khối sau
        }
    }

    private void UpdateAdjacentBlock(int index)
    {
        var adjacentBlock = puzzleData.blocks[index];
        BlockManager blockManager = adjacentBlock.block.GetComponent<BlockManager>();

        if (blockManager.currentState == BlockManager.LightState.ThreeLights)
        {
            blockManager.SetState(BlockManager.LightState.Off);
        }
        else
        {
            blockManager.SetState(blockManager.currentState + 1);
        }
    }

    private void ResetPuzzle()
    {
        foreach (var blockData in puzzleData.blocks)
        {
            BlockManager blockManager = blockData.block.GetComponent<BlockManager>();
            blockManager.SetState((BlockManager.LightState)blockData.firstState);
        }
        puzzleActive = false;
    }

    private void CheckPuzzleSolved()
    {
        if (puzzleData.blocks.TrueForAll(b => b.isActive))
        {
            puzzleData.isSolved = true;
            // Logic để xử lý khi câu đố được giải, ví dụ: mở cửa, thưởng
        }
    }
}
