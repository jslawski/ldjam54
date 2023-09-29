using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [SerializeField]
    private GameObject audioChannelPrefab;

    private List<AudioChannel> playingAudioChannels;

    public int audioChannelCacheSize = 50;
    private Stack<AudioChannel> audioChannelCache;

    private int channelIdCounter = 0;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            this.CreateAudioChannelCache();
        }
    }

    private void CreateAudioChannelCache()
    {
        this.audioChannelCache = new Stack<AudioChannel>();
        this.playingAudioChannels = new List<AudioChannel>();

        for (int i = 0; i < this.audioChannelCacheSize; i++)
        {
            GameObject newAudioChannel = Instantiate(this.audioChannelPrefab, this.transform);
            this.audioChannelCache.Push(newAudioChannel.GetComponent<AudioChannel>());
            newAudioChannel.name = "Unused";
        }
    }

    private AudioChannel GetAudioChannel(AudioClip clip, AudioChannelSettings channelSettings)
    {
        if (this.audioChannelCache.Count < 1)
        {
            Debug.LogError("No Audio Channels available. Audio Channel not assigned.");
            return null;
        }
        
        AudioChannel newChannel = this.audioChannelCache.Pop();        
        newChannel.Setup(clip, channelSettings);        
        return newChannel;
    }

    public int Play(AudioClip clip, AudioChannelSettings channelSettings)
    {
        AudioChannel newAudioChannel = this.GetAudioChannel(clip, channelSettings);
        newAudioChannel.Play();
        this.playingAudioChannels.Add(newAudioChannel);
        return newAudioChannel.channelId;
    }

    public int PlaySlideUp(AudioClip clip, AudioChannelSettings channelSettings, float durationInSeconds)
    {
        AudioChannel newAudioChannel = this.GetAudioChannel(clip, channelSettings);
        newAudioChannel.PlaySlideUp(durationInSeconds);
        this.playingAudioChannels.Add(newAudioChannel);
        return newAudioChannel.channelId;
    }

    public int PlaySlideDown(AudioClip clip, AudioChannelSettings channelSettings, float durationInSeconds)
    {
        AudioChannel newAudioChannel = this.GetAudioChannel(clip, channelSettings);
        newAudioChannel.PlaySlideDown(durationInSeconds);
        this.playingAudioChannels.Add(newAudioChannel);
        return newAudioChannel.channelId;
    }

    public int PlayPingPong(AudioClip clip, AudioChannelSettings channelSettings, float periodInSeconds)
    {
        AudioChannel newAudioChannel = this.GetAudioChannel(clip, channelSettings);
        newAudioChannel.PlayPingPong(periodInSeconds);
        this.playingAudioChannels.Add(newAudioChannel);
        return newAudioChannel.channelId;
    }

    public int PlayPeriodically(AudioClip clip, AudioChannelSettings channelSettings, float periodInSeconds)
    {
        AudioChannel newAudioChannel = this.GetAudioChannel(clip, channelSettings);
        newAudioChannel.PlayPeriodically(periodInSeconds);
        this.playingAudioChannels.Add(newAudioChannel);
        return newAudioChannel.channelId;
    }

    public void Stop(int channelId)
    {
        if (this.playingAudioChannels.Exists(x => x.channelId == channelId))
        {
            this.playingAudioChannels.Find(x => x.channelId == channelId).Stop();
        }
    }

    public int GetNewChannelId()
    {
        this.channelIdCounter++;
        return channelIdCounter;
    }

    public void ReleaseChannel(AudioChannel releasedChannel)
    {
        if (this.playingAudioChannels.Exists(x => x == releasedChannel) == false)
        {
            return;
        }

        this.playingAudioChannels.Remove(releasedChannel);
        this.audioChannelCache.Push(releasedChannel);

        releasedChannel.transform.position = this.transform.position;
    }

    public int Crossfade(int fromChannelId, AudioClip clip, AudioChannelSettings channelSettings, float fadeDuration)
    {
        if (fromChannelId > 0 && this.playingAudioChannels.Exists(x => x.channelId == fromChannelId))
        {
            AudioChannel fromChannel = this.playingAudioChannels.Find(x => x.channelId == fromChannelId);            
            fromChannel.FadeOut(fadeDuration);            
        }

        AudioChannel toChannel = this.GetAudioChannel(clip, channelSettings);
        toChannel.FadeIn(fadeDuration);
        toChannel.Play();

        this.playingAudioChannels.Add(toChannel);

        return toChannel.channelId;
    }

    public void SetPitch(int channelId, float newPitch)
    {
        AudioChannel targetChannel = this.playingAudioChannels.Find(x => x.channelId == channelId);

        targetChannel.channelSettings.minPitch = newPitch;
        targetChannel.channelSettings.maxPitch = newPitch;
    }

    public void SetVolume(int channelId, float newVolume)
    {
        if (channelId > 0 && this.playingAudioChannels.Exists(x => x.channelId == channelId))
        {            
            AudioChannel targetChannel = this.playingAudioChannels.Find(x => x.channelId == channelId);

            if (targetChannel.crossFading == true)
            {
                return;
            }

            targetChannel.channelSettings.volume = newVolume;
        }        
    }

    public bool IsPlaying(int channelId)
    {
        return (channelId > 0 && this.playingAudioChannels.Exists(x => x.channelId == channelId));        
    }
}
