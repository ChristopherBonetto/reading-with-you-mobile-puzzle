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

		// Detect start drag or tap
		if (Input.GetMouseButtonDown(0) && !m_isHolding)
		{
			if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit testHit))
			{
				Block testBlock = testHit.collider.GetComponent<Block>();
				if (testBlock && BlockManager.Instance.UnstableBlocks <= 0)
				{
					m_isHolding = BlockManager.Instance.StartDrag(testBlock);
				}
				else
				{
					Tap(testHit);
				}
			}
		}

		// Detect move
		if (Input.GetMouseButton(0) && m_isHolding)
		{
			BlockManager.Instance.Move();
		}

		// Detect release
		if (Input.GetMouseButtonUp(0) && m_isHolding)
		{
			BlockManager.Instance.Release();
			m_isHolding = false;
		}
	}

	#endregion

	#region Touch

	/// <summary>
	/// Detect tap on player or objective
	/// </summary>
	/// <param name="hitted">Tap hit</param>
	private void Tap(RaycastHit hitted)
	{
        if(hitted.transform.GetComponent<PlayerActions>() || hitted.transform.GetComponent<FinalObjectActions>())
        {
            if (CanStart())
            {
                UIManager.Instance.Controls[UIControlName.InGame].OnHide();
                GameManager.Instance.StartWalkingPlayer();
                GameManager.Instance.FinalObject.ToggleCollider(false);
            }
        }
	}

	/// <summary>
	/// Check player start condition
	/// </summary>
	/// <returns>True if player can start walking</returns>
	private bool CanStart()
	{
		return (!m_isHolding &&
				GameManager.Instance.CurrentState != GameState.Moving &&
				BlockManager.Instance.UnstableBlocks <= 0);
	}

	#endregion
}
