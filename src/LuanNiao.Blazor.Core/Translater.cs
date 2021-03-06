﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Linq;

namespace LuanNiao.Blazor.Core
{
    public static class Translater
    {
        public class SourceItem
        {
            public string CultureName { get; set; }
            public string Data { get; set; }
            public SourceItemType ItemType { get; set; }
        }

        public enum SourceItemType
        {
            LocalFile = 0,
            UrlAddress = 1,
            OrignalString = 2
        }



        private readonly static Dictionary<string, Dictionary<string, string>> _languageSource = new Dictionary<string, Dictionary<string, string>>();
        
        public static string CurrentCulture { get; private set; }

        public static event Action<string> CultureChanged;
        public static void AddLanguageFile(SourceItem[] sources)
        {
            if (sources == null)
            {
                return;
            }
            foreach (var item in sources)
            {
                switch (item.ItemType)
                {
                    case SourceItemType.LocalFile:
                        break;
                    case SourceItemType.UrlAddress:
                        LoadResourceFile(item.CultureName, item.Data);
                        break;
                    case SourceItemType.OrignalString:
                        AddToCultureData(item.CultureName, item.Data);
                        break;
                    default:
                        break;
                }

            }
        }

        public static void ConvertTo(string culture)
        {
            CurrentCulture = culture;
            CultureChanged?.Invoke(CurrentCulture);
        }

        public static string Tr(string key)
        {
            if (CurrentCulture is null || !_languageSource.ContainsKey(CurrentCulture) || !_languageSource[CurrentCulture].ContainsKey(key))
            {
                return key;
            }
            return _languageSource[CurrentCulture][key];
        }

        public static string Tr(Func<string, string> action)
        {
            if (action!=null)
            {
                return action(CurrentCulture);
            }
            return "";
        }



        private static void LoadResourceFile(string culture, string fileUrl)
        {
            string resultstring = "";
            Encoding encoding = Encoding.UTF8;
            var request = WebRequest.Create(fileUrl);
            request.Method = "GET";
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
            {
                resultstring = reader.ReadToEnd();
            }
            AddToCultureData(culture, resultstring);
        }
        public static void AddToCultureData(string culture, string jsonData) => AddToCultureData(culture, System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(jsonData));

        public static void AddToCultureData(string culture, Dictionary<string, string> langData)
        {
            if (!_languageSource.ContainsKey(culture))
            {
                try
                {
                    _languageSource.Add(culture, langData);
                }
                catch
                {

                }
            }
            else
            {
                foreach (var item in langData)
                {
                    if (!_languageSource[culture].ContainsKey(item.Key))
                    {
                        _languageSource[culture].Add(item.Key, item.Value);
                    }
                }
            }
        }
    }
}
