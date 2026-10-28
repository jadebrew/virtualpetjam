using Audio;
using Needs;
using Notification;

namespace RandomEvents
{
    public class PetGoesToLitterBox : IRandomEvent
    {
        public void Happen()
        {
            if (!NeedsManager.Instance.IsPetDirty()) return;
            
            Petmanager.Instance.MovePetToLitterBox();
            NotificationManager.Instance.NotifyPetGoesToLitterBox();
        }
    }
}