using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioTest : MonoBehaviour
{
    [SerializeField]
    private AudioClip bgm;
    private AudioChannelSettings bgmSettings;
    [SerializeField]
    private AudioClip oneOffSound;
    private AudioChannelSettings oneOffSettings;
    [SerializeField]
    private AudioClip loopingSound;
    private AudioChannelSettings loopingSettings;

    private int loopingChannelId = 0;
    private int pingPongChannelId = 0;
    private int periodicChannelId = 0;

    // Start is called before the first frame update
    void Start()
    {
        this.bgmSettings = new AudioChannelSettings(true, 1, 1, 1, "BGM");
        this.oneOffSettings = new AudioChannelSettings(false, 0.8f, 1.2f, 1.0f, "SFX", this.transform);
        this.loopingSettings = new AudioChannelSettings(true, 0.8f, 1.2f, 1.0f, "SFX", this.transform);

        AudioManager.instance.Play(this.bgm, this.bgmSettings);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            AudioManager.instance.Play(this.oneOffSound, this.oneOffSettings);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            this.loopingChannelId = AudioManager.instance.Play(this.loopingSound, this.loopingSettings);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            AudioManager.instance.PlaySlideUp(this.loopingSound, this.loopingSettings, 3.0f);            
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            AudioManager.instance.PlaySlideUp(this.loopingSound, this.loopingSettings, 1.0f);
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            AudioManager.instance.PlaySlideDown(this.loopingSound, this.loopingSettings, 10.0f);
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            AudioManager.instance.PlaySlideDown(this.loopingSound, this.loopingSettings, 0.5f);
        }
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            this.pingPongChannelId = AudioManager.instance.PlayPingPong(this.loopingSound, this.loopingSettings, 2.0f);
        }
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            this.pingPongChannelId = AudioManager.instance.PlayPingPong(this.loopingSound, this.loopingSettings, 0.2f);
        }
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            this.periodicChannelId = AudioManager.instance.PlayPeriodically(this.oneOffSound, this.oneOffSettings, 0.1f);
        }

        if (Input.GetKeyUp(KeyCode.Alpha2))
        {
            AudioManager.instance.Stop(this.loopingChannelId);
        }
        if (Input.GetKeyUp(KeyCode.Alpha7) || Input.GetKeyUp(KeyCode.Alpha8))
        {
            AudioManager.instance.Stop(this.pingPongChannelId);
        }
        if (Input.GetKeyUp(KeyCode.Alpha9))
        {
            AudioManager.instance.Stop(this.periodicChannelId);
        }
    }
}
