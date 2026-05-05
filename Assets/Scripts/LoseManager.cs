using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class LoseManager : MonoBehaviour
{
    [SerializeField] private UniversalRendererData rendererData;

    private void Start()
    {
        if (rendererData != null && rendererData.rendererFeatures.Count > 0)
        {
            rendererData.rendererFeatures[0].SetActive(false);
        }
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(0);
    }
}
