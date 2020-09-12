using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public GameObject pipePrefab;
    public GameObject hoopPrefab;
    public GameObject trapPrefab;
    public GameObject finishPrefab;
    public GameObject stripePrefab;

    public Queue<GameObject> pipePool;
    public Queue<GameObject> stripePool;
    public Coroutine currentLevelCoroutine;
    public Coroutine endLevelCoroutine;
    public Coroutine finishLevelCoroutine;

    GameObject currentPipe;
    GameObject lastPipe;

    public Level curLevel;
    public Player player;

    public static LevelManager instance;

    public int levelCount;

    private void Start()
    {
        instance = this;

        pipePool = new Queue<GameObject>();
        stripePool = new Queue<GameObject>();
    }

    public void StartLevel()
    {
        if(curLevel != null)
        {
            Destroy(curLevel.gameObject);
            Destroy(player.gameObject);
            pipePool = new Queue<GameObject>();
            stripePool = new Queue<GameObject>();
            StopAllCoroutines();
        }
        curLevel = new GameObject("Level").AddComponent<Level>();
        curLevel.levelRequirement = (UIManager.instance.levelCount*5) + 20;
        player = Instantiate(hoopPrefab, Vector3.up*3, Quaternion.identity).GetComponent<Player>();
        lastPipe = Instantiate(pipePrefab, Vector3.zero, pipePrefab.transform.rotation, curLevel.gameObject.transform);
        pipePool.Enqueue(lastPipe);
        currentLevelCoroutine = StartCoroutine(InitLevel());
    }

    

    public IEnumerator InitLevel()
    {
        while (true)
        {
            int zScale = Random.Range(3, 5);
            int scale = Random.Range(3, 6);
            if(pipePool.Count<10)
            {
                GameObject currentPipe = Instantiate(pipePrefab,
                                                        new Vector3(lastPipe.transform.position.x,
                                                                    lastPipe.transform.localScale.z + zScale + lastPipe.transform.position.y,
                                                                    lastPipe.transform.position.z),
                                                        pipePrefab.transform.rotation,
                                                        curLevel.gameObject.transform);

                currentPipe.transform.localScale = new Vector3(0.3f * scale, 0.3f * scale, zScale);
                pipePool.Enqueue(currentPipe);
                lastPipe = currentPipe;
                ArrangeLevelElements(currentPipe, (currentPipe.transform.localScale.z - 1) * 2);
                yield return new WaitForSeconds(0.1f);
            }
            else {
                if (pipePool.Peek().transform.position.y < -15)
                {
                    currentPipe = pipePool.Dequeue();
                    for (int i = 0; i < currentPipe.transform.childCount; i++)
                    {
                        Destroy(currentPipe.transform.GetChild(i).gameObject);
                    }
                    currentPipe.transform.position = new Vector3(lastPipe.transform.position.x,
                                                                        lastPipe.transform.localScale.z + zScale + lastPipe.transform.position.y,
                                                                        lastPipe.transform.position.z);

                    currentPipe.transform.localScale = new Vector3(0.3f * scale, 0.3f * scale, zScale);
                    pipePool.Enqueue(currentPipe);
                    lastPipe = currentPipe;

                    ArrangeLevelElements(currentPipe, (currentPipe.transform.localScale.z - 1) * 2);
                }
                yield return new WaitForSeconds(0.4f);

            }
            
            
        }

    }

    public void ArrangeLevelElements(GameObject pipe, float remainingArea)
    {
        if (remainingArea > 0.6f)
        {
            if (remainingArea > 4 )
            {
                int zscale = Random.Range(3, 5);
                GameObject trap = Instantiate(trapPrefab, new Vector3(pipe.transform.position.x, pipe.transform.position.y - remainingArea/2 + zscale / pipe.transform.localScale.y, pipe.transform.position.z), trapPrefab.transform.rotation);
                trap.transform.localScale = new Vector3(pipe.transform.localScale.x*2+0.3f, pipe.transform.localScale.y*2 + 0.3f, zscale/pipe.transform.localScale.z);
                trap.transform.parent = pipe.transform;
                ArrangeLevelElements(pipe, remainingArea - (zscale * 2 ));
            }
            else
            {
                if(remainingArea < 3)
                {
                    return;
                }
                GameObject currentStripe;
                for (float i = remainingArea/2-0.5f; i > -remainingArea/2; i -= 0.6f)
                {
                    currentStripe = Instantiate(stripePrefab, new Vector3(pipe.transform.position.x, pipe.transform.position.y -i ,  pipe.transform.position.z), Quaternion.identity,curLevel.transform);
                    currentStripe.transform.localScale = new Vector3(pipe.transform.localScale.x, stripePrefab.transform.localScale.y , pipe.transform.localScale.y );
                    currentStripe.transform.parent = pipe.transform;
                }

                if(Random.Range(0,3)%2 == 1 && GameManager.instance.currentGameState != GameManager.GameState.Idle)
                {
                    GameObject trap = Instantiate(trapPrefab, new Vector3(pipe.transform.position.x, pipe.transform.position.y + Random.Range(-2f,2f), pipe.transform.position.z), trapPrefab.transform.rotation);
                    trap.transform.localScale = new Vector3(player.defaultScale.x * 2 + 0.3f, player.defaultScale.z * 2 + 0.3f, 1f);
                    trap.transform.parent = pipe.transform;
                }
            }
        }
        else
        {
            return;
        }
    }
    public void EndLevel()
    {
        StopCoroutine(currentLevelCoroutine);
        currentPipe = pipePool.Dequeue();
        for (int i = 0; i < currentPipe.transform.childCount; i++)
        {
            Destroy(currentPipe.transform.GetChild(i).gameObject);
        }
        currentPipe.transform.position = new Vector3(lastPipe.transform.position.x,
                                                            lastPipe.transform.localScale.z + 4 + lastPipe.transform.position.y,
                                                            lastPipe.transform.position.z);
        currentPipe.transform.localScale = new Vector3(0.3f * 5, 0.3f * 5, 4);

        Instantiate(finishPrefab, currentPipe.transform.position, finishPrefab.transform.rotation, curLevel.transform);
        pipePool.Enqueue(currentPipe);
        lastPipe = currentPipe;
        endLevelCoroutine = StartCoroutine(EndLevelCor());
    }
    public IEnumerator EndLevelCor()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.6f);
            if(pipePool.Peek().transform.position.y < -15)
            {

                currentPipe = pipePool.Dequeue();
                for (int i = 0; i < currentPipe.transform.childCount; i++)
                {
                    Destroy(currentPipe.transform.GetChild(i).gameObject);
                }
                currentPipe.transform.position = new Vector3(lastPipe.transform.position.x,
                                                                    lastPipe.transform.localScale.z + 4 + lastPipe.transform.position.y,
                                                                    lastPipe.transform.position.z);

                currentPipe.transform.localScale = new Vector3(0.3f * 5, 0.3f * 5, 4);


                pipePool.Enqueue(currentPipe);
                lastPipe = currentPipe;

            }
        }
    }
    public void ArrangeEndGamePipes(GameObject pipe)
    {
        currentPipe = pipePool.Dequeue();
        for (int i = 0; i < currentPipe.transform.childCount; i++)
        {
            Destroy(currentPipe.transform.GetChild(i).gameObject);
        }
        currentPipe.transform.position = new Vector3(lastPipe.transform.position.x,
                                                            lastPipe.transform.localScale.z + 4 + lastPipe.transform.position.y,
                                                            lastPipe.transform.position.z);

        currentPipe.transform.localScale = new Vector3(0.3f * 5, 0.3f * 5, 4);
        pipePool.Enqueue(currentPipe);
        lastPipe = currentPipe;

        ArrangeEndGamePipes(currentPipe);
    }

    public void ResetLevel()
    {
        //player = Instantiate(hoopPrefab, Vector3.zero, Quaternion.identity).GetComponent<Player>();
        StopAllCoroutines();
        curLevel.levelComplete = true;
    }

    public void WinLevel()
    {
        curLevel.levelComplete = true;
        StopCoroutine(endLevelCoroutine);
        finishLevelCoroutine = StartCoroutine(FinishLevel());
    }
    public IEnumerator FinishLevel()
    {
        UIManager.instance.LevelPassed();
        while(player.transform.position.y < 20f)
        {
            player.transform.position += curLevel.speed;
            yield return new WaitForSeconds(0.03f);
        }
    }
}
