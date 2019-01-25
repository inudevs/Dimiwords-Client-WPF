using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using static Dimiwords_Client_WPF.DiscordRpc;

namespace Dimiwords_Client_WPF
{
    class Discord
    {
        static RichPresence presence;
        static EventHandlers handlers;
        
        public const string DLL = "discord-rpc.dll";

        public static bool LibCheck()
        {
            return File.Exists(DLL);
        }

        private static void Initialize(string clientId)
        {
            handlers = new EventHandlers
            {
                readyCallback = ReadyCallback
            };
            handlers.disconnectedCallback += DisconnectedCallback;
            handlers.errorCallback += ErrorCallback;
            DiscordRpc.Initialize(clientId, ref handlers, true, null);
        }
        
        private static IntPtr C2Ptr(string str)
        {
            if (str != null)
            {
                var retArray = Encoding.UTF8.GetBytes(str);
                var retPtr = Marshal.AllocHGlobal(retArray.Length + 1);
                Marshal.Copy(retArray, 0, retPtr, retArray.Length);
                Marshal.WriteByte(retPtr, retArray.Length, 0);
                return retPtr;
            }
            return IntPtr.Zero;
        }

        public static void StateUpdate(string details, string state)
        {
            presence.details = C2Ptr(details);
            presence.state = C2Ptr(state);
            presence.startTimestamp = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
            DiscordRpc.UpdatePresence(ref presence);
        }
        
        public static void UpdatePresence()
        {
            presence.details = C2Ptr("https://dimiwords.tk");
            presence.state = C2Ptr("시간 버리는 중...");
            presence.startTimestamp = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
            presence.largeImageKey = "logo";
            presence.largeImageText = null;
            presence.smallImageKey = null;
            presence.smallImageText = null;
            DiscordRpc.UpdatePresence(ref presence);
        }
        
        public static void Start()
        {
            var clientId = "528625130012934174";
            Initialize(clientId);
        }
        
        public static void Stop()
        {
            DiscordRpc.Shutdown();
        }
        
        private static void ReadyCallback()
        {
            Debug.WriteLine("Ready");
        }
        
        public static void DisconnectedCallback(int errorCode, string message)
        {
            Debug.WriteLine($"Disconnect {errorCode}: {message}");
        }
        
        private static void ErrorCallback(int errorCode, string message)
        {
            Debug.WriteLine($"Error {errorCode}: {message}");
        }
    }

    class DiscordRpc
    {
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void ReadyCallback();

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void DisconnectedCallback(int errorCode, string message);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void ErrorCallback(int errorCode, string message);

        public struct EventHandlers
        {
            public ReadyCallback readyCallback;
            public DisconnectedCallback disconnectedCallback;
            public ErrorCallback errorCallback;
        }

        [Serializable]
        public struct RichPresence
        {
            public IntPtr state;
            public IntPtr details;
            public long startTimestamp;
            public long endTimestamp;
            public string largeImageKey;
            public string largeImageText;
            public string smallImageKey;
            public string smallImageText;
            public string partyId;
            public int partySize;
            public int partyMax;
            public string matchSecret;
            public string joinSecret;
            public string spectateSecret;
            public bool instance;
        }
        
        [DllImport(Discord.DLL, EntryPoint = "Discord_Initialize", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Initialize(string applicationId, ref EventHandlers handlers, bool autoRegister, string optionalSteamId);

        [DllImport(Discord.DLL, EntryPoint = "Discord_UpdatePresence", CallingConvention = CallingConvention.Cdecl)]
        public static extern void UpdatePresence(ref RichPresence presence);

        [DllImport(Discord.DLL, EntryPoint = "Discord_Shutdown", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Shutdown();
    }
}
}
