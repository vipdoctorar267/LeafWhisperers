using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FXAudioClipData
{
    public string name; 
    public AudioClip clip; 
    public bool loop; 
    public float clipVolume = 1f; 
}

public class InGameAudioManager : MonoBehaviour
{
    public static InGameAudioManager Instance { get; private set; }

    [SerializeField] private List<FXAudioClipData> _audioClips;
    [SerializeField] private float _globalVolume = 1f; // Âm lượng tổng quát từ 0 đến 1

    private FXAudioClipData _bulletAudioClip;
    private FXAudioClipData _coinAudioClip;

    private Dictionary<CharacterStateMachine.CharFxState, FXAudioClipData> _charAudioClipDict = new Dictionary<CharacterStateMachine.CharFxState, FXAudioClipData>();
    private Dictionary<RunSpider.RunSpiderState, FXAudioClipData> _runSpiderAudioClipDict = new Dictionary<RunSpider.RunSpiderState, FXAudioClipData>();
    private Dictionary<JumpSpider.JumpSpiderState, FXAudioClipData> _jumpSpiderAudioClipDict = new Dictionary<JumpSpider.JumpSpiderState, FXAudioClipData>();
    private Dictionary<ClimbSpider.ClimbSpiderState, FXAudioClipData> _climbSpiderAudioClipDict = new Dictionary<ClimbSpider.ClimbSpiderState, FXAudioClipData>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        InitializeAudioClipDict();
    }

    private void InitializeAudioClipDict()
    {
        // Lưu các AudioClip vào dictionary với các trạng thái tương ứng
        _charAudioClipDict[CharacterStateMachine.CharFxState.Walk] = GetClipData("CharWalkClip");
        _charAudioClipDict[CharacterStateMachine.CharFxState.Run] = GetClipData("CharRunClip");
        _charAudioClipDict[CharacterStateMachine.CharFxState.Jump] = GetClipData("CharJumpClip");
        _charAudioClipDict[CharacterStateMachine.CharFxState.Attack01] = GetClipData("CharAttack01Clip");
        _charAudioClipDict[CharacterStateMachine.CharFxState.Attack02] = GetClipData("CharAttack02Clip");
        _charAudioClipDict[CharacterStateMachine.CharFxState.Attack03] = GetClipData("CharAttack03Clip");
        _charAudioClipDict[CharacterStateMachine.CharFxState.Dash] = GetClipData("CharDashClip");
        _charAudioClipDict[CharacterStateMachine.CharFxState.OnDMG] = GetClipData("CharOnDMGClip");
        _charAudioClipDict[CharacterStateMachine.CharFxState.Dead] = GetClipData("CharDeadClip");
        //------------------------------------------------------------------------------------------
        _runSpiderAudioClipDict[RunSpider.RunSpiderState.Wander] = GetClipData("RunSpiderWanderClip");
        _runSpiderAudioClipDict[RunSpider.RunSpiderState.Chase] = GetClipData("RunSpiderChaseClip");
        _runSpiderAudioClipDict[RunSpider.RunSpiderState.Dash] = GetClipData("RunSpiderDashClip");
        _runSpiderAudioClipDict[RunSpider.RunSpiderState.OnDMG] = GetClipData("RunSpiderOnDMGClip");
        _runSpiderAudioClipDict[RunSpider.RunSpiderState.Dead] = GetClipData("RunSpiderDeadClip");
        //------------------------------------------------------------------------------------------
        _jumpSpiderAudioClipDict[JumpSpider.JumpSpiderState.Jump] = GetClipData("JumpSpiderJumpClip");
        _jumpSpiderAudioClipDict[JumpSpider.JumpSpiderState.Fall] = GetClipData("JumpSpiderFallClip");
        _jumpSpiderAudioClipDict[JumpSpider.JumpSpiderState.Dead] = GetClipData("JumpSpiderDeadClip");
        _jumpSpiderAudioClipDict[JumpSpider.JumpSpiderState.OnDMG] = GetClipData("JumpSpiderOnDMGClip");
        //------------------------------------------------------------------------------------------
        _climbSpiderAudioClipDict[ClimbSpider.ClimbSpiderState.Attack] = GetClipData("ClimbSpiderAttackClip");
        _climbSpiderAudioClipDict[ClimbSpider.ClimbSpiderState.OnDMG] = GetClipData("ClimbSpiderOnDMGClip");
        _climbSpiderAudioClipDict[ClimbSpider.ClimbSpiderState.Dead] = GetClipData("ClimbSpiderDeadClip");
        //-------------------------------------------------------------------------------------------
        _bulletAudioClip = GetClipData("ClimSpiderBulletClip"); 
        _coinAudioClip = GetClipData("CoinClip");     
    }

    //----------------------------------------------------------------------------------
    public void SetFXGlobalVolume(float volume)
    {
        _globalVolume = Mathf.Clamp01(volume); // Chuyển giá trị từ 0-100 thành 0-1
    }

    public float FXGlobalVolume
    {
        get { return _globalVolume; }
    }
    //---------------------------------------------------------------------------------
    public FXAudioClipData GetClipData(string clipName)
    {
        return _audioClips.Find(clipData => clipData.name == clipName);
    }

    public FXAudioClipData GetCharacterClip(CharacterStateMachine.CharFxState state)
    {
        if (_charAudioClipDict.TryGetValue(state, out FXAudioClipData clipData))
        {
            return clipData;
        }
        return null;
    }

    public FXAudioClipData GetRunSpiderClip(RunSpider.RunSpiderState state)
    {
        if (_runSpiderAudioClipDict.TryGetValue(state, out FXAudioClipData clipData))
        {
            return clipData;
        }
        return null;
    }

    public FXAudioClipData GetJumpSpiderClip(JumpSpider.JumpSpiderState state)
    {
        if (_jumpSpiderAudioClipDict.TryGetValue(state, out FXAudioClipData clipData))
        {
            return clipData;
        }
        return null;
    }

    public FXAudioClipData GetClimbSpiderClip(ClimbSpider.ClimbSpiderState state)
    {
        if (_climbSpiderAudioClipDict.TryGetValue(state, out FXAudioClipData clipData))
        {
            return clipData;
        }
        return null;
    }
    //--------------------------------------------------------------------------------------------
    public void PlayCharacterSound(AudioSource audioSource, CharacterStateMachine.CharFxState state)
    {
        var clipData = GetCharacterClip(state);
        if (clipData != null && clipData.clip != null)
        {
            audioSource.PlayOneShot(clipData.clip, clipData.clipVolume * _globalVolume);
        }
    }

    public void PlayRunSpiderSound(AudioSource audioSource, RunSpider.RunSpiderState state)
    {
        var clipData = GetRunSpiderClip(state);
        if (clipData != null && clipData.clip != null)
        {
            audioSource.PlayOneShot(clipData.clip, clipData.clipVolume * _globalVolume);
        }
    }

    public void PlayJumpSpiderSound(AudioSource audioSource, JumpSpider.JumpSpiderState state)
    {
        var clipData = GetJumpSpiderClip(state);
        if (clipData != null && clipData.clip != null)
        {
            audioSource.PlayOneShot(clipData.clip, clipData.clipVolume * _globalVolume);
        }
    }

    public void PlayClimbSpiderSound(AudioSource audioSource, ClimbSpider.ClimbSpiderState state)
    {
        var clipData = GetClimbSpiderClip(state);
        if (clipData != null && clipData.clip != null)
        {
            audioSource.PlayOneShot(clipData.clip, clipData.clipVolume * _globalVolume);
        }
    }


    //--------------------------------------------------------------------------------------------
    public void PlayBulletSound(AudioSource audioSource)
    {
        if (_bulletAudioClip != null && _bulletAudioClip.clip != null)
        {
            audioSource.PlayOneShot(_bulletAudioClip.clip, _bulletAudioClip.clipVolume * _globalVolume);
        }
    }

    public void PlayCoinCollectSound(AudioSource audioSource)
    {
        if (_coinAudioClip != null && _coinAudioClip.clip != null)
        {
            audioSource.PlayOneShot(_coinAudioClip.clip, _coinAudioClip.clipVolume * _globalVolume);
        }
    }
}
