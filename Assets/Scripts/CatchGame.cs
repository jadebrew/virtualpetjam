using UnityEngine;

using static AudioManager.SFXTitle;
using static AudioManager.SongTitle;

public class CatchGame : MonoBehaviour
{
    public GameObject fallingItem;
    public GameObject player;
    public float[] positions;
    public float speed;
    private bool gameOver = false;
    private int caught = 0;
    public GameObject catchScene;
    private AudioManager _audio;
    AudioManager.SFXTitle soundEffect;

    public void Init(AudioManager.SFXTitle useSFX)
    {
        gameOver = false;
        caught = 0;
        catchScene.SetActive(true);
        soundEffect = useSFX;
        _audio = AudioManager.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        if(!gameOver)
        {
            Vector3 pos = fallingItem.transform.localPosition;
            pos.y -= Time.deltaTime * speed;
            if(pos.y<-12)
            {
                if (pos.x == player.transform.localPosition.x)
                {
                    caught++;
                    _audio.Play(Eat);
                    if (caught >= 10)
                    {
                        gameOver = true;
                        catchScene.SetActive(false);
                    }
                } else {
                    _audio.Play(Annoyed);
                }
                pos.y = 60;
                pos.x = positions[(int)Mathf.Round(UnityEngine.Random.Range(0,positions.Length))];
            }
            fallingItem.transform.localPosition = pos;
        }
    }

    public void SetPosition(int position)
    {
        Vector3 pos = player.transform.localPosition;
        pos.x = positions[position];
        player.transform.localPosition = pos;
    }
}
