using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    // uniform distance between nodes
    public static float spacing = 2f;

    // four compass directions
    public static readonly Vector2[] directions =
    {
        // Right
        new Vector2(spacing, 0f),

        // Left
        new Vector2(-spacing, 0f),

        // Forward
        new Vector2(0f, spacing),

        // Barkward
        new Vector2(0f, -spacing)
    };

    // list of all of the Nodes on the Board
    List<Node> m_allNodes = new List<Node>();
    public List<Node> AllNodes { get { return m_allNodes; } }

    // the Node directly under the Player
    Node m_playerNode;
    public Node PlayerNode { get { return m_playerNode; } }

    // the Node representing the end of the maze
    Node m_goalNode;
    public Node GoalNode { get { return m_goalNode; } }

    // the List of Nodes representing all the subgoal nodes in the maze
    public List<Node> m_subGoalNodes;
    public List<Node> SubGoalNodes { get {return m_subGoalNodes; } }
    GameObject m_subGoalInstance;

    // current subgoal node index
    int m_currentSubGoalIndex = 0;
    public int CurrentSubGoalIndex { get {return m_currentSubGoalIndex; } set { m_currentSubGoalIndex = value; } }

    // iTween parameters for drawing the goal
    public GameObject goalPrefab;
    public float drawGoalTime = 2f;
    public float drawGoalDelay = 2f;
    public iTween.EaseType drawGoalEaseType = iTween.EaseType.easeOutExpo;

    // iTween parameters for drawing the subgoal
    public GameObject subGoalPrefab;
    public float drawSubGoalTime = 2f;
    public float drawSubGoalDelay = 2f;
    public iTween.EaseType drawSubGoalEaseType = iTween.EaseType.easeOutExpo;

    // the PlayerMover component
    PlayerMover m_player;

    // Video node
    public Node videoNode;

    // video player time
    public GameObject videoPlayer;

    // video end time
    public float videoEndTime;

    // bool for gate
    public bool isGateOpen = false;

    // Animator
    public Animator gateAnimator;

    // Ambient audio
    // public AudioSource ambientAudio;

    void Awake()
    {
        m_player = Object.FindObjectOfType<PlayerMover>().GetComponent<PlayerMover>();
        GetNodeList();

        m_goalNode = FindGoalNode();
        

        m_subGoalNodes = FindSubGoalNodes();
    }

    // sets the AllNodes and m_allNodes fields
    public void GetNodeList()
    {
        Node[] nList = Object.FindObjectsOfType<Node>();
        m_allNodes = new List<Node>(nList);
    }

    // returns a Node at a given position
    public Node FindNodeAt(Vector3 pos)
    {
        Vector2 boardCoord = Utility.Vector2Round(new Vector2(pos.x, pos.z));
        return m_allNodes.Find(n => n.Coordinate == boardCoord);
    }

    Node FindGoalNode()
    {
        return m_allNodes.Find(n => n.isLevelGoal);
    }


    List<Node> FindSubGoalNodes()
    {
        return m_allNodes.FindAll(n => n.isLevelSubGoal);
    }


    // return the PlayerNode
    public Node FindPlayerNode()
    {
        if (m_player != null && !m_player.isMoving)
        {
            return FindNodeAt(m_player.transform.position);
        }
        return null;
    }

    // set the m_playerNode
    public void UpdatePlayerNode()
    {
        m_playerNode = FindPlayerNode();

        if (m_playerNode.isPivotPoint)
        {
            m_playerNode.UpdateCamera();
        }
        
        if (m_playerNode.isGateOpener)
        {
            isGateOpen = true;
            OpenGate();
        }

        if (isGateOpen && m_playerNode == videoNode)
        {
            PlayVideo();
        }
    }

    // draw a colored sphere at the PlayerNode
    void OnDrawGizmos()
    {
        Gizmos.color = new Color(0f, 1f, 1f, 0.5f);
        if (m_playerNode != null)
        {
            Gizmos.DrawSphere(m_playerNode.transform.position, 0.2f);
        }
    }

    // draw the Goal prefab at the Goal Node
    public void DrawGoal()
    {
        if (goalPrefab != null && m_goalNode != null)
        {
            GameObject goalInstance = Instantiate(goalPrefab, m_goalNode.transform.position,
                                                  Quaternion.identity);
            iTween.ScaleFrom(goalInstance, iTween.Hash(
                "scale", Vector3.zero,
                "time", drawGoalTime,
                "delay", drawGoalDelay,
                "easetype", drawGoalEaseType
            ));
        }
    }

    // draw the Goal prefab at the Goal Node
    public void DrawSubGoal()
    {
        Debug.Log(m_subGoalNodes.Count);
        if (subGoalPrefab != null && m_subGoalNodes != null && m_subGoalNodes.Count > 0 && m_currentSubGoalIndex < m_subGoalNodes.Count) 
        {
            m_subGoalInstance = Instantiate(subGoalPrefab, m_subGoalNodes[m_currentSubGoalIndex].transform.position,
                                                     Quaternion.identity);
            iTween.ScaleFrom(m_subGoalInstance, iTween.Hash(
                "scale", Vector3.zero,
                "time", drawSubGoalTime,
                "delay", drawSubGoalDelay,
                "easetype", drawSubGoalEaseType
            ));
        }
    }

    public void ResetSubGoal()
    {
        Destroy(m_subGoalInstance);
        DrawSubGoal();
    }

    // start initializing the Nodes/drawing links
    public void InitBoard()
    {
        if (m_playerNode != null)
        {
            m_playerNode.InitNode();
            Node init_node = m_allNodes.Find(n => n.isInitNode);
            if (init_node != null)
            {
                init_node.InitNode();
            }
        }
    }

    public void OpenGate()
    {
        gateAnimator.SetBool("isOpen", true);
    }

    public void PlayVideo()
    {
        if (videoPlayer != null)
        {
            videoPlayer.SetActive(true);
            Destroy(videoPlayer, videoEndTime);
        }
    }
}