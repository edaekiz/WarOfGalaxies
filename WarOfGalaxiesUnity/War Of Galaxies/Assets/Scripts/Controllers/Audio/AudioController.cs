using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class AudioController : MonoBehaviour
{
    public static AudioController AC { get; set; }

    /// <summary>
    /// Ses aktif mi değil mi?
    /// </summary>
    public bool IsSoundActive = true;

    private AudioSource audioSource;

    /// <summary>
    /// Müziklerin listesi.
    /// </summary>
    public string[] Musics;

    /// <summary>
    /// Son çalınan müziği tutuyoruz.
    /// </summary>
    private string lastPlayedMusic;

    private void Awake()
    {
        if (AC == null)
            AC = this;
        else
            Destroy(gameObject);

    }

    private void Start()
    {
        // Ses değerini alıyoruz.
        AudioListener.volume = 1;//PlayerPrefs.GetFloat(ConfigManager.CM.VOLUME_KEY, 1);

        // Kamera da bu müziği oynatacağız.
        audioSource = GetComponent<AudioSource>();

        // Başlangıç müziğini çalıyourz.
        PlayMusic(GetARandomMusic());

    }

    public void ChangeAudioSource(GameObject source)
    {
        // Kamera da bu müziği oynatacağız.
        audioSource = source.GetComponent<AudioSource>();

        // Başlangıç müziğini çalıyourz.
        PlayMusic(GetARandomMusic());
    }

    /// <summary>
    /// Sesi kapatır yada açar.
    /// </summary>
    public void ToggleMute()
    {
        if (IsSoundActive)
            Mute();
        else
            UnMute();
    }

    /// <summary>
    /// Bütün sesleri kapatır.
    /// </summary>
    public void Mute()
    {
        // Bütün sesleri kapatıyoruz.
        FindObjectsOfType<AudioSource>().ToList().ForEach(e => e.mute = true);

        // Sesi kapalı olarak işaretliyoruz.
        IsSoundActive = false;
    }

    /// <summary>
    /// Bütün sesleri açar.
    /// </summary>
    public void UnMute()
    {
        // Bütün sesleri açıyoruz.
        FindObjectsOfType<AudioSource>().ToList().ForEach(e => e.mute = false);

        // Sesi açık olarak işaretliyoruz.
        IsSoundActive = true;
    }

    /// <summary>
    /// Verilen sesi çalar.
    /// </summary>
    /// <param name="go">Hangi birim üzerinde bu ses çalacak.</param>
    /// <param name="audioName">Çalınacak ses.</param>
    public void PlaySound(MonoBehaviour go, string audioName, float volume = 1f)
    {

        // Eğer ses dosyası boş ise geri dön.
        if (string.IsNullOrEmpty(audioName))
            return;


        // Sesi çalacak olan item.
        AudioSource audioSource = go.GetComponent<AudioSource>();

        // Yok ise geri dön.
        if (audioSource == null)
            return;

        // Klip ismi.
        AudioClip audio = Resources.Load<AudioClip>(audioName);

        // Eğer ses yüklendiyse çalıyoruz.
        audioSource.GetComponent<AudioSource>().PlayOneShot(audio, volume);
    }

    /// <summary>
    /// Verilen sesi kamera üzerinden çalar.
    /// </summary>
    /// <param name="audioName">Çalınacak ses.</param>
    public void PlaySoundOnCamera(string audioName, float volume = 1)
    {

        // Eğer ses dosyası boş ise geri dön.
        if (string.IsNullOrEmpty(audioName))
            return;

        AudioClip audio = Resources.Load<AudioClip>(audioName);

        // Eğer ses yüklendiyse çalıyoruz.
        audioSource.PlayOneShot(audio, volume);
    }

    /// <summary>
    /// Müziği kapatır.
    /// </summary>
    public void StopMusic()
    {
        audioSource.Stop();
    }

    /// <summary>
    /// Verilen müziği çalar.
    /// </summary>
    /// <param name="musicName">Müziğin adı.</param>
    /// <param name="isLooped">Tekrarlanacak mı?</param>
    public void PlayMusic(string musicName)
    {
        // Eğer ses dosyası boş ise geri dön.
        if (string.IsNullOrEmpty(musicName))
            return;

        // Sesi biraz kısıyoruz.
        audioSource.volume = 0.7f;

        AudioClip clip = Resources.Load<AudioClip>(musicName);
        
        // Klibi değiştiriyoruz.
        audioSource.clip = clip;

        // Klibi çalıyoruz.
        audioSource.Play();

        // Son çalınan müziği güncelliyoruz.
        lastPlayedMusic = musicName;
    }

    /// <summary>
    /// Verilen seslerden rastgele birisini çalar.
    /// </summary>
    /// <param name="go">Hangi birim üzerinde çalınacak.</param>
    /// <param name="audioNames">Hangi seslerden birisi çalınacak.</param>
    public void PlayRandomSound(MonoBehaviour go, float volume = 1, params string[] audioNames)
    {
        // Parametre yok ise geri dön.
        if (audioNames == null || audioNames.Length == 0)
            return;

        // Rastgele bir ses seçiyoruz.
        int audioIndex = Random.Range(0, audioNames.Length);

        // Sesi çalıyoruz.
        PlaySound(go, audioNames[audioIndex], volume);
    }

    private void Update()
    {
        // UI butonları çok fazla olduğu için bu şekilde bir kontrol yapıyoruz.
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.currentSelectedGameObject != null)
                PlaySoundOnCamera("GameJoystickButton6");

            // Eğer ses çalmıyor ise bir ses çal.
            if (!audioSource.isPlaying)
                PlayMusic(GetARandomMusic());
        }
    }

    /// <summary>
    /// Rastgele bir ses çalar.
    /// </summary>
    /// <returns></returns>
    public string GetARandomMusic()
    {
        // Eğer başka ses yok ise direk sesi dön.
        if (Musics.Length == 1)
            return Musics[0];
        else if (Musics.Length > 1)
        {
            // Rastgele bir ses alıyoruz.
            string musicname = Musics[Random.Range(0, Musics.Length)];

            // Eğer random olarak alınan ses listede var ise yeni bir tane bul.
            while (musicname == lastPlayedMusic)
                musicname = Musics[Random.Range(0, Musics.Length)];

            // Yeni sesi dön.
            return musicname;
        }

        // Eğer hiç ses yok ise geri dön.
        return string.Empty;

    }
}
