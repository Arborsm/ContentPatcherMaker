using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ContentPatcherMaker.Core.Serialization;

public class JsonTreeNode
{
    public string Name { get; set; }
    public string? Value { get; set; }
    public ObservableCollection<JsonTreeNode> Children { get; } = new();

    public JsonTreeNode(string name, string? value = null)
    {
        Name = name;
        Value = value;
    }
}

public static class JsonTreeBuilder
{
    public static ObservableCollection<object> Build(string json)
    {
        var roots = new ObservableCollection<object>();
        try
        {
            var token = JToken.Parse(string.IsNullOrWhiteSpace(json) ? "{}" : json);
            if (token is JObject obj)
            {
                var node = new JsonTreeNode("root");
                BuildObject(node, obj);
                roots.Add(node);
            }
            else if (token is JArray arr)
            {
                var node = new JsonTreeNode("root[]");
                BuildArray(node, arr);
                roots.Add(node);
            }
            else
            {
                roots.Add(new JsonTreeNode("root", token.ToString()));
            }
        }
        catch (JsonException ex)
        {
            roots.Add(new JsonTreeNode("error", ex.Message));
        }
        return roots;
    }

    private static void BuildObject(JsonTreeNode parent, JObject obj)
    {
        foreach (var prop in obj.Properties())
        {
            var child = new JsonTreeNode(prop.Name);
            parent.Children.Add(child);
            AddToken(child, prop.Value);
        }
    }

    private static void BuildArray(JsonTreeNode parent, JArray arr)
    {
        for (int i = 0; i < arr.Count; i++)
        {
            var child = new JsonTreeNode($"[{i}]");
            parent.Children.Add(child);
            AddToken(child, arr[i]!);
        }
    }

    private static void AddToken(JsonTreeNode node, JToken token)
    {
        switch (token)
        {
            case JObject obj:
                BuildObject(node, obj);
                break;
            case JArray arr:
                BuildArray(node, arr);
                break;
            default:
                node.Value = token.Type == JTokenType.String ? token.ToString() : token.ToString(Newtonsoft.Json.Formatting.None);
                break;
        }
    }
}

