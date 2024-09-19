using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class BlockData
{
    public int index; // Số thứ tự của block trong danh sách
    public GameObject block; // Khối (Block) chứa đèn
    public int firstState; // Trạng thái ban đầu của khối
    public bool isActive; // Trạng thái hiện tại (đã sáng hết đèn chưa)
}
public class BlockManager : MonoBehaviour
{
    public enum LightState { Off, OneLight, TwoLights, ThreeLights }

    public GameObject light01;
    public GameObject light02;
    public GameObject light03;

    public LightState currentState;

    private void Start()
    {
        UpdateLights();
    }
    private void FixedUpdate()
    {
        UpdateLights();
    }
    public void SetState(LightState newState)
    {
        currentState = newState;
        
    }

    private void UpdateLights()
    {
        light01.SetActive(currentState >= LightState.OneLight);
        light02.SetActive(currentState >= LightState.TwoLights);
        light03.SetActive(currentState == LightState.ThreeLights);
    }
}
