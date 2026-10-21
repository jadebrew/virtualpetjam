using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    
    [SerializeField] public GameObject notificationMessage;
    [SerializeField] public TextMeshProUGUI notificationText;
    [SerializeField] public Image NotificationImage;
    
    private bool visible = false; //notification is visible
    private bool idle = true; //notification allows new messages
    private float rootY = 68; //rootY is the Y value notification window will rest when hidden
    private float showY = -16; //showY is the Y value from rootY to slide to
    private float timer = 0; // time since shown
    private float showDelay = 10; // total time to show notificationpublic Sprite sprite;
    private float slideSpeed = 20;
    private string lastMsg; // lastMsg is just to check if the notification is new in Show()

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void Show()
    {
        if (notificationText.text != lastMsg)
        {
            visible = true;
            idle = false;
            lastMsg = notificationText.text;
            timer = 0;
        }
    }

    public bool isIdle()
    {
        return idle;
    }

    void Update()
    {
        float posY = notificationMessage.transform.localPosition.y;
        float newY = posY;
        timer += Time.deltaTime;
        if (visible)
        {
            idle = false;
            if(posY > rootY + showY)
            {
                newY -= slideSpeed * Time.deltaTime;
            } else {
                visible = false;
            }
        } else if (timer > showDelay) {
            newY += slideSpeed * Time.deltaTime;
            if (newY > rootY)
            {
                newY = rootY;
                idle = true;
            }
        }
    notificationMessage.transform.localPosition = new Vector3(notificationMessage.transform.localPosition.x,newY,notificationMessage.transform.localPosition.z);
    }
}
