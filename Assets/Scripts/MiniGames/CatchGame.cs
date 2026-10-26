using Audio;
using TMPro;
using UnityEngine;
using static SFXTag;

namespace MiniGames
{
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

        private float patience = 0;
        private float maxPatience = 2;
        private float cooldown = 0;

        private int score = 0;

        private int randomCoin = 5;

        //TODO once catchgame is only used for hunger you dont need to pass the sfx
        public void Init()
        {
            gameOver = false;
            caught = 0;
            catchScene.SetActive(true);
            _audio = AudioManager.Instance;
            verdictText.text = "Wait!";
            score = 0;
            Run();

        }
        void Run()
        {
            var newPos = player.transform.localPosition;
            Vector3 pos = fallingItem.transform.localPosition;
            //newPos.x = positions[(int)Mathf.Round(UnityEngine.Random.Range(0,positions.Length))];
            while (newPos.x == pos.x)
            {
                newPos.x = positions[(int)Mathf.Round(UnityEngine.Random.Range(0,positions.Length))];
                verdictText.text = "Not yet!";
            }
            player.transform.localPosition = newPos;
            patience = 0;
            cooldown = 0;
        }
        // Update is called once per frame
        void Update()
        {
            var bottom_y = -12;
            var top_y = 60;

            if(!gameOver)
            {
                string verdict = "Oof";
                patience += Time.deltaTime;
                cooldown = Mathf.Max(0,cooldown-Time.deltaTime);
                if (patience>maxPatience)
                {
                    Run();
                }

                Vector3 pos = fallingItem.transform.localPosition;
                pos.y -= Time.deltaTime * speed;

                var current_y = (pos.y - bottom_y);
                if (current_y<30)
                {
                    verdictText.text = "Now!";

                } else if (current_y<40) {
                    Run();
                }
                if(pos.y<bottom_y)
                {
                    caught++;
                    AudioManager.Instance.Play(Wash);
                    if (pos.x == player.transform.localPosition.x)
                    {
                        if (cooldown < 0.5)
                        {
                            verdict = "Too Soon!";
                        } else if (cooldown < 0.6)
                        {
                            verdict = "Okay!";
                            score += 25;
                        } else if (cooldown < 0.75)
                        {
                            verdict = "Perfect!";
                            score += 100;
                        }else if (cooldown < 0.8)
                        {
                            verdict = "Amazing!";
                            score += 50;
                        }else
                        {
                            verdict = "Okay!";
                            score += 25;
                        }
                        verdictText.text = verdict;

                        _audio.Play(Wash);
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

                    pos.y = top_y;
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
}
