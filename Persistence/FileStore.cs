using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Spider.Persistence
{
    public class FileStore
    {
        static readonly Dictionary<Type, PropertyInfo> KeyMembers = new Dictionary<Type, PropertyInfo>();
        static readonly ILog Log = LogManager.GetLogger("Spider");

        private readonly string _rootFolder;

        public FileStore(string rootFolder)
        {
            _rootFolder = rootFolder;
        }

        public void Save(object toSave)
        {
            Save(toSave, GetKey(toSave));
        }

        public void Save(object toSave, string key)
        {
            string fileName = GenerateFileName(key, "json");

            Log.InfoFormat("Saving file {0}", fileName);

            JsonSerializer serializer = new JsonSerializer();
            serializer.Converters.Add(new IsoDateTimeConverter());
            serializer.NullValueHandling = NullValueHandling.Ignore;

            using (StreamWriter sw = new StreamWriter(fileName))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                serializer.Serialize(writer, toSave);
            }
        }

        public void StoreText(string text, string key, string format)
        {
            string fileName = GenerateFileName(key, format);

            Log.InfoFormat("Saving file {0}", fileName);

            using (TextWriter writer = new StreamWriter(File.OpenWrite(fileName)))
            {
                writer.Write(text);
            }

        }


        public T Load<T>(string key) where T: class 
        {
            string fileName = GenerateFileName(key, "json");
            if (!File.Exists(fileName))
            {
                Log.DebugFormat("File '{0}' not found", fileName);
                return null;
            }

            JsonSerializer serializer = new JsonSerializer();
            serializer.Converters.Add(new IsoDateTimeConverter());
            serializer.NullValueHandling = NullValueHandling.Ignore;

            using (StreamReader sr = new StreamReader(fileName))
            using (JsonReader reader = new JsonTextReader(sr))
            {
                return serializer.Deserialize<T>(reader);
            }
        }


        public string LoadText(string key, string format)
        {
            string fileName = GenerateFileName(key, format);

            if (!File.Exists(fileName))
            {
                Log.DebugFormat("File '{0}' not found", fileName);
                return null;
            }

            using (TextReader reader = File.OpenText(fileName))
            {
                return reader.ReadToEnd();
            }
        }

        private static string GetKey(object o)
        {
            return GetKeyMemberForType(o.GetType()).GetValue(o, null).ToString();
        }

        private static PropertyInfo GetKeyMemberForType(Type type)
        {
            if (KeyMembers.ContainsKey(type))
                return KeyMembers[type];

            lock (KeyMembers)
            {
                if (KeyMembers.ContainsKey(type))
                    return KeyMembers[type];

                // Reflection to find it then store for posterity
                PropertyInfo member = type.GetProperty("Id");
                if (member == null)
                    throw new ArgumentException("Cannot find key field Id for type " + type.FullName);
                if (!member.CanRead)
                    throw new ArgumentException("Key field Id for type " + type.FullName + " is not readable.");

                KeyMembers.Add(type, member);
                return member;
            }
        }


        private string GenerateFileName(string key, string format)
        {
            int separator = key.LastIndexOf("/");
            string folder = "";
            if (separator >= 0) 
                folder = key.Substring(0, separator).Replace("/", "\\");

            string file = string.Format("{0}.{1}", key.Substring(separator + 1), format);
            string path = GeneratePath(folder);
            return Path.Combine(path, file);
        }

        private string GeneratePath(string folder)
        {
            string path = Path.Combine(_rootFolder, folder);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            return path;
        }



    }
}
