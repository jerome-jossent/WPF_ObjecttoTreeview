using Newtonsoft.Json;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Media;

internal class ObjectExplorer
{
    public static bool FillTreeView(TreeView trv, object objet)
    {
        TreeViewItem trvi = FillTreeView(objet);
        if (trvi == null)
            return false;

        trv.Items.Clear();
        trv.Items.Add(trvi);
        return true;
    }

    public static TreeViewItem FillTreeView(object objet)
    {
        string json = JsonConvert.SerializeObject(objet, Formatting.Indented);
        Dictionary<string, dynamic>? dic = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(json);

        if (dic == null)
            return null;

        TreeViewItem trvi = new TreeViewItem();
        ListViewItemHeaderName(trvi, objet.GetType().ToString());
        TreeViewItemAddType(trvi, Datatype.OBJ);
        TreeViewItemFill(trvi, dic);

        return trvi;
    }

    static void TreeViewItemFill(TreeViewItem trvi, object value)
    {
        string typ = value.GetType().ToString();
        switch (typ)
        {
            case "System.Collections.Generic.Dictionary`2[System.String,System.Object]":
                Dictionary<string, dynamic>? v_dico = value as Dictionary<string, dynamic>;
                if (v_dico == null) break;
                foreach (var item in v_dico)
                    TreeViewItemFill(trvi, item);
                break;

            case "System.Collections.Generic.KeyValuePair`2[System.String,System.Object]":
                KeyValuePair<string, object>? v_kv = value as KeyValuePair<string, object>?;
                if (v_kv == null) break;
                TreeViewItem tvi_kv = new TreeViewItem();
                ListViewItemHeaderName(tvi_kv, v_kv.Value.Key);

                trvi.Items.Add(tvi_kv);
                TreeViewItemFill(tvi_kv, v_kv.Value.Value);
                break;

            case "Newtonsoft.Json.Linq.JArray":
                Newtonsoft.Json.Linq.JArray? v_array = value as Newtonsoft.Json.Linq.JArray;
                if (v_array == null) break;
                TreeViewItemAddType(trvi, Datatype.ARY);

                TreeViewItem tvi_array = new TreeViewItem();
                ListViewItemHeaderName(tvi_array, "");
                string chaine = string.Join(", ", v_array);
                ListViewItemHeaderValue(tvi_array, "[" + chaine + "]");
                trvi.Items.Add(tvi_array);
                break;

            case "Newtonsoft.Json.Linq.JObject":
                Newtonsoft.Json.Linq.JObject? v_jobject = value as Newtonsoft.Json.Linq.JObject;
                string json = JsonConvert.SerializeObject(v_jobject, Formatting.Indented);
                Dictionary<string, dynamic>? dic = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(json);
                if (dic == null) break;
                TreeViewItemAddType(trvi, Datatype.OBJ);
                TreeViewItemFill(trvi, dic);
                break;

            case "System.Int64":
                long? v_long = value as long?;
                TreeViewItemAddType(trvi, Datatype.LNG);
                ListViewItemHeaderValue(trvi, v_long.ToString());
                break;

            case "System.Double":
                double? v_double = value as double?;
                TreeViewItemAddType(trvi, Datatype.DBL);
                ListViewItemHeaderValue(trvi, v_double.ToString());
                break;

            case "System.String":
                string? v_string = value as string;
                TreeViewItemAddType(trvi, Datatype.STR);
                ListViewItemHeaderValue(trvi, "\"" + v_string + "\"".ToString());
                break;

            default:
                trvi.Header += "type non pris en charge : \"" + typ + "\"";
                break;
        }
    }

    static void ListViewItemHeaderName(TreeViewItem trvi, string text)
    {
        StackPanel sp = new StackPanel() { Orientation = Orientation.Horizontal };
        sp.Height = 20;

        TextBlock tbk = new TextBlock() { Text = "\"" + text + "\" : ", FontWeight = FontWeights.Bold };
        sp.Children.Add(tbk);
        trvi.Header = sp;
    }

    enum Datatype { LNG, DBL, STR, ARY, OBJ}

    static void TreeViewItemAddType(TreeViewItem trvi, Datatype dt)
    {
        StackPanel sp = (StackPanel)trvi.Header;
        Ellipse bullet = new Ellipse()
        {
            Width = 5,
            Height = 5,
            StrokeThickness = 0,
        };
        SolidColorBrush color;
        switch (dt)
        {
            case Datatype.LNG: color = new SolidColorBrush(Colors.GreenYellow); break;
            case Datatype.DBL: color = new SolidColorBrush(Colors.Cyan); break;
            case Datatype.STR: color = new SolidColorBrush(Colors.Blue); break;
            case Datatype.ARY: color = new SolidColorBrush(Colors.Orange); break;
            case Datatype.OBJ: color = new SolidColorBrush(Colors.Red); break;
            default: color = new SolidColorBrush(Colors.Black); break;
        }
        bullet.Fill = color;
        sp.Children.Insert(0, bullet);
    }

    static void ListViewItemHeaderValue(TreeViewItem trvi, string text)
    {
        StackPanel sp = (StackPanel)trvi.Header;
        TextBlock tbk = new TextBlock() { Text = text };
        sp.Children.Add(tbk);
        trvi.Header = sp;
    }

    public static void ExpandAll(ItemsControl items, bool expand)
    {
        foreach (ItemsControl childControl in items.Items)
        {
            if (childControl != null)
                ExpandAll(childControl, expand);

            TreeViewItem? trvi = childControl as TreeViewItem;
            if (trvi != null)
                trvi.IsExpanded = true;
        }
    }
}