using UnityEngine;

public class ButtonSound : MonoBehaviour
{
    [SerializeField]
    private AudioClip highlighted;
    [SerializeField]
    private AudioClip pressed;

    public void OnHighlight()
    {
        AudioController.UIPlayer.PlayOneShot(highlighted);
    }

    public void OnPress()
    {
        AudioController.UIPlayer.PlayOneShot(pressed);
    }
}
