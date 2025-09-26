using UnityEngine;
using System.Collections;

public class PuzzleManager : MonoBehaviour
{
    [Header("Config")]
    public int totalPiezas;                 // cuántas piezas tiene el puzzle
    public RectTransform puzzleContainer;   // padre que va a saltar
    public ParticleSystem confetti;         // arrástralo desde el inspector

    private int piezasColocadas = 0;

    // Llamar desde cada pieza cuando se bloquee
    public void PiezaCompletada()
    {
        piezasColocadas++;
        if (piezasColocadas >= totalPiezas)
        {
            StartCoroutine(JumpWithParticles());
        }
    }

    IEnumerator JumpWithParticles()
    {
        // --- Lanza las partículas ---
        if (confetti) confetti.Play();

        Vector2 startPos = puzzleContainer.anchoredPosition;
        Vector2 upPos    = startPos + Vector2.up * 40f; // 40 px de salto
        float t = 0f;

        // Subir en 0.15 s
        while (t < 0.15f)
        {
            puzzleContainer.anchoredPosition =
                Vector2.Lerp(startPos, upPos, t / 0.15f);
            t += Time.deltaTime;
            yield return null;
        }

        t = 0f;
        // Bajar en 0.25 s con rebote (usamos Mathf.Sin para simular un “bounce”)
        while (t < 0.25f)
        {
            float p = t / 0.25f;
            float bounce = Mathf.Sin(p * Mathf.PI); // curva de rebote
            puzzleContainer.anchoredPosition =
                Vector2.Lerp(upPos, startPos, p) + Vector2.up * bounce * 10f;
            t += Time.deltaTime;
            yield return null;
        }

        puzzleContainer.anchoredPosition = startPos;
    }
}