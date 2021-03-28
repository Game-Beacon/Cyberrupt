using System;
using System.Collections.Generic;

public static class DependencyContainer
{
    private static Dictionary<Type, List<object>> instances;

    /// <summary>
    /// 嘗試在字典內新增指定型別的實體。
    /// </summary>
    /// <typeparam name="T">
    /// 指定新增型別。
    /// </typeparam>
    /// <param name="obj">
    /// 要新增的型別實體。
    /// </param>
    public static void AddDependency<T>(T obj)
    {
        if (instances == null)
            instances = new Dictionary<Type, List<object>>();

        Type objType = typeof(T);
        instances.TryGetValue(objType, out var dependencies);

        if (dependencies == null)
        {
            dependencies = new List<object>();
            dependencies.Add(obj);
            instances.Add(objType, dependencies);
        }
        else if (!dependencies.Contains(obj))
            instances[objType].Add(obj);
    }

    /// <summary>
    /// 嘗試在字典內搜尋指定型別串列，並回傳該串列的第一個元素。
    /// </summary>
    /// <typeparam name="T">
    /// 指定搜尋型別。
    /// </typeparam>
    /// <returns>
    /// 回傳的型別實體。
    /// </returns>
    public static object GetDependency<T>()
    {
        if (instances == null)
            instances = new Dictionary<Type, List<object>>();

        Type objType = typeof(T);

        instances.TryGetValue(objType, out var dependencies);

        if (dependencies == null)
            throw new System.Exception("Type " + objType.ToString() + " is not found in the dependency container.");

        return dependencies[0];
    }

    /// <summary>
    /// 嘗試在字典內搜尋指定型別串列，並回傳整個串列。
    /// </summary>
    /// <typeparam name="T">
    /// 指定搜尋型別。
    /// </typeparam>
    /// <returns>
    /// 回傳的型別串列。
    /// </returns>
    public static List<object> GetAllDependencies<T>()
    {
        if (instances == null)
            instances = new Dictionary<Type, List<object>>();

        Type objType = typeof(T);
        instances.TryGetValue(objType, out var dependencies);

        if (dependencies == null)
            throw new System.Exception("Type " + objType.ToString() + " is not found in the dependency container.");
        else
            return dependencies;
    }

    public static void Clear()
    {
        if (instances != null)
            instances.Clear();
    }
}
