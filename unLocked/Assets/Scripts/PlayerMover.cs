using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMover : Mover
{
    // reference to visual arrows
    PlayerCompass m_playerCompass;
    GameObject m_playerFigure;


    // invoke the base class Awake method and setup the PlayerMover
    protected override void Awake()
    {
        base.Awake();
        m_playerCompass = Object.FindObjectOfType<PlayerCompass>().GetComponent<PlayerCompass>();

        // PlayerMover always face the direction they are moving
        faceDestination = true;

        m_playerFigure = transform.Find("Tyler").gameObject;

    }

    protected override void Start()
    {
        base.Start();
		UpdateBoard();
    }

    // update the Board's PlayerNode
    void UpdateBoard()
    {
        if (m_board != null)
        {
            m_board.UpdatePlayerNode();
        }
    }

    protected override IEnumerator MoveRoutine(Vector3 destinationPos, float delayTime)
    {
        // disable PlayerCompass arrows
		if (m_playerCompass != null)
		{
			m_playerCompass.ShowArrows(false);
		}

        m_playerFigure.SetActive(true);

        // run the parent class MoveRoutine
        yield return StartCoroutine(base.MoveRoutine(destinationPos, delayTime));

        // update the Board's PlayerNode
		UpdateBoard();

        // enable PlayerCompass arrows
		if (m_playerCompass != null)
		{
			m_playerCompass.ShowArrows(true);
		}

        if (m_board.PlayerNode.isStealthNode)
        {
            m_playerFigure.SetActive(false);
        }

        base.finishMovementEvent.Invoke();
    }

    // teleport to another node
    public void Teleportation()
    {
        if (m_board.PlayerNode.isTeleportationNode)
        {
            Vector3 newPosition = m_board.PlayerNode.teleportationDestination.transform.position;
            Move(newPosition, 0, true);
        }
    }
}