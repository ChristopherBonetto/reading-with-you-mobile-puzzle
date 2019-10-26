using UnityEngine;

public class TouchManager : Singleton<TouchManager>
{
    #region Variables

	private bool m_isHolding;

    #endregion

    #region Core loop

	void Update()
	{
		if (GameManager.Instance.CurrentState != GameState.Playing)
		{
			return;
		}

		if (Input.GetMouseButtonDown(0))
		{
			if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit testHit))
			{
				Block testBlock = testHit.collider.GetComponent<Block>();
				if (testBlock)
				{
					m_isHolding = true;
					BlockManager.Instance.StartDrag(testBlock);
				}
				else
				{
					Tap(testHit);
				}
			}
		}

		if (Input.GetMouseButton(0) && m_isHolding)
		{
			BlockManager.Instance.Move();
		}

		if (Input.GetMouseButtonUp(0) && m_isHolding)
		{
			BlockManager.Instance.Release();
			m_isHolding = false;
		}
	}

	#endregion

	#region Touch

	private void Tap(RaycastHit hitted)
	{
        if(hitted.transform.GetComponent<PlayerActions>() || hitted.transform.GetComponent<FinalObjectActions>())
        {
            if (CanStart())
            {
                GameManager.Instance.StartWalkingPlayer();
                GameManager.Instance.FinalObject.EnableDisableCollider(false);
            }
        }
	}

	private bool CanStart()
	{
		return (!m_isHolding &&
				GameManager.Instance.CurrentState != GameState.Moving &&
				BlockManager.Instance.UnstableBlocks <= 0);
	}

	#endregion
}
