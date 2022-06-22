using UnityEngine;

namespace CardGrid
{
    public class DebugSystem
    {
        public static CommonGameSettings.DebugSettings Settings = new CommonGameSettings.DebugSettings();

        public enum Type
        {
            SaveSystem,
            PlayerInput,
            Battle,
            Error
        }
        
        public static void DebugLog(string log, Type type)
        {
            foreach (var channel in Settings.DebugsChannels)
            {
                if (channel.Type == type && channel.Active)
                {
                    if (type == Type.Error)
                    {
                        Debug.LogError(log);
                    }
                    else
                    {
                        Debug.Log(log);
                    }
                }
            }
        }
    }
}