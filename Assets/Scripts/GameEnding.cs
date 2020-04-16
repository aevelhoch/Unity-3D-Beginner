using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameEnding : MonoBehaviour
{
    public float fadeDuration = 1f;
    public float displayImageDuration = 1f;
    public GameObject player;
    public CanvasGroup exitBackgroundImageCanvasGroup;
    public AudioSource exitAudio;
    public CanvasGroup caughtBackgroundImageCanvasGroup;
    public AudioSource caughtAudio;
    bool m_IsPlayerAtExit;
    bool m_IsPlayerCaught;
    float m_Timer;
    bool m_HasAudioPlayed;

    // new variables added by me for the progressbar/particles
    public GameObject progressBar;
    float m_startDist;
    RectTransform m_progressBarTransform;
    ParticleSystem m_finishLineParticles;

    // added the code in Start, for the progressbar
    private void Start()
    {
        // get starting dist from exit to use as max for interpolation
        m_startDist = getDistToExit();
        // get progress bar component and set it to default
        m_progressBarTransform = progressBar.GetComponent<RectTransform>();
        m_progressBarTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 30);
        // get particle system for finishline
        m_finishLineParticles = GetComponent<ParticleSystem>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player)
        {
            m_IsPlayerAtExit = true;
        }
    }

    public void CaughtPlayer()
    {
        m_IsPlayerCaught = true;
    }

    private void Update()
    {
        if (m_IsPlayerAtExit)
        {
            EndLevel(exitBackgroundImageCanvasGroup, false, exitAudio);
        }
        else if (m_IsPlayerCaught)
        {
            EndLevel(caughtBackgroundImageCanvasGroup, true, caughtAudio);
        }
    }

    // added all the code here, for the progressbar
    private void FixedUpdate()
    {
        // get the current dist to exit
        float currentDist = getDistToExit();
        // set interpolation constraints
        float maxWidth = (Screen.width * 2);
        float minWidth = 30f;
        // default the width we'll be setting to the bar to be minimum, if farther than current distance
        float interpWidth = minWidth;
        // otherwise perform some linear interp
        if (currentDist < m_startDist)
        {
            // currentDist/startDist is percent of dist remaining, so 1-that is percent to covered
            float alphaDist = 1 - (currentDist / m_startDist);
            // do the interp
            interpWidth = (1.0f - alphaDist) * minWidth + alphaDist * maxWidth;
        }
        // then set the bar to the interpolated % covered
        m_progressBarTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, interpWidth);
        // then, if we're getting to the home stretch, start the particles in the game ending zone
        if (currentDist < m_startDist / 4)
        {
            m_finishLineParticles.Play();
        }
    }

    float getDistToExit()
    {
        Vector3 distToExit = player.transform.position - transform.position;
        return distToExit.magnitude;
    }

    void EndLevel(CanvasGroup imageCanvasGroup, bool doRestart, AudioSource audioSource)
    {
        if (!m_HasAudioPlayed)
        {
            audioSource.Play();
            m_HasAudioPlayed = true;
        }
        m_Timer += Time.deltaTime;
        imageCanvasGroup.alpha = m_Timer / fadeDuration;
        if(m_Timer > fadeDuration + displayImageDuration)
        {
            if (doRestart)
            {
                SceneManager.LoadScene(0);
            } else { 
                Application.Quit();
            }
        }
    }
}
