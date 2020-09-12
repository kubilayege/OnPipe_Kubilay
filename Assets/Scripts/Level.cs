using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    public Vector3 speed;
    public int levelRequirement;
    public int currentPoint;
    public bool levelComplete;
    // Start is called before the first frame update
    void Start()
    {
        levelComplete = false;
        speed = new Vector3(0, 0.1f, 0);
        UIManager.instance.UpdateScore(currentPoint, levelRequirement);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(!levelComplete)
            transform.position -= speed;
    }

    public void ScorePoint()
    {
        if(speed.y < 0.27f)
        {
            speed += speed/20f;
        }
        currentPoint++;
        UIManager.instance.UpdateScore(currentPoint, levelRequirement);
        if(currentPoint == levelRequirement)
        {
            GameManager.instance.ChangeGameState(GameManager.GameState.CompleteMission);
        }
    }
}
