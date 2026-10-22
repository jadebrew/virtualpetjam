using UnityEngine;

public class Settings
{
    public static string PetName = "Mimi";
    
    // Optional: Resets static state when entering Play Mode in Unity
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStaticData()
    {
        PetName = "Mimi";
    }
}