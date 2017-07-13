# 设计思路
![](https://github.com/RickJiangShu/Documents/blob/master/ConfigManager/p1.jpg "")<br>
整个流程如上图所示，所以整个思路可以分为以下四个部分：解析、反射、序列化和反序列化

# 解析
1. 按“分隔符”与“换行符”将表格切割成“行x列”的**字符串矩阵**，关键代码如下：
```
public static string[,] Content2Matrix(string config, string sv, string lf, out int row, out int col)
{
    config = config.Trim();//清空末尾的空白

    //分割
    string[] lines = Regex.Split(config, lf);
    string[] firstLine = Regex.Split(lines[0], sv, RegexOptions.Compiled);
            
    row = lines.Length;
    col = firstLine.Length;
    string[,] matrix = new string[row, col];
    //为第一行赋值
    for (int i = 0, l = firstLine.Length; i < l; i++)
    {
        matrix[0, i] = firstLine[i];
    }
    //为其他行赋值
    for (int i = 1, l = lines.Length; i < l; i++)
    {
        string[] line = Regex.Split(lines[i], sv);
        for (int j = 0, k = line.Length; j < k; j++)
        {
            matrix[i, j] = line[j];
        }
    }
    return matrix;
}
```

2. 从矩阵中取出相应的字符，替换**自定义模板**中的变量并写入文件，关键代码如下：
```
string idType = ConfigTools.SourceType2CSharpType(src.matrix[1, 0]);
string idField = src.matrix[2, 0];

//属性声明
string declareProperties = "";
for (int x = 0; x < src.column; x++)
{
    string comment = src.matrix[0, x];
    string csType = ConfigTools.SourceType2CSharpType(src.matrix[1, x]);
    string field = src.matrix[2, x];
    string declare = string.Format(templete2, comment, csType, field);
    declareProperties += declare;
}

//替换
content = content.Replace("/*ClassName*/", src.configName);
content = content.Replace("/*DeclareProperties*/", declareProperties);
content = content.Replace("/*IDType*/", idType);
content = content.Replace("/*IDField*/", idField);

//写入
ConfigTools.WriteFile(outputPath, content);
```
# 反射
上面解析出了C#文件和一个SerializableSet.cs，接下来将通过反射特性**实例化**一个SerializableSet对象，关键代码如下：
```
public static object Serialize(List<Source> sources)
{
    Type t = FindType("SerializableSet");
    if (t == null)
    {
        UnityEngine.Debug.LogError("找不到SerializableSet类！");
        return null;
    }

    object set = UnityEngine.ScriptableObject.CreateInstance(t);

    foreach(Source source in sources)
    {
        string fieldName = source.sourceName + "s";
        Array configs = Source2Configs(source);
        FieldInfo fieldInfo = t.GetField(fieldName);
        fieldInfo.SetValue(set,configs);
    }
    return set;
}
```

# 序列化
最后就是使用Unity API**创建Asset文件**，关键代码如下：
```
UnityEngine.Object set = (UnityEngine.Object)Serializer.Serialize(sources);
string o = cache.assetOutputFolder + "/" + assetName;
AssetDatabase.CreateAsset(set, o);
```

# 反序列化
因为要在运行时使用，所以反序列化的代码没有使用反射（效率低）。而是在解析的过程中解析出一个反序列化文件，生成的代码如下：
```
public class Deserializer
{
    public static void Deserialize(SerializableSet set)
    {
        for (int i = 0, l = set.Equips.Length; i < l; i++)
        {
            EquipConfig.GetDictionary().Add(set.Equips[i].EquipId, set.Equips[i]);
        }

        for (int i = 0, l = set.EquipCSVs.Length; i < l; i++)
        {
            EquipCSVConfig.GetDictionary().Add(set.EquipCSVs[i].EquipId, set.EquipCSVs[i]);
        }
    }
}
```
