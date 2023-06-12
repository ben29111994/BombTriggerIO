using UnityEngine;

public class SoundManager : MonoBehaviour {

	public static SoundManager instance;

	public AudioSource audioSource;
    public AudioClip win, lose, button, explode, ticking, bomb, doubleKill, tripleKill, multiKill;
    //public AudioClip[] hit;
    //public AudioClip[] push;

    void Start()
    {
        //if (instance)
        //{
        //    DestroyImmediate(gameObject);
        //}
        //else
        //{
        instance = this;
        //    DontDestroyOnLoad(gameObject);
        //}
        try
        {
            audioSource = GetComponent<AudioSource>();
        }
        catch { }
    }

    //public void PlayRandomHit()
    //{
    //    var random = Random.Range(0, hit.Length - 1);
    //    audioSource.PlayOneShot(hit[random]);
    //}

    //public void PlayRandomPush()
    //{
    //    var random = Random.Range(0, push.Length - 1);
    //    audioSource.PlayOneShot(push[random]);
    //}

    public void PlaySound(AudioClip clip){
        audioSource.PlayOneShot(clip);
	}
}
