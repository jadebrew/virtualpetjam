using Audio;
using Needs;
using Notification;
using UnityEngine;

namespace RandomEvents
{
    public class PetGoesAngry : IRandomEvent
    {
        public void Happen()
        {
            if (NeedsManager.Instance.IsPetHappy) return;
            Petmanager.Instance.ForceNeed("angry");
        }
    }
}
