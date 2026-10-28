using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Serialization;

namespace Needs
{
    public class NeedsUIManager : MonoBehaviour
    {
        public static NeedsUIManager Instance { get; private set; }
        
        [SerializeField] public TextMeshProUGUI sleepScoreText;
        [SerializeField] public TextMeshProUGUI hygieneScoreText;
        [SerializeField] public TextMeshProUGUI hungerScoreText;
        
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
            //foreach (var (needType, need) in needs)
            //{
            //    var visualValue = (int) Math.Round(need.value, 0);
            //
            //    switch (needType)
           //     {
            //        case NeedType.Sleep:
             //           sleepScoreText.text = $"{visualValue}";
            //            break;
             //       case NeedType.Hygiene:
			//		if(hygieneScoreText != null)
              //              hygieneScoreText.text = $"{visualValue}";
                      //  break;
              //      case NeedType.Hunger:
                       // hungerScoreText.text = $"{visualValue}";
              //          break;
              //      default:
               //         Debug.LogWarning("NeedType not found");
               //         break;
               // }
           // }
        }
    }
}
