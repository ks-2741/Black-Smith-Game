using System.Collections;
using UnityEngine;

public class TargetZoneMover : MonoBehaviour
{
    [Header("References")]
    public GrindstoneGameManager gameManager;
    public RectTransform track;


    [Header("Movement")]
    public float moveSpeed = 4f;

    public float minWait = 0.5f;

    public float maxWait = 1.5f;



    private RectTransform rect;

    private Vector2 targetPos;

    private Coroutine moveRoutine;



    void Awake()
    {
        rect = GetComponent<RectTransform>();
    }



    void Start()
    {
        targetPos = rect.anchoredPosition;
    }



    void Update()
    {
        if (gameManager == null)
            return;


        if (!gameManager.IsGrindingActive())
            return;



        rect.anchoredPosition =
            Vector2.Lerp(
                rect.anchoredPosition,
                targetPos,
                moveSpeed * Time.deltaTime);
    }



    public void StartMoving()
    {
        Debug.Log("TargetZoneMover started");


        if (moveRoutine != null)
            StopCoroutine(moveRoutine);



        moveRoutine = StartCoroutine(MoveRoutine());
    }



    public void StopMoving()
    {
        if (moveRoutine != null)
        {
            StopCoroutine(moveRoutine);
            moveRoutine = null;
        }
    }



    IEnumerator MoveRoutine()
    {
        while (true)
        {
            if (track != null)
            {
                float limit =
                    track.rect.width / 2f -
                    rect.rect.width / 2f;



                targetPos = new Vector2(
                    Random.Range(-limit, limit),
                    rect.anchoredPosition.y);
            }


            yield return new WaitForSeconds(
                Random.Range(minWait, maxWait));
        }
    }
}