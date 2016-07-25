using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

public class LanguageReader: MonoBehaviour {

    public TextAsset textAsset;
    public List<Dictionary<string, string>> langDict = new List<Dictionary<string, string>>();

    void Awake()
    {
        Read();
    }

    void Read()
    {
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(textAsset.text);
        XmlNodeList languagesNodeList = xmlDoc.GetElementsByTagName("language");
        foreach (XmlNode languageNode in languagesNodeList)
        {
            XmlNodeList languageContent = languageNode.ChildNodes;
            Dictionary<string, string> languageDict = new Dictionary<string, string>();
            foreach (XmlNode node in languageContent)
            {
                languageDict.Add(node.Name, node.InnerText);
            }
            langDict.Add(languageDict);
        }
    }
}
