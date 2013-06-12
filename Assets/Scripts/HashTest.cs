using UnityEngine;
using System.Collections.Generic;

public class HashTest : MonoBehaviour
{
    public Transform point;
    public Transform yellowCube;
    public Transform blueCube;
    public int numberOfPoints = 1;
    public int sphereRadius = 10;
    public int cellSize = 1;

    private SpatialHash<Transform> hash;
    private Transform[] clones;
    private Transform clone;
    private int wireSize;
    private List<Transform> yellowCubes = new List<Transform>();
    private List<Transform> blueCubes = new List<Transform>();

	void Start ()
    {
        hash = new SpatialHash<Transform>(cellSize);
        clones = new Transform[numberOfPoints];
        wireSize = Mathf.CeilToInt(sphereRadius / cellSize) * cellSize + cellSize / 2;
        for (var i = 0; i < numberOfPoints; i++)
        {
            clone = Instantiate(point, Random.insideUnitSphere * sphereRadius, Quaternion.identity) as Transform;
            if (clone != null)
            {
                hash.Insert(clone.position, clone);
                clone.parent = transform;
                clones[i] = clone;
            }
        }
        InvokeRepeating("MovePoint", 0, 0.5f);
        InvokeRepeating("ClearHash", 10, 10);

        for (var x = -wireSize; x <= wireSize; x++)
        {
            for (var y = -wireSize; y <= wireSize; y++)
            {
                for (var z = -wireSize; z <= wireSize; z++)
                {
                    yellowCubes.Add(Instantiate(yellowCube, new Vector3(x, y, z), Quaternion.identity) as Transform);
                    blueCubes.Add(Instantiate(blueCube, new Vector3(x, y, z), Quaternion.identity) as Transform);
                }
            }
        }
	}
	
	void Update ()
	{
	    for (var i = 0; i < yellowCubes.Count; i++)
	    {
            if (hash.QueryPosition(yellowCubes[i].position).Count > 0)
            {
                yellowCubes[i].renderer.enabled = true;
            }
            else
            {
                yellowCubes[i].renderer.enabled = false;
            }
	    }
        for (var i = 0; i < blueCubes.Count; i++)
        {
            if (hash.ContainsKey(blueCubes[i].position) && hash.QueryPosition(blueCubes[i].position).Count == 0)
            {
                blueCubes[i].renderer.enabled = true;
            }
            else
            {
                blueCubes[i].renderer.enabled = false;
            }
        }
	}

    void MovePoint()
    {
        for (var i = 0; i < clones.Length; i++)
        {
            clones[i].position = Random.insideUnitSphere * sphereRadius;
            hash.UpdatePosition(clones[i].position, clones[i]);
        }
    }

    void ClearHash()
    {
        hash.Reset();
    }
}
