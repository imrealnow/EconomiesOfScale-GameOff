using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/Persistent/DataFile")]
public class PersistentScriptableObject : ScriptableObject
{
    [SerializeField]
    private List<DataEntry> dataTemplate = new List<DataEntry>();
    [SerializeField, HideInInspector]
    private DataContainer dataContainer;

    public event Action DataChanged;

    public dynamic Data => CreateDynamicData();
    public bool IsLoaded => dataContainer != null;
    public string FilePath => Path.Combine(Application.persistentDataPath, (this.name + ".xml"));
    public List<DataEntry> DataTemplate => dataTemplate;
    public DataContainer DataContainer => dataContainer;

    private Dictionary<string, Type> allowedTypes =>
        dataTemplate.ToDictionary(data => data.key, data => SystemType.GetTypeFromEnum(data.type));

    public void Awake()
    {
        LoadData();
    }

    public void SetDataTemplate(List<DataEntry> template)
    {
        dataTemplate = template;
        CreateDataContainer();
        SaveData();
    }

    [ContextMenu("Add Field")]
    private void AddTemplateField()
    {
        dataTemplate.Add(new DataEntry("New Field", SystemType.TypeEnum.String));
    }

    [ContextMenu("Refresh Data")]
    private void CreateDataContainer()
    {
        dataContainer = new DataContainer(dataTemplate);
    }

    [ContextMenu("Save Data")]
    public void SaveData()
    {
        if (!dataContainer.IsAssigned)
            dataContainer = new DataContainer(dataTemplate);
        var serializer = new XmlSerializer(typeof(DataContainer));
        using (var stream = new FileStream(FilePath, FileMode.Create))
            serializer.Serialize(stream, dataContainer);
    }

    [ContextMenu("Load Data")]
    public void LoadData()
    {
        DataContainer savedData;
        if (!File.Exists(FilePath))
        {
            savedData = new DataContainer(dataTemplate);
        }
        else
        {
            var serializer = new XmlSerializer(typeof(DataContainer));
            using var stream = new FileStream(FilePath, FileMode.Open);
            stream.Position = 0;
            savedData = serializer.Deserialize(stream) as DataContainer;
        }
        dataContainer = savedData;
    }

    private ExpandoObject CreateDynamicData()
    {
        if (!dataContainer.IsAssigned)
            LoadData();
        dynamic newDataObject = new ExpandoObject();
        var dictionaryObject = newDataObject as IDictionary<string, object>;
        foreach (var pair in dataContainer.data)
        {
            dictionaryObject.Add(pair.Key, pair.Value);
        }
        return dictionaryObject as ExpandoObject;
    }

    public bool TrySetValue<T>(string propName, T value, bool ignoreType = false)
    {
        // if template contains property
        if (allowedTypes.TryGetValue(propName, out Type propType) || ignoreType)
        {
            // if the types don't match
            if (typeof(T) != propType && !ignoreType)
                return false;
            // find matching property and change its value
            var match = dataContainer.data.First(pair => pair.Key.Equals(propName));
            int matchIndex = dataContainer.data.IndexOf(match);
            dataContainer.data[matchIndex] = new KeyValuePair<string, object>(propName, value);
            DataChanged?.Invoke();
            SaveData();
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool TryGetValue<T>(string propName, out T value)
    {
        // if template contains property
        value = default(T);
        if (allowedTypes.TryGetValue(propName, out Type propType))
        {
            // if the types don't match
            if (typeof(T) != propType)
                return false;
            // find matching property and change its value
            value = (T)dataContainer.data.Single(pair => pair.Key.Equals(propName)).Value;
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool TryGetRawValue(string propName, out object value)
    {
        value = dataContainer.data.Single(pair => pair.Key.Equals(propName)).Value;
        return value != null;
    }

    [ContextMenu("Print current data")]
    private void PrintData()
    {
        var currentData = Data as IDictionary<string, object>;
        foreach (var pair in currentData)
        {
            Debug.Log($@"{pair.Key}:{pair.Value.ToString()}");
        }
    }
}

[Serializable]
[XmlType(TypeName = "Container"), XmlRoot(elementName: "Data Container")]
public class DataContainer
{
    [XmlArray]
    [XmlArrayItem(ElementName = "DataEntry", Type = typeof(DataEntry))]
    public List<DataEntry> dataTemplate;
    [XmlArray]
    [XmlArrayItem(ElementName = "Data", Type = typeof(KeyValuePair<string, object>))]
    public List<KeyValuePair<string, object>> data;
    [XmlElement(ElementName = "IsAssigned")]
    public bool IsAssigned = false;

    public DataContainer(List<DataEntry> dataTemplate)
    {
        this.IsAssigned = true;
        this.dataTemplate = dataTemplate;
        this.data = dataTemplate.ConvertAll(d =>
            new KeyValuePair<string, object>(d.key, Activator.CreateInstance(SystemType.GetTypeFromEnum(d.type))));
    }

    public DataContainer()
    {
        this.IsAssigned = false;
        this.dataTemplate = null;
        this.data = null;
    }
}

[Serializable]
public struct DataEntry
{
    public string key;
    public SystemType.TypeEnum type;


    public DataEntry(string key, SystemType.TypeEnum dataType)
    {
        this.key = key;
        this.type = dataType;
    }
}

[Serializable]
[XmlType(TypeName = "Pair")]
public struct KeyValuePair<K, V>
{
    public K Key
    { get; set; }

    public V Value
    { get; set; }

    public KeyValuePair(K key, V value)
    {
        Key = key;
        Value = value;
    }
}