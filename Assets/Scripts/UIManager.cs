using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    
    [SerializeField] public GameObject notificationMessage;
    [SerializeField] public TextMeshProUGUI notificationText;
    [SerializeField] public Image NotificationImage;

    private float timer = 0;
    private float showDelay = 1;
    private float downY = 0;
    private float upY =32;
    private bool visible = false;
    private float lerpSpeed = 0.5f;
    private float rootY;
    
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        rootY = notificationMessage.transform.localPosition.y;
    }

    public void Show()
    {
        visible = true;
    }

    void Update()
    {
        Vector3 nPos = notificationMessage.transform.localPosition;
        float targetY = rootY + nPos.y;
        if (visible)
        {
            targetY = rootY + downY;
            timer += Time.deltaTime;
            if (timer > showDelay)
            {
                visible = false;
                timer = 0;
            }
        } else {
            targetY = rootY + upY;
        }
        notificationMessage.transform.localPosition = new Vector3(nPos.x,Mathf.Lerp(nPos.y, targetY, lerpSpeed * Time.deltaTime),nPos.z);
    }
}
