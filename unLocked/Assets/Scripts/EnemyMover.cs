using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MovementType
{
    Stationary,
    Patrol,
    Spinner,
	Shuttle,
    Circular
}

public enum MovementDirection
{
	Right,
	Left,
	Forward,
	Backward
}

public class EnemyMover : Mover
{
    // local direction to move (defaults to local positive z)
    public Vector3 directionToMove = new Vector3(0f, 0f, Board.spacing);

    // movement mode
    public MovementType movementType = MovementType.Stationary;

    // wait time for stationary enemies
    public float standTime = 0.5f;

	// list of predefined path that the grunt moves in 
	public MovementDirection[] shuttleDirections;

	// the next node in the predefined shuttle path
	protected int m_directionIndex = 0;

	// check if the forward movement in the shuttle
	protected bool m_forwardIteration = true;


    protected override void Awake()
    {
        base.Awake();

        // EnemyMovers always face the direction they are moving
        faceDestination = true;
    }

    protected override void Start()
    {
        base.Start();
    }

    // complete one turn of movement
    public void MoveOneTurn()
    {
        switch (movementType)
        {
            case MovementType.Patrol:
                Patrol();
                break;
            case MovementType.Stationary:
				Stand();
                break;
            case MovementType.Spinner:
                Spin();
                break;
            case MovementType.Shuttle:
                Shuttle();
                break;
            case MovementType.Circular:
                Circle();
                break;
        }
    }

    void Patrol()
    {
        StartCoroutine(PatrolRoutine());
    }

    IEnumerator PatrolRoutine()
    {
        // cache our starting position
        Vector3 startPos = new Vector3(m_currentNode.Coordinate.x, 0f, m_currentNode.Coordinate.y);

        // one space forward
        Vector3 newDest = startPos + transform.TransformVector(directionToMove);

        // two spaces forward
        Vector3 nextDest = startPos + transform.TransformVector(directionToMove * 2f);

        // move to our new destination
        Move(newDest, 0f);

        // pause until we complete the movement
        while (isMoving)
        {
			yield return null; 
        }

        // check if we have reached a deadend
        if (m_board != null)
        {
            // our destination Node
            Node newDestNode = m_board.FindNodeAt(newDest);

            // the Node two spaces away
            Node nextDestNode = m_board.FindNodeAt(nextDest);

            // if the Node two spaces away does not exist OR is not connected to our destination Node...
            if (nextDestNode == null || !newDestNode.LinkedNodes.Contains(nextDestNode))
            {
                // turn to face our original Node and set that as our new destination
                destination = startPos;
                FaceDestination();

                // wait until we are done rotating
                yield return new WaitForSeconds(rotateTime);
            }
        }

		// broadcast message at end of movement
		base.finishMovementEvent.Invoke();
    }

    // movement turn for stationary enemies
    void Stand()
    {
        StartCoroutine(StandRoutine());
    }

    // routine for stationary movement
    IEnumerator StandRoutine()
    {
        // time to wait
        yield return new WaitForSeconds(standTime);

        // broadcast message at end of movement
        base.finishMovementEvent.Invoke();
    }

    void Spin()
    {
        StartCoroutine(SpinRoutine());
    }

    IEnumerator SpinRoutine()
    {
        // local z forward
        Vector3 localForward = new Vector3(0f, 0f, Board.spacing);

        // destination is always one space directly behind us
        destination = transform.TransformVector(localForward * -1f) + transform.position;

        // rotate 180 degrees
        FaceDestination();

        // wait for rotation to finish
        yield return new WaitForSeconds(rotateTime);

		// broadcast message at end of movement
		base.finishMovementEvent.Invoke();
    }

	void Shuttle()
	{
		StartCoroutine(ShuttleRoutine());
	}

	IEnumerator ShuttleRoutine()
	{
		MovementDirection nextDirection = shuttleDirections[m_directionIndex];

		if (nextDirection == MovementDirection.Right)
		{
			if (m_forwardIteration)
			{
				MoveRight();
			}
			else
			{
				MoveLeft();
			}
		}
		else if (nextDirection == MovementDirection.Left)
		{
			if (m_forwardIteration)
			{
				MoveLeft();
			}
			else
			{
				MoveRight();
			}
		}
		else if (nextDirection == MovementDirection.Forward)
		{
			if (m_forwardIteration)
			{
				MoveForward();
			}
			else
			{
				MoveBackward();
			}
		}
		else if (nextDirection == MovementDirection.Backward)
		{
			if (m_forwardIteration)
			{
				MoveBackward();
			}
			else
			{
				MoveForward();
			}
		}


		if ((m_forwardIteration && m_directionIndex == (shuttleDirections.Length - 1)) || (!m_forwardIteration && m_directionIndex == 0))
		{
			m_forwardIteration = !m_forwardIteration;
		}
		else 
		{
            if (m_forwardIteration)
            {
                m_directionIndex = m_directionIndex + 1;
            }
            else
            {
                m_directionIndex = m_directionIndex - 1;
            }
		}

        while (isMoving)
        {
            yield return null;
        }

        // Experimental code
        nextDirection = shuttleDirections[m_directionIndex];

        if (m_forwardIteration)
        {
            switch(nextDirection)
            {
                case MovementDirection.Forward:
                    destination = m_currentNode.transform.position + new Vector3(Board.directions[2].x, 0f,
                                            Board.directions[2].y);
                    break;

                case MovementDirection.Backward:
                    destination = m_currentNode.transform.position + new Vector3(Board.directions[3].x, 0f,
                                            Board.directions[3].y);
                    break;

                case MovementDirection.Right:
                    destination = m_currentNode.transform.position + new Vector3(Board.directions[0].x, 0f,
                                            Board.directions[0].y);
                    break;

                case MovementDirection.Left:
                    destination = m_currentNode.transform.position + new Vector3(Board.directions[1].x, 0f,
                                            Board.directions[1].y);
                    break;
            }
        }
        else 
        {
            switch (nextDirection)
            {
                case MovementDirection.Forward:
                    destination = m_currentNode.transform.position + new Vector3(Board.directions[3].x, 0f,
                                            Board.directions[3].y);
                    break;

                case MovementDirection.Backward:
                    destination = m_currentNode.transform.position + new Vector3(Board.directions[2].x, 0f,
                                            Board.directions[2].y);
                    break;

                case MovementDirection.Right:
                    destination = m_currentNode.transform.position + new Vector3(Board.directions[1].x, 0f,
                                            Board.directions[1].y);
                    break;

                case MovementDirection.Left:
                    destination = m_currentNode.transform.position + new Vector3(Board.directions[0].x, 0f,
                                            Board.directions[0].y);
                    break;
            }
        }
        FaceDestination();

        yield return new WaitForSeconds(rotateTime);
		base.finishMovementEvent.Invoke();
	}
	
    void Circle()
	{
		StartCoroutine(CircleRoutine());
	}

	IEnumerator CircleRoutine()
	{
		MovementDirection nextDirection = shuttleDirections[m_directionIndex];

		if (nextDirection == MovementDirection.Right)
		{
            MoveRight();
		}
		else if (nextDirection == MovementDirection.Left)
		{
            MoveLeft();
		}
		else if (nextDirection == MovementDirection.Forward)
		{
            MoveForward();
		}
		else if (nextDirection == MovementDirection.Backward)
		{
            MoveBackward();
		}


        m_directionIndex = (m_directionIndex + 1) % shuttleDirections.Length;

        while (isMoving)
        {
            yield return null;
        }

        // Experimental code
        nextDirection = shuttleDirections[m_directionIndex];

        switch(nextDirection)
        {
            case MovementDirection.Forward:
                destination = m_currentNode.transform.position + new Vector3(Board.directions[2].x, 0f,
                                        Board.directions[2].y);
                break;

            case MovementDirection.Backward:
                destination = m_currentNode.transform.position + new Vector3(Board.directions[3].x, 0f,
                                        Board.directions[3].y);
                break;

            case MovementDirection.Right:
                destination = m_currentNode.transform.position + new Vector3(Board.directions[0].x, 0f,
                                        Board.directions[0].y);
                break;

            case MovementDirection.Left:
                destination = m_currentNode.transform.position + new Vector3(Board.directions[1].x, 0f,
                                        Board.directions[1].y);
                break;
        } 
        FaceDestination();

        yield return new WaitForSeconds(rotateTime);
		base.finishMovementEvent.Invoke();
	}

}