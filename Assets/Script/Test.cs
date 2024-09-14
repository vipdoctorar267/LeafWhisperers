using UnityEngine;
using Newtonsoft.Json;

public class Test : MonoBehaviour
{
    void Start()
    {
        var example = new { Name = "Test", Value = 123 };
        string json = JsonConvert.SerializeObject(example);
        Debug.Log("Serialized JSON: " + json);
    }
}
