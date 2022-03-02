using UnityEngine;
using UnityEngine.UI;

public class PromptScript : MonoBehaviour {

    public Button ConfirmButton;
    public Button CancelButton;
    public Text Message;

    public void DestroyPrompt()
    {
        Destroy(this.gameObject);
    }


}
