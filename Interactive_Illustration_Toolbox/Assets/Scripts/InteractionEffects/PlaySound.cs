using System.Security.Cryptography;
using UnityEngine;

namespace InteractionEffects
{
    public class PlaySound : InteractionEffect
    {
        [SerializeField] private AudioClip soundToPlay;
        [SerializeField] private float volume = 1f;
        [SerializeField] private float randomPitchMin = 1f;
        [SerializeField] private float randomPitchMax = 1f;
        
        public override bool CheckSetup()
        {
            if (soundToPlay == null)
            {
                Debug.LogError("AudioClip not set on InteractionEffect! Please set it.");
            }

            return soundToPlay != null;
        }

        public override void Trigger()
        {
            GameObject holder = new GameObject();
            holder.name = "Sound " + soundToPlay.name;
            AudioSource source = holder.AddComponent<AudioSource>();
            source.clip = soundToPlay;
            source.volume = volume;
            source.pitch = Random.Range(randomPitchMin, randomPitchMax);
            source.Play();
            Destroy(holder, soundToPlay.length);
        }
    }
}
