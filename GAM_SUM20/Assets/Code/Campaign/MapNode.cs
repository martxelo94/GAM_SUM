using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;

public class MapNode : MonoBehaviour
{
    public TeamType team;
    public MapNode[] nextNodes;
    public List<MapNode> parentNodes;
    //[HideInInspector]
    public int node_idx = -1;  //set by MapCampaign to proper game progresion
    private LineRenderer[] linksInstantiated;
    public MapCampaign map;
    Vector3 initScale;
    // army stuff
    public Deck army;
    [Range(0, 2)]
    public int army_model_idx = 0;

    // reward for capturing
    public CardTypeCount[] deck_reward;

    private void Awake()
    {
        //map = FindObjectOfType<MapCampaign>();
        parentNodes = new List<MapNode>();
        Assert.IsTrue(deck_reward != null && deck_reward.Length == (int)CardType.CardType_Count);
    }

    // Start is called before the first frame update
    void Start()
    {
        Assert.IsTrue(map != null); // set by MapCampaign
        Assert.IsTrue(node_idx != -1);
        UpdateLinks();
        SetTeamColor();
        // opponent armies face parent node
        for(int i = 0; i < nextNodes.Length; ++i) {
            MapNode n = nextNodes[i];
            n.parentNodes.Add(this);
            if (n.army != null && n.army.team == TeamType.Opponent) {
                Vector3 dir = n.army.transform.position - transform.position;
                n.army.transform.up = -dir.normalized;
            }
        }
        initScale = transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        if (map.is_moving) {
            transform.localScale = initScale;
            return;
        }
        //if (is_selected && map.is_attaking)
        //{
        //    // highlight adyacents
        //    for (int i = 0; i < nextNodes.Length; ++i)
        //    {
        //        nextNodes[i].Pulse();
        //    }
        //}
        //else 
        if (team == TeamType.Player)
        {
            if (army != null)
            {
                if(army.cards_to_play_count == 0 || map.is_attacking)
                    Pulse();
                else {
                    // make children pulse
                    for (int i = 0; i < nextNodes.Length; ++i)
                    {
                        nextNodes[i].Pulse();
                    }
                }
            }

        }
    }

    private void OnMouseDown()
    {
        if (map.is_moving)
            return;

        map.SelectNode(this);
    }

    public void Pulse()
    {
        float t = Mathf.PingPong(Time.time, 0.5f);
        transform.localScale = initScale + initScale * t;
    }

    public void SetTeamColor()
    {
        Material mat_player = Resources.Load("Materials/M_Player") as Material;
        Material mat_opponent = Resources.Load("Materials/M_Opponent") as Material;
        switch (team) {
            case TeamType.Player:
                GetComponent<Renderer>().material = mat_player;
                for (int i = 0; i < nextNodes.Length; ++i)
                    linksInstantiated[i].startColor = mat_player.color;
                break;
            case TeamType.Opponent:
                GetComponent<Renderer>().material = mat_opponent;
                for (int i = 0; i < nextNodes.Length; ++i)
                    linksInstantiated[i].startColor = mat_opponent.color;
                break;
            default:
                break;
        }
        for (int i = 0; i < nextNodes.Length; ++i) {
            MapNode n = nextNodes[i];
            switch (n.team)
            {
                case TeamType.Player:
                    linksInstantiated[i].endColor = mat_player.color;
                    break;
                case TeamType.Opponent:
                    linksInstantiated[i].endColor = mat_opponent.color;
                    break;
                default:
                    break;
            }
        }
    }

    public void SpawnArmy()
    {
        Assert.IsTrue(army == null);
        GameObject army_obj = Instantiate(map.armyPrefabs[army_model_idx], transform);
        Assert.IsTrue(army_obj != null);
        army_obj.transform.position = transform.position;
        army_obj.name = "Army";
        army = army_obj.GetComponent<Deck>();
        Assert.IsTrue(army != null);
        army.team = team;
        //set look direction
        Vector3 dir = Vector3.zero;
        foreach (MapNode n in nextNodes) {
            dir += n.transform.position - transform.position;
        }
        army_obj.transform.up = dir.normalized;
    }

    public void DestroyArmy()
    {
        Assert.IsTrue(army != null);
        DestroyImmediate(army.gameObject);
        army = null;
    }

    // duplicated from MapCameraMovement
    bool IsTouchDown()
    {
#if UNITY_EDITOR
        return Input.GetMouseButtonDown(0);
#else
        return Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began;
#endif
    }

    public void Select()
    {
        transform.localScale = initScale * 1.5f;
        //for (int i = 0; i < nextNodes.Length; ++i)
        //{
        //    nextNodes[i].transform.localScale *= 1.5f;
        //}
    }

    public void Unselect()
    {
        transform.localScale = initScale;
        for (int i = 0; i < nextNodes.Length; ++i) {
            nextNodes[i].transform.localScale = nextNodes[i].initScale;
        }
    }


    public bool UpdateLinks()
    {
        if (linksInstantiated != null && linksInstantiated.Length == nextNodes.Length)
            return false;
        LineRenderer[] lines = GetComponentsInChildren<LineRenderer>();
        if (lines.Length > 0)
            foreach (LineRenderer l in lines)
                Destroy(l.gameObject);
        //if (linksInstantiated == null)
        CreateLinks();
        return true;
    }

    public void CreateLinks()
    {
        if(linksInstantiated != null)
            foreach (LineRenderer lr in linksInstantiated)
                    if(lr != null)
                        DestroyImmediate(lr.gameObject);
        linksInstantiated = new LineRenderer[nextNodes.Length];
        Material mat = Resources.Load("Materials/M_Line") as Material;
        for (int i = 0; i < nextNodes.Length; ++i) {
            GameObject lineObj = new GameObject();
            lineObj.name = "Link " + i.ToString();
            LineRenderer line = lineObj.AddComponent<LineRenderer>();
            line.transform.parent = transform;
            line.SetPosition(0, transform.position);
            line.SetPosition(1, nextNodes[i].transform.position);
            line.startWidth = transform.localScale.x;
            line.endWidth = nextNodes[i].transform.localScale.x * 0.5f;

            line.sharedMaterial = mat;
            linksInstantiated[i] = line;
        }

    }

    public void Set(NodeSaveData data)
    {
        Assert.IsTrue(name == data.name);
        team = data.team;
        army_model_idx = data.model_idx;
        if(army != null)
            DestroyArmy();
        //Assert.IsTrue(army_model_idx > -1);
        if(data.deck != null)
        {
            //if (army == null)
            {
                SpawnArmy();
            }
            army.SetDeck(data.deck);
        }
        //SetTeamColor();   // done at Start once
    }
}
