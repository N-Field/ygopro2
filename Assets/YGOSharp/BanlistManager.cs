using System.Collections.Generic;
using System.IO;

namespace YGOSharp
{
    public static class BanlistManager
    {
        public static List<Banlist> Banlists { get; private set; }

        public static bool initialize(string fileName, bool test = false)
        {
            List<Banlist> new_list = new List<Banlist>();
            Banlist current = null;
            StreamReader reader = new StreamReader(fileName);
            bool success = true;
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                try
                {
                    if (line == null)
                        continue;
                    if (line.StartsWith("#"))
                        continue;
                    if (line.StartsWith("!"))
                    {
                        current = new Banlist();
                        current.Name = line.Substring(1, line.Length - 1);
                        new_list.Add(current);
                        continue;
                    }
                    if (!line.Contains(" "))
                        continue;
                    if (current == null)
                        continue;
                    string[] data = line.Split(new char[] {  ' '  }, System.StringSplitOptions.RemoveEmptyEntries);
                    int id = int.Parse(data[0]);
                    int count = int.Parse(data[1]);
                    current.Add(id, count);
                }
                catch (System.Exception e)  
                {
                    success = false;
                    UnityEngine.Debug.Log(line);
                    UnityEngine.Debug.Log(e);
                }
            }
            if(!test) {
                if (Banlists == null) {
                    Banlists = new_list;
                } else { 
                    foreach (var item in new_list) {
                        Banlists.Add(item);
                    }
                }
            }
            return (success && new_list.Count > 1);
        }

        public static void initializeComplete() {
            Banlist blank = new Banlist();
            blank.Name = "N/A";
            Banlists.Add(blank);
        }
        public static int GetIndex(uint hash)
        {
            for (int i = 0; i < Banlists.Count; i++)
                if (Banlists[i].Hash == hash)
                    return i;
            return 0;
        }

        public static string GetName(uint hash)    
        {
            for (int i = 0; i < Banlists.Count; i++)
                if (Banlists[i].Hash == hash)
                    return Banlists[i].Name;
            return InterString.Get("未知卡表");
        }

        public static List<string> getAllName()
        {
            List<string> returnValue = new List<string>();
            foreach (var item in Banlists)
            {
                returnValue.Add(item.Name);
            }
            return returnValue;
        }

        public static Banlist GetByName(string name)
        {
            Banlist returnValue = Banlists[Banlists.Count - 1];
            foreach (var item in Banlists)
            {
                if (item.Name == name)
                {
                    returnValue = item;
                }
            }
            return returnValue;
        }

        public static Banlist GetByHash(uint hash)
        {
            Banlist returnValue = Banlists[Banlists.Count - 1];
            foreach (var item in Banlists)
            {
                if (item.Hash == hash)
                {
                    returnValue = item;
                }
            }
            return returnValue;
        }

    }
}
