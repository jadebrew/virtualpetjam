using Audio;
using Needs;
using Notification;

namespace RandomEvents
{
    public class PetTurnsRadioOff : IRandomEvent
    {
        public void Happen()
        {
            if (!NeedsManager.Instance.IsPetSleepy()) return;
            
            Petmanager.Instance.MovePetToRadio();
            NotificationManager.Instance.NotifyPetTurnedOffRadio();
            StereoManager.Instance.Pause();
        }
    }
}