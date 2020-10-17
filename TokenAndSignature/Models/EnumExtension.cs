﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TokenAndSignature.Models
{
    public static class EnumExtension
    {
        private static Dictionary<string, Dictionary<string, string>> _enumCache;

        private static Dictionary<string, Dictionary<string, string>> EnumCache
        {
            get
            {
                if (_enumCache == null)
                {
                    _enumCache = new Dictionary<string, Dictionary<string, string>>();
                }
                return _enumCache;
            }
            set { _enumCache = value; }
        }

        public static string GetEnumText(this Enum en)
        {
            string enString = string.Empty;
            if (null == en) return enString;
            var type = en.GetType();
            enString = en.ToString();
            if (!EnumCache.ContainsKey(type.FullName))
            {
                var fields = type.GetFields();
                Dictionary<string, string> temp = new Dictionary<string, string>();
                foreach (var item in fields)
                {
                    var attrs = item.GetCustomAttributes(typeof(TextAttribute), false);
                    if (attrs.Length == 1)
                    {
                        var v = ((TextAttribute)attrs[0]).Value;
                        temp.Add(item.Name, v);
                    }
                }
                EnumCache.Add(type.FullName, temp);
            }
            if (EnumCache[type.FullName].ContainsKey(enString))
            {
                return EnumCache[type.FullName][enString];
            }
            return enString;
        }
    }

    public class TextAttribute : Attribute
    {
        public TextAttribute(string value)
        {
            Value = value;
        }

        public string Value { get; set; }
    }
}
