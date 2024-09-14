using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NPCController : MonoBehaviour
{
    [HideInInspector] public bool isSeenPlayer = false;
    public Transform playerTransform;
    public Transform npcTransform;
    public GameObject talkBt;
    private float direction = 1;
    // Start is called before the first frame update
    void Start()
    {
        talkBt.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateDirection();
    }
    void UpdateDirection()
    {
        if (isSeenPlayer && playerTransform != null)
        {
            direction = playerTransform.position.x < transform.position.x ? -1f : 1f;
        }
        else StartCoroutine(ChangeDirection());
        Vector3 newScale = transform.localScale;
        newScale.x = Mathf.Abs(newScale.x) * direction; // Giữ nguyên chiều hướng mà không lật
        transform.localScale = newScale;
    }
    IEnumerator ChangeDirection()
    {
        yield return new WaitForSeconds(1f);
        direction = 1;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("-------ISeeeeeeeuuuuuuuuuuuuuuu------");
            isSeenPlayer = true;
            playerTransform = other.transform;
            talkBt.SetActive(true);

        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        isSeenPlayer = false;
        talkBt.SetActive(false); 
    }
}
