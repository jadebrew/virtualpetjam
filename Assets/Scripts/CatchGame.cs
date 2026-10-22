using UnityEngine;
using static SFXTag;
using TMPro;

public class CatchGame : MonoBehaviour
{
    public GameObject fallingItem;
    public GameObject player;
    public float[] positions;
    public float speed;
    private bool gameOver = false;

    public TMP_Text verdictText;
    public TMP_Text scoreText;

    private int caught = 0;
    public GameObject catchScene;
    private AudioManager _audio;
    SFXTag soundEffect;

    private float patience = 0;
    private float maxPatience = 2;
    private float cooldown = 0;

    private int score = 0;

    public void Init(SFXTag useSFX)
    {
        gameOver = false;
        caught = 0;
        catchScene.SetActive(true);
        soundEffect = useSFX;
        _audio = AudioManager.Instance;
        verdictText.text = "Catch!";
        score = 0;
        Run();

    }
    void Run()
    {
        var newPos = player.transform.localPosition;
        Vector3 pos = fallingItem.transform.localPosition;
        newPos.x = positions[(int)Mathf.Round(UnityEngine.Random.Range(0,positions.Length))];
        while (newPos.x == pos.x)
        {
            newPos.x = positions[(int)Mathf.Round(UnityEngine.Random.Range(0,positions.Length))];
        }
         player.transform.localPosition = newPos;
         patience = 0;
         cooldown = 0;
    }
    // Update is called once per frame
    void Update()
    {
        if(!gameOver)
        {
            patience += Time.deltaTime;
            cooldown = Mathf.Max(0,cooldown-Time.deltaTime);
            if (patience>maxPatience)
            {
                Run();
            }

            Vector3 pos = fallingItem.transform.localPosition;
            pos.y -= Time.deltaTime * speed;
            if(pos.y<-12)
            {
                caught++;
                if (pos.x == player.transform.localPosition.x)
                {
                    string verdict = "Oof";
                    if (cooldown < 0.5)
                    {
                        verdict = "Too late!";
                    } else if (cooldown < 0.75)
                    {
                        verdict = "Okay!";
                        score += 25;
                    } else if (cooldown < 0.80)
                    {
                        verdict = "Good!";
                        score += 50;
                    }else if (cooldown < 0.9)
                    {
                        verdict = "Amazing!";
                        score += 75;
                    }else
                    {
                        verdict = "Perfect!";
                        score += 100;
                    }
                    verdictText.text = verdict;

                    _audio.Play(Eat);
                    if (caught >= 10)
                    {
                        gameOver = true;
                        catchScene.SetActive(false);
                    }
                } else {
                    _audio.Play(Annoyed);
                    verdictText.text = "Miss!";
                    Run();
                }
                scoreText.text = "Score " + score;

                pos.y = 60;
                pos.x = positions[(int)Mathf.Round(UnityEngine.Random.Range(0,positions.Length))];
            }
            fallingItem.transform.localPosition = pos;
        }
    }

    public void SetPosition(int position)
    {
        if(cooldown <= 0)
        {
            Vector3 pos = player.transform.localPosition;
            pos.x = positions[position];
            player.transform.localPosition = pos;
            patience = 0;
            cooldown = 1;
        } else {
            _audio.Play(Annoyed);
        }
    }
}
