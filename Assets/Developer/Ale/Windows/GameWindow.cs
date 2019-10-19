public class GameWindow : UIControl
{
    public override UIControlName Name => UIControlName.InGame;
	
    /// <summary>
    /// Called when click home button
    /// </summary>
    public void OnHomeButton()
    {
        // Store fade class
        FadeBetweenScene fade = UIManager.Instance.Controls[UIControlName.Fade] as FadeBetweenScene;

        #region Local Method
        // Show level panel.
        void ShowLevel()
        {
            UIManager.Instance.Show(UIControlName.LevelSelection);
        }

        // Turn off fade panel
        void HideFade()
        {
            UIManager.Instance.Hide(UIControlName.Fade);
        }
        #endregion

        // Store into delegate.
        fade.OnFadeOutComplete = HideFade;
        fade.OnFadeInComplete = ShowLevel;

        // Turn on fade panel and disable this.
        UIManager.Instance.ShowAndHide(UIControlName.Fade, this);
    }

    /// <summary>
    /// Called when a level is completed.
    /// </summary>
    public void OnLevelCompleted()
    {
        // Store fade class
        FadeBetweenScene fade = UIManager.Instance.Controls[UIControlName.Fade] as FadeBetweenScene;

        #region Local Method
  
        // Turn off fade and enable game window.
        void ShowAndHideGameAndFade()
        {
            UIManager.Instance.ShowAndHide(this.Name, fade);
        }
        #endregion

        // Store into delegate.
        fade.OnFadeOutComplete = ShowAndHideGameAndFade;

        // Turn on fade panel and disable this.
        UIManager.Instance.ShowAndHide(UIControlName.Fade, this);
    }
}
