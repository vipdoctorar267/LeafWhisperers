using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Revive : MonoBehaviour
{
    private UIManager _UIManager;
    private TeleportController _teleCtrl;
    private CharacterStateMachine _charStateMachine;
    private PlayerManager _playerManager;
    // Start is called before the first frame update
    void Start()
    {
        _UIManager = FindObjectOfType<UIManager>();
        _teleCtrl = FindObjectOfType<TeleportController>();
        _charStateMachine = FindObjectOfType<CharacterStateMachine>();
        _playerManager = FindObjectOfType<PlayerManager>();
    }

    // Update is called once per frame
   public void CallRevive()
    {
        _teleCtrl.TeleportToLastPoint();
        _playerManager._playerData._currentHealth = (int)(_playerManager._playerData._maxHealth * 0.3f);
        _charStateMachine.isDead = false;
        _playerManager.SavePlayerData();
        _UIManager.SetPanelState(UIManager.PanelState.InGUI);

    }
}
