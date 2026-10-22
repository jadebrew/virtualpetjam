using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Needs
{
    public class NeedsManager : MonoBehaviour
    {
        public static NeedsManager Instance { get; private set; }
        
        private const float NeedMin = 0;
        private const float NeedMax = 100;

        private Dictionary<NeedType, Need> _needs = new ();
        
        public float NeedDecay { get;  private set; }

        public bool IsPetHappy { get; private set; } = true;
    
        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            
            foreach (var need in Resources.LoadAll<Need>("Needs"))
            {
                _needs[need.type] = need;
            }
        }

        void Update()
        {
            
        }

        public void UpdateNeedDecay(float ticksInAday)
        {
            NeedDecay = NeedMax/ticksInAday;
        }

        public void UpdateNeedAfterItem(Item item)
        {
            var need = _needs[item.needModifier];
            
            need.value += item.needModValue;
            if (need.value > NeedMax)
            {
                need.value = NeedMax;
            }

            NeedsUIManager.Instance?.UpdateScoresVisually(_needs);
        }
        
        public void DecayNeeds()
        {
            IsPetHappy = true;

            foreach (var need in _needs.Select(pair => pair.Value))
            {
                need.value -= NeedDecay;
                Debug.Log("Lost " + NeedDecay + " " + need.title);
                if (!(need.value < NeedMin)) continue;
                
                need.value = NeedMin;
                IsPetHappy = false;
            }
            
            NeedsUIManager.Instance?.UpdateScoresVisually(_needs);
            
            Debug.Log("NeedDecay: " + NeedDecay);//TODO remove
        }

        public void PrintStressTestResults(float gameTickRate, float secondsInDay, float ticksInAday, float temporaryNeed)
        {
            Debug.Log("Full Needs: " + NeedMax);
            Debug.Log("Tick Rate: " + gameTickRate);
            Debug.Log("Seconds in a day: " + secondsInDay);
            Debug.Log("Ticks in a day: " + ticksInAday);
            Debug.Log("Needs in a Tick: " + NeedDecay);
            Debug.Log("Full Needs after 24h neglect: " + Mathf.Round(temporaryNeed));
        }
    }
}
