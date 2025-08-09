using UnityEngine;

public class SFXManager : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip coinSFX;
    [SerializeField] AudioClip jumpSFX;
    [SerializeField] AudioClip doubleJumpSFX;
    [SerializeField] AudioClip doubleJumpGetSFX;
    [SerializeField] AudioClip landSFX;
    [SerializeField] AudioClip damagedSFX;
    [SerializeField] AudioClip shieldBreakSFX;
    [SerializeField] AudioClip gameOverSFX;
    [SerializeField] AudioClip[] kuhoProjectileSFX;

    public void PlaySFX(string clipToPlay,int clipNum)
    {
        switch (clipToPlay)
        {
            case "Coin":
                audioSource.clip = coinSFX;
                break;
            case "kuhoProjectileSFX":
                audioSource.clip = kuhoProjectileSFX[clipNum];
                break;
            case "Jump":
                audioSource.clip = jumpSFX;
                break;
            case "DoubleJump":
                audioSource.clip = doubleJumpSFX;
                break;
            case "DoubleJumpGet":
                audioSource.clip = doubleJumpGetSFX;
                break;
            case "Land":
                audioSource.clip = landSFX;
                break;
            case "KuhoDamage":
                audioSource.clip = damagedSFX;
                break;
            case "ShieldBreak":
                audioSource.clip = shieldBreakSFX;
                break;
            case "GameOver":
                audioSource.clip = gameOverSFX;
                break;
        }
        audioSource.Play();
    }
}
