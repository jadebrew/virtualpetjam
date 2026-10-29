using System.Collections.Generic;
using UnityEngine;

namespace RandomEvents
{
    public class RandomEventManager : MonoBehaviour
    {
        public static RandomEventManager Instance { get; private set; }

        private List<IRandomEvent> _eventPool = new();
        private Stack<IRandomEvent> _eventStack = new();

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            
            _eventPool.Add(new PetGoesToLitterBox());
            _eventPool.Add(new PetTurnsRadioOff());
            _eventPool.Add(new PetGoesAngry());
            _eventPool.Add(new PetGoesSad());
        }

        public void RunRandomEvent()
        {
            GetRandomEvent().Happen();
        }

        private IRandomEvent GetRandomEvent()
        {
            if (_eventStack.Count > 0) return _eventStack.Pop();
            
            //refill stack
            _eventPool.Shuffle();
            foreach(var randomEvent in _eventPool)
            {
                _eventStack.Push(randomEvent);
            }
            return _eventStack.Pop();
        }
    }
    

}
