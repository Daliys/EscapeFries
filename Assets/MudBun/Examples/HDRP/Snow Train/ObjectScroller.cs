/******************************************************************************/
/*
  Project   - MudBun
  Publisher - Long Bunny Labs
              http://LongBunnyLabs.com
  Author    - Ming-Lun "Allen" Chou
              http://AllenChou.net
*/
/******************************************************************************/

using System.Collections.Generic;

using UnityEngine;

namespace MudBun
{
  public class ObjectScroller : MonoBehaviour
  {
    public float ScrollSpeed = 20.0f;
    public Vector2 ScrollRange = new Vector2(-20.0f, 20.0f);

    public GameObject Sleeper;
    public int NumSleepers = 32;
    public float SleeperY = 0.0f;

    public GameObject Tree;
    public int NumTrees = 16;
    public float TreePositionDisturbance = 0.5f;
    public float TreeRotationDisturbance = 15.0f;
    public Vector2 TreeRange = new Vector2(2.0f, 8.0f);
    public float TreeY = 0.0f;

    private List<GameObject> m_aObj;

    private void Init()
    {
      if (m_aObj != null)
      {
        foreach (var obj in m_aObj)
          Destroy(obj);

        m_aObj.Clear();
      }
      else
      {
        m_aObj = new List<GameObject>();
      }

      float zStart = ScrollRange.y;
      float zRange = ScrollRange.y - ScrollRange.x;

      if (Sleeper != null)
      {
        float sleeperInterval = zRange / NumSleepers;
        for (int i = 0; i < NumSleepers; ++i)
        {
          var sleeper = Instantiate(Sleeper);
          sleeper.transform.position = new Vector3(0.0f, SleeperY, zStart - i * sleeperInterval);
          sleeper.transform.parent = transform;
          m_aObj.Add(sleeper);
        }
      }

      if (Tree != null)
      {
        float treeInterval = zRange / NumTrees;
        for (int i = 0; i < NumSleepers; ++i)
        {
          var tree = Instantiate(Tree);
          Vector3 offset = new Vector3((Random.value > 0.5f ? 1.0f : -1.0f) * Random.Range(TreeRange.x, TreeRange.y) + Random.Range(-TreePositionDisturbance, TreePositionDisturbance), TreeY, Random.Range(-TreePositionDisturbance, TreePositionDisturbance));
          tree.transform.position = new Vector3(0.0f, TreeY, zStart - i * treeInterval) + offset;
          float t = 2.0f * Mathf.PI * Random.value;
          tree.transform.rotation = Quaternion.AngleAxis(Random.value * TreeRotationDisturbance, new Vector3(Mathf.Cos(t), 0.0f, Mathf.Sin(t))) * Quaternion.AngleAxis(Random.value * 360.0f, Vector3.up);
          tree.transform.parent = transform;
          m_aObj.Add(tree);
        }
      }
    }

    private void OnValidate()
    {
      if (Application.isPlaying)
        Init();
    }

    private void Start()
    {
      Init();
    }

    private void Update()
    {
      foreach (var obj in m_aObj)
      {
        obj.transform.position += new Vector3(0.0f, 0.0f, -ScrollSpeed * Time.deltaTime);
        if (obj.transform.position.z < ScrollRange.x)
          obj.transform.position += new Vector3(0.0f, 0.0f, ScrollRange.y - ScrollRange.x);
      }
    }
  }
}

