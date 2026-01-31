using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Turning;

public class SetTurnTypeFromPlayerPref : MonoBehaviour
{
    public SnapTurnProvider snapTurn;
    public ContinuousTurnProvider continuousTurn;

    // Start is called before the first frame update
    void Start()
    {
        ApplyPlayerPref();
    }

    public void ApplyPlayerPref()
    {
        if(PlayerPrefs.HasKey("turn"))
        {
            int value = PlayerPrefs.GetInt("turn");
            if(value == 0)
            {
                snapTurn.leftHandTurnInput.inputAction.Enable();
                snapTurn.rightHandTurnInput.inputAction.Enable();
                continuousTurn.leftHandTurnInput.inputAction.Disable();
                continuousTurn.rightHandTurnInput.inputAction.Disable();
            }
            else if(value == 1)
            {
                snapTurn.leftHandTurnInput.inputAction.Disable();
                snapTurn.rightHandTurnInput.inputAction.Disable();
                continuousTurn.leftHandTurnInput.inputAction.Enable();
                continuousTurn.rightHandTurnInput.inputAction.Enable();
            }
        }
    }
}
