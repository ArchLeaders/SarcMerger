using System.Buffers;
using System.Collections.Frozen;
using System.Data;
using System.IO.Hashing;
using System.Text;
using BymlLibrary;
using CommunityToolkit.HighPerformance;
using CommunityToolkit.HighPerformance.Buffers;
using LiteYaml.Parser;

namespace SarcMergerTools.Scripts;

public class KeyedArrayCompilerScript
{
    private static readonly FrozenDictionary<string, BymlNodeType> _typeNameToNodeType = new Dictionary<string, BymlNodeType>() {
        ["string"] = BymlNodeType.String,
        ["u"] = BymlNodeType.UInt32,
        ["ul"] = BymlNodeType.UInt64,
        ["int"] = BymlNodeType.Int,
        ["binary"] = BymlNodeType.Binary,
    }.ToFrozenDictionary();
    
    private readonly HashSet<string> _keyPool = [];
    private readonly HashSet<string> _stringPool = [];

    private readonly Dictionary<string, Dictionary<string, (BymlNodeType, string)>> _typeUniqueEntries = [];
    private readonly Dictionary<string, (BymlNodeType, string)> _entries = [];

    public KeyedArrayCompilerScript(ReadOnlySequence<byte> src)
    {
        YamlParser parser = new(src);
        parser.SkipAfter(ParseEventType.MappingStart);

        while (parser.CurrentEventType is not ParseEventType.MappingEnd) {
            string key = parser.ReadScalarAsString()!;
            if (!parser.TryGetCurrentTag(out Tag? tag)) {
                continue;
            }
            
            string value = parser.ReadScalarAsString()!;
            ProcessEntry(key, tag.Suffix, value);
        }
    }

    public void Compile(Stream output)
    {
        SetupStringPools(
            out List<string> keyPool, out Dictionary<string, int> keyPoolLookup,
            out List<string> stringPool, out Dictionary<string, byte> stringPoolLookup
        );
        
        output.Write((ushort)_typeUniqueEntries.Count);
        foreach ((string classType, Dictionary<string, (BymlNodeType, string)> entries) in _typeUniqueEntries) {
            output.Write(XxHash3.HashToUInt64(classType.AsSpan().Cast<char, byte>()));
            output.Write((ushort)entries.Count);

            foreach ((string key, (BymlNodeType type, string str)) in entries) {
                output.Write(keyPoolLookup[key]);
                output.Write(type);
                output.Write(stringPoolLookup[str]);
            }
        }
        
        output.Write(_entries.Count);
        foreach ((string key, (BymlNodeType type, string str)) in _entries) {
            output.Write(keyPoolLookup[key]);
            output.Write(type);
            output.Write(stringPoolLookup[str]);
        }
        
        output.Write(keyPool.Count);
        foreach (byte[] utf8 in keyPool.Select(key => Encoding.UTF8.GetBytes(key))) {
            output.Write(utf8);
            output.Write<byte>(0);
        }
        
        output.Write(stringPool.Count);
        foreach (byte[] utf8 in stringPool.Select(str => Encoding.UTF8.GetBytes(str))) {
            output.Write(utf8);
            output.Write<byte>(0);
        }
    }

    private void ProcessEntry(string arrayKey, string typeName, string key)
    {
        int keyStartIndex;
        
        if ((keyStartIndex = arrayKey.IndexOf('/')) == -1) {
            _entries[arrayKey] = (_typeNameToNodeType[typeName], key);
            return;
        }
        
        string classType = arrayKey[..keyStartIndex];
        arrayKey = arrayKey[(keyStartIndex + 1)..];
        if (!_typeUniqueEntries.TryGetValue(classType, out Dictionary<string, (BymlNodeType, string)>? entries)) {
            _typeUniqueEntries[classType] = entries = [];
        }
        
        entries[arrayKey] = (_typeNameToNodeType[typeName], key);
    }

    private void SetupStringPools(out List<string> keyPool, out Dictionary<string, int> keyPoolLookup, out List<string> stringPool, out Dictionary<string, byte> stringPoolLookup)
    {
        keyPoolLookup = [];
        keyPool = [];
        
        stringPoolLookup = [];
        stringPool = [];

        foreach ((string key, (_, string str)) in _entries.Concat(_typeUniqueEntries.SelectMany(x => x.Value))) {
            if (!keyPoolLookup.ContainsKey(key)) {
                keyPool.Add(key);
                keyPoolLookup[key] = -1;
            }

            if (stringPoolLookup.ContainsKey(str)) {
                continue;
            }

            stringPool.Add(str);
            stringPoolLookup[str] = 0;
        }

        keyPool.Sort(StringComparer.Ordinal);
        stringPool.Sort(StringComparer.Ordinal);

        for (int i = 0; i < keyPool.Count; i++) {
            keyPoolLookup[keyPool[i]] = i;
        }

        for (int i = 0; i < stringPool.Count; i++) {
            stringPoolLookup[stringPool[i]] = Convert.ToByte(i);
        }
    }
}