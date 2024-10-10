using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource clipAudioSource;
    public AudioSource bgAudioSource;
    public AudioSource specialAudioSource;

    [SerializeField] private AudioClipStore myAudioClipStore;

    [System.Serializable]
    private struct AudioClipStore
    {
        public AudioClip buttonClick;
        public AudioClip coinAdd;
        public AudioClip objectSelect;
        public AudioClip undoClick;
        public AudioClip spinning;
        public AudioClip welcomeTable;
        public AudioClip noMoreBet;
        public AudioClip spinningDeflectStage;
        public AudioClip spinningLastStage;
    }


    public static AudioManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            Debug.Log($"Audio Manager Instance Done");
        }
        else Destroy(instance);
    }

    private void PlayAudioClip(AudioClip givenAudio, AudioSource givenAudioSource)
    {
        givenAudioSource.PlayOneShot(givenAudio);
    }

    public void PlayWelcomeClip() => PlayAudioClip(myAudioClipStore.welcomeTable, specialAudioSource);
    public void PlayNoMoreBetClip() => PlayAudioClip(myAudioClipStore.noMoreBet, specialAudioSource);
    public void PlayDeflectSound() => PlayAudioClip(myAudioClipStore.spinningDeflectStage, specialAudioSource);
    public void PlaySpinLastStageSound() => PlayAudioClip(myAudioClipStore.spinningLastStage, specialAudioSource);
    public void StopSpinLastStageSound()
    {
        specialAudioSource.Stop();
        specialAudioSource.clip = null;
    }

    public void PlayButtonClick() => PlayAudioClip(myAudioClipStore.buttonClick, clipAudioSource);
    public void PlayCoinAdd() => PlayAudioClip(myAudioClipStore.coinAdd, clipAudioSource);
    public void PlayObjectSelect() => PlayAudioClip(myAudioClipStore.objectSelect, clipAudioSource);
    public void PlayUndoClick() => PlayAudioClip(myAudioClipStore.undoClick, clipAudioSource);
    public void PlayBackGround() => bgAudioSource.Play();
    public void PlayAndStopSpinning(bool isPlay)
    {
        clipAudioSource.clip = isPlay ? myAudioClipStore.spinning : null;
        if (isPlay) clipAudioSource.Play();
    }


    public void MuteUnmuteSound(bool isSoundMute)
    {
        clipAudioSource.mute = isSoundMute;
        specialAudioSource.mute = isSoundMute;
    }
    public void MuteUnmuteMusic(bool isMusicMute)
    {
        bgAudioSource.mute = isMusicMute;
    }





}
