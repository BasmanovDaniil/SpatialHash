using System.Linq;
using UnityEngine;
using System.Collections.Generic;

public class SpatialHash<T>
{
    private Dictionary<int, List<T>> dict;
    private Dictionary<T, int> objects;
    private int cellSize;

    public SpatialHash(int cellSize)
    {
        this.cellSize = cellSize;
        dict = new Dictionary<int, List<T>>();
        objects = new Dictionary<T, int>();
    }

    public void Insert(Vector3 vector, T obj)
    {
        var key = Key(vector);
        if (dict.ContainsKey(key))
        {
            dict[key].Add(obj);
        }
        else
        {
            dict[key] = new List<T> { obj };
        }
        objects[obj] = key;
    }

    public void UpdatePosition(Vector3 vector, T obj)
    {
        if (objects.ContainsKey(obj))
        {
            dict[objects[obj]].Remove(obj);
        }
        Insert(vector, obj);
    }

    public List<T> QueryPosition(Vector3 vector)
    {
        var key = Key(vector);
        return dict.ContainsKey(key) ? dict[key] : new List<T>();
    }

    public bool ContainsKey(Vector3 vector)
    {
        return dict.ContainsKey(Key(vector));
    }

    public void Clear()
    {
        var keys = dict.Keys.ToArray();
        for (var i = 0; i < keys.Length; i++)
            dict[keys[i]].Clear();
        objects.Clear();
    }

    public void Reset()
    {
        dict.Clear();
        objects.Clear();
    }

    private const int BIG_ENOUGH_INT = 16 * 1024;
    private const double BIG_ENOUGH_FLOOR = BIG_ENOUGH_INT + 0.0000;

    private static int FastFloor(float f)
    {
        return (int)(f + BIG_ENOUGH_FLOOR) - BIG_ENOUGH_INT;
    }

    private int Key(Vector3 v)
    {
        return ((FastFloor(v.x / cellSize) * 73856093) ^
                (FastFloor(v.y / cellSize) * 19349663) ^
                (FastFloor(v.z / cellSize) * 83492791));
    }
}