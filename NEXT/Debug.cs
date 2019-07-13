using System;

namespace Next {

    public static class Debug
    {
        public static void Out(ColossalFramework.Plugins.PluginManager.MessageType messageType, bool useComma, params System.Object[] o)
        {
#if DEBUG
            try
            {
                string s = "";
                for (int i = 0; i < o.Length; i++)
                {
                    s += o[i].ToString();
                    if (i < o.Length - 1 && useComma)
                        s += "  ,  ";
                }
                DebugOutputPanel.AddMessage(messageType, s);
            }
            catch (Exception)
            {
            }
#endif
        }

        public static void Log(params System.Object[] o)
        {
#if DEBUG
            Message(o);
#endif
        }

        public static void Message(params System.Object[] o)
        {
#if DEBUG
            Message(true, o);
#endif
        }

        public static void Message(bool useComma, params System.Object[] o)
        {
#if DEBUG
            Out(ColossalFramework.Plugins.PluginManager.MessageType.Message, useComma, o);
#endif
        }

        public static void Warning(params System.Object[] o)
        {
#if DEBUG
            Warning(true, o);
#endif
        }

        public static void Warning(bool useComma, params System.Object[] o)
        {
#if DEBUG
            Out(ColossalFramework.Plugins.PluginManager.MessageType.Warning, useComma, o);
#endif
        }

        public static void Error(params System.Object[] o)
        {
#if DEBUG
            Error(true, o);
#endif
        }

        public static void Error(bool useComma, params System.Object[] o)
        {
#if DEBUG
            Out(ColossalFramework.Plugins.PluginManager.MessageType.Error, useComma, o);
#endif
        }
    }

}