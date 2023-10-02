using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonSounds : Button
{
    private AudioClip hoverPressSound;
    private AudioClip upSound;

    private Button curButton;

    AudioChannelSettings buttonAudioSettings;

    public override void OnPointerEnter(PointerEventData eventData)
    {
        this.hoverPressSound = Resources.Load<AudioClip>("Audio/sfx_buttonClick");
        this.upSound = Resources.Load<AudioClip>("Audio/sfx_buttonClick2");

        this.buttonAudioSettings = new AudioChannelSettings(false, 1, 1, 1, "SFX");

        base.OnPointerEnter(eventData);

        AudioManager.instance.Play(this.hoverPressSound, this.buttonAudioSettings);
    }

    public override void OnSelect(BaseEventData eventData)
    {
        base.OnSelect(eventData);

        this.hoverPressSound = Resources.Load<AudioClip>("Audio/sfx_buttonClick");
        this.upSound = Resources.Load<AudioClip>("Audio/sfx_buttonClick2");

        this.buttonAudioSettings = new AudioChannelSettings(false, 1, 1, 1, "SFX");

        base.OnSelect(eventData);

        AudioManager.instance.Play(this.hoverPressSound, this.buttonAudioSettings);
    }
}

