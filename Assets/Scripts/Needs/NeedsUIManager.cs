using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Needs
{
    public class NeedsUIManager : MonoBehaviour
    {
        public static NeedsUIManager Instance { get; private set; }

        [SerializeField] public RectTransform hungerBar;
        [SerializeField] public RectTransform sleepBar;
        [SerializeField] public RectTransform hygieneBar;
        
        private const float FullBarWidth = 26.0f;
        
        private Coroutine _activeIconCoroutine;
        
        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            
            Instance = this;
        }

        public void UpdateScoresVisually(Dictionary<NeedType, Need> needs)
        {
            foreach (var (needType, need) in needs)
            {
                switch (needType)
                {
                    case NeedType.Sleep:
                        sleepBar.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, need.value/need.max * FullBarWidth);
                        break;
                    case NeedType.Hygiene:
                        hygieneBar.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, need.value/need.max * FullBarWidth);
                        break;
                    case NeedType.Hunger:
                        hungerBar.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, need.value/need.max * FullBarWidth);
                        break;
                    default:
                        Debug.LogWarning("NeedType not found");
                        break;
                }
            }
        }
        
        public void DisplayPetIcon(NeedType needType, float lengthSeconds = 0)
        {
            //to avoid race conditions
            if (_activeIconCoroutine != null)
                return;

            _activeIconCoroutine = StartCoroutine(ShowPetIconRoutine(needType, lengthSeconds));
        }

        private IEnumerator ShowPetIconRoutine(NeedType needType, float lengthSeconds = 0)//0 = until turned off
        {
            switch (needType)
            {
                case NeedType.Sleep:
                    Petmanager.Instance.SetIcon("zzz");
                    break;
                case NeedType.Hygiene:
                    Petmanager.Instance.SetIcon("stink");
                    break;
                case NeedType.Hunger:
                    Petmanager.Instance.SetIcon("hungry");
                    break;
                case NeedType.None:
                    Petmanager.Instance.SetIcon("nothing");
                    break;
                default:
                    Debug.LogWarning("NeedType not found");
                    break;
            }

            if (!(lengthSeconds > 0)) yield break;
            
            yield return new WaitForSeconds(lengthSeconds);
            Petmanager.Instance.SetIcon("nothing");
            _activeIconCoroutine = null;
        }
    }
}
