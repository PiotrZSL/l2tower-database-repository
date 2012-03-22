using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Windows.Forms;
using System.Runtime.Serialization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace L2TowerDatabaseGenerator
{
    [Serializable]
    public class SkillTableItem
    {
        public static T Clone<T>(T source)
        {
            if (!typeof(T).IsSerializable)
            {
                throw new ArgumentException("The type must be serializable.", "source");
            }

            // Don't serialize a null object, simply return the default for that object
            if (Object.ReferenceEquals(source, null))
            {
                return default(T);
            }

            IFormatter formatter = new BinaryFormatter();
            Stream stream = new MemoryStream();
            using (stream)
            {
                formatter.Serialize(stream, source);
                stream.Seek(0, SeekOrigin.Begin);
                return (T)formatter.Deserialize(stream);
            }
        }
        [Serializable]
        public class SkillEffectItem
        {
            internal Dictionary<string, string> values = new Dictionary<string, string>();

            public string getName()
            {
                return values["name"];
            }

            public string getValue(string name)
            {
                if (values.ContainsKey(name))
                    return values[name];
                else
                    return null;
            }
        }
        private Dictionary<string, string> settings = new Dictionary<string, string>();
        private Dictionary<string, SkillEffectItem> effects = new Dictionary<string, SkillEffectItem>();

        private long skillId;
        private long skillLvl;
        private string name;

        public long getSkillId()
        {
            return skillId;
        }

        public string getName()
        {
            return name;
        }

        public long getSkillLvl()
        {
            return skillLvl;
        }

        public string getValue(string name)
        {
            if (settings.ContainsKey(name))
                return settings[name];
            else
                return null;
        }

        public static List<SkillTableItem> parseXml(string fileName)
        {
            List<SkillTableItem> items = new List<SkillTableItem>();
            XmlDocument oXmlDocument = new XmlDocument();
            oXmlDocument.Load(fileName);
            XmlNodeList oPersonNodesList = oXmlDocument.GetElementsByTagName("skill");
            foreach (XmlNode skill in oPersonNodesList)
            {
                items.AddRange(parseSkill(skill));
            }
            Log.log("Processed " + fileName + " - found " + items.Count + " skills");
            return items;
        }

        private static List<SkillTableItem> parseSkill(XmlNode node)
        {
            Dictionary<string, string[]> tables = new Dictionary<string, string[]>();
            List<SkillTableItem> items = new List<SkillTableItem>();
            foreach (XmlNode ns in node.ChildNodes)
            {
                if (ns.Name == "table")
                {
                    string v = ns.ChildNodes.Item(0).Value.Trim();
                    v = v.Replace('\n', ' ');
                    v = v.Replace('\t', ' ');
                    v = v.Replace('\r', ' ');
                    while(v.Contains("  "))
                    {
                        v = v.Replace("  ", " ");
                    }
                    v = v.Trim();
                    tables.Add(ns.Attributes.GetNamedItem("name").Value.Trim(), v.Split(' '));
                }
            }
            SkillTableItem baseItem = new SkillTableItem();
            int levels = Convert.ToInt32(node.Attributes.GetNamedItem("levels").Value);
            baseItem.name = node.Attributes.GetNamedItem("name").Value;
            baseItem.skillId = Convert.ToInt32(node.Attributes.GetNamedItem("id").Value);
            try
            {
                for (int i = 1; i <= levels; ++i)
                {
                    baseItem.skillLvl = i;
                    foreach (XmlNode ns in node.ChildNodes)
                    {
                        if (ns.Name == "set")
                        {
                            string value = ns.Attributes.GetNamedItem("val").Value.Trim();
                            if (value.Length > 0 && tables.ContainsKey(value))
                            {
                                value = tables[value][i - 1];
                            }
                            baseItem.settings[ns.Attributes.GetNamedItem("name").Value.Trim()] = value;
                        }
                        else if (ns.Name == "for")
                        {
                            foreach (XmlNode ne in ns.ChildNodes)
                            {
                                if (ne.Name == "effect")
                                {
                                    SkillEffectItem effect = new SkillEffectItem();
                                    foreach (XmlNode na in ne.Attributes)
                                    {
                                        string value = na.Value.Trim();
                                        if (value.Length > 0 && tables.ContainsKey(value))
                                        {
                                            value = tables[value][i - 1];
                                        }
                                        effect.values.Add(na.Name.Trim(), value);
                                    }
                                    if (effect.values.Count > 0)
                                    {
                                        baseItem.effects[effect.getName()] = effect;
                                    }
                                }
                            }
                        }
                    }
                    items.Add(baseItem);
                    baseItem = Clone<SkillTableItem>(baseItem);
                }

                //enchants...
                SkillTableItem backup = Clone<SkillTableItem>(baseItem);

                for (int i = 1; i <= 20; ++i) //maximum 20 enchants
                {
                    int level = 0;
                    while (level < 100)
                    {
                        level++;
                        baseItem = Clone<SkillTableItem>(backup);
                        baseItem.skillLvl = i * 100 + level;
                        bool wasEnchant = false;
                        foreach (XmlNode ns in node.ChildNodes)
                        {
                            if (ns.Name == "enchant" + i)
                            {
                                wasEnchant = true;
                                string value = ns.Attributes.GetNamedItem("val").Value.Trim();
                                if (value.Length > 0 && tables.ContainsKey(value))
                                {
                                    if (level - 1 < tables[value].Length)
                                    {
                                        value = tables[value][level - 1];
                                    }
                                    else
                                    {
                                        level = 100;
                                        break;
                                    }
                                }
                                baseItem.settings[ns.Attributes.GetNamedItem("name").Value.Trim()] = value;
                            }
                            else if (ns.Name == "enchant" + i + "for")
                            {
                                wasEnchant = true;
                                foreach (XmlNode ne in ns.ChildNodes)
                                {
                                    if (ne.Name == "effect")
                                    {
                                        SkillEffectItem effect = new SkillEffectItem();
                                        foreach (XmlNode na in ne.Attributes)
                                        {
                                            string value = na.Value.Trim();
                                            if (value.Length > 0 && tables.ContainsKey(value))
                                            {
                                                if (level - 1 < tables[value].Length)
                                                {
                                                    value = tables[value][level - 1];
                                                }
                                                else
                                                {
                                                    level = 100;
                                                    break;
                                                }
                                            }
                                            effect.values.Add(na.Name.Trim(), value);
                                        }
                                        if (effect.values.Count > 0)
                                        {
                                            baseItem.effects[effect.getName()] = effect;
                                        }
                                    }
                                }
                            }
                        }
                        if (!wasEnchant)
                        {
                            level = 100;
                        }
                        if (level < 100)
                        {
                            items.Add(baseItem);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(baseItem.name);
            }
            return items;
        }
    }
}
