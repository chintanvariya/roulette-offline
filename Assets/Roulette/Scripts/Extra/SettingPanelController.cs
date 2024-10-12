using System;
using UnityEngine;
namespace RouletteByFinix
{
    public class SettingPanelController : MonoBehaviour
    {
        [SerializeField] private Sprite onSprite;
        [SerializeField] private Sprite offSprite;

        private Action SettingPanelVerify;
        public static Action VibrateAction;

        private void Start()
        {
            soundData.onOffButton.onClick.AddListener(OnClickSoundButton);
            vibrateData.onOffButton.onClick.AddListener(OnClickVibrateButton);
            musicData.onOffButton.onClick.AddListener(OnClickMusicButton);
        }

        private void OnEnable()
        {
            SettingPanelVerify += CheckPlayerPrefsSound;
            SettingPanelVerify += CheckPlayerPrefsVibration;
            SettingPanelVerify += CheckPlayerPrefsMusic;
            VibrateAction += Vibrate;
            SettingPanelVerify?.Invoke();
        }

        private void OnDisable()
        {
            SettingPanelVerify -= CheckPlayerPrefsSound;
            SettingPanelVerify -= CheckPlayerPrefsVibration;
            SettingPanelVerify -= CheckPlayerPrefsMusic;
            VibrateAction -= Vibrate;
        }

        public bool sound
        {
            get { return (PlayerPrefs.GetString("Sound") == "True"); }
            set { PlayerPrefs.SetString("Sound", value.ToString()); }
        }

        public bool vibrate
        {
            get { return (PlayerPrefs.GetString("Vibrate") == "True"); }
            set { PlayerPrefs.SetString("Vibrate", value.ToString()); }
        }

        public bool music
        {
            get { return (PlayerPrefs.GetString("Music") == "True"); }
            set { PlayerPrefs.SetString("Music", value.ToString()); }
        }


        [System.Serializable]
        public struct Data
        {
            public Sprite onSprite;
            public Sprite offSprite;
            public UnityEngine.UI.Image iconImage;
            public UnityEngine.UI.Button onOffButton;
        }

        [Header(" ==== SOUND ==== ")]
        [SerializeField] private Data soundData;

        [Header(" ==== VIBRATE ==== ")]
        [SerializeField] private Data vibrateData;

        [Header(" ==== MUSIC ==== ")]
        [SerializeField] private Data musicData;

        public void OnClickSoundButton()
        {
            Debug.Log("OnClickSoundButton");
            SetValueForSound(!sound, sound ? soundData.offSprite : soundData.onSprite, sound ? offSprite : onSprite);
            AudioManager.instance.PlayObjectSelect();
        }

        public void OnClickVibrateButton()
        {
            Debug.Log("OnClickVibrateButton");
            SetValueForVibration(!vibrate, vibrate ? vibrateData.offSprite : vibrateData.onSprite, vibrate ? offSprite : onSprite);
            if (vibrate)
            {
                Debug.Log($"vibrate ON");
                Vibrate();
            }
        }

        public void OnClickMusicButton()
        {
            Debug.Log("OnClickMusicButton");
            SetValueForMusic(!music, music ? musicData.offSprite : musicData.onSprite, music ? offSprite : onSprite);
        }

        public void SetValueForSound(bool onOffText, Sprite onOffIconSprite, Sprite onOffSprite)
        {
            Debug.Log("SetValueForSound");
            //soundData.soundIconImage.sprite = onOffIconSprite;
            // soundData.soundOnOffText.text = onOffText;
            soundData.onOffButton.image.sprite = onOffSprite;
            sound = onOffText;
            Debug.Log($"Audiomanager instance {AudioManager.instance}");
            AudioManager.instance.MuteUnmuteSound(!sound);
        }

        public void SetValueForVibration(bool onOffText, Sprite onOffIconSprite, Sprite onOffSprite)
        {
            Debug.Log("SetValueForVibration");
            // vibrateData.vibrateIconImage.sprite = onOffIconSprite;
            // vibrateData.vibrateOnOffText.text = onOffText;
            vibrateData.onOffButton.image.sprite = onOffSprite;
            vibrate = onOffText;
        }

        public void SetValueForMusic(bool onOffText, Sprite onOffIconSprite, Sprite onOffSprite)
        {
            Debug.Log("SetValueForMusic");
            // emojiData.emojiIconImage.sprite = onOffIconSprite;
            // musicData.musicOnOffText.text = onOffText;
            musicData.onOffButton.image.sprite = onOffSprite;
            music = onOffText;
            AudioManager.instance.MuteUnmuteMusic(!music);
        }

        public void CheckPlayerPrefsSound()
        {
            if (PlayerPrefs.HasKey("Sound"))
            {
                Debug.Log("CheckPlayerPrefsSound || HasKey || " + sound);
                SetValueForSound(sound, sound ? soundData.onSprite : soundData.offSprite, sound ? onSprite : offSprite);
            }
            else
            {
                SetValueForSound(true, soundData.onSprite, onSprite);
            }
        }

        public void CheckPlayerPrefsVibration()
        {
            if (PlayerPrefs.HasKey("Vibrate"))
            {
                Debug.Log("CheckPlayerPrefsVibration || HasKey || " + vibrate);
                SetValueForVibration(vibrate, vibrate ? vibrateData.onSprite : vibrateData.offSprite, vibrate ? onSprite : offSprite);
            }
            else
            {
                Debug.Log("vibration key || " + vibrate);
                SetValueForVibration(true, vibrateData.onSprite, onSprite);
            }
        }

        public void CheckPlayerPrefsMusic()
        {
            if (PlayerPrefs.HasKey("Music"))
            {
                Debug.Log("CheckPlayerPrefsEmoji || HasKey || " + music);
                SetValueForMusic(music, music ? musicData.onSprite : musicData.offSprite, music ? onSprite : offSprite);
            }
            else
            {
                SetValueForMusic(true, musicData.onSprite, onSprite);
            }
        }

        // Vibration 
        public void Vibrate()
        {
            if (vibrate)
                RouletteVibration.Vibrate(50);
        }
    }
}
