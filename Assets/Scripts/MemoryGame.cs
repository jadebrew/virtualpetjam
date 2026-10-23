using Audio;
using UnityEngine;
using static SFXTag;

public class MemoryGame : MonoBehaviour
{
    public GameObject player;
    public GameObject[] cells;
    public float speed;
    private bool gameOver = false;
    private int caught = 0;
    public GameObject gameScene;
    private AudioManager _audio;
    SFXTag soundEffect;

    private Item[] collectables;

    public void Init(SFXTag useSFX)
    {
        gameOver = false;
        caught = 0;
        gameScene.SetActive(true);
        soundEffect = useSFX;

        _audio = AudioManager.Instance;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach (GameObject cell in cells)
        {

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Set()
    {

    }
}
