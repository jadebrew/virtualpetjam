using UnityEngine;
using UnityEngine.EventSystems;

namespace Needs
{
    public class NeedsBarHelper : MonoBehaviour, 
        IPointerEnterHandler, 
        IPointerExitHandler, 
        IPointerClickHandler
    {
        public NeedType needsType;
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            NeedsUIManager.Instance.DisplayPetIcon(needsType);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            NeedsUIManager.Instance.DisplayPetIcon(NeedType.None);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            NeedsUIManager.Instance.DisplayPetIcon(needsType, 1);
        }
    }
}