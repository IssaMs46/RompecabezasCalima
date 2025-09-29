using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PuzzleManager : MonoBehaviour
{
    [Header("Config")]
    public int totalPiezas;                       // cuántas piezas tiene el puzzle
    public RectTransform puzzleContainer;         // padre que va a saltar
    public List<ParticleSystem> confettis;        // lista de sistemas de partículas

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
        // --- Lanza todos los sistemas de partículas de la lista ---
        foreach (var ps in confettis)
        {
            if (ps) ps.Play();
        }

        Vector2 startPos = puzzleContainer.anchoredPosition;
        float jumpHeight = 40f;     // altura base de cada salto en píxeles
        int totalRebotes = 3;       // 👈 cantidad de rebotes

        for (int i = 0; i < totalRebotes; i++)
        {
            Vector2 upPos = startPos + Vector2.up * jumpHeight;
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
            // Bajar en 0.25 s con un pequeño “bounce” senoidal
            while (t < 0.25f)
            {
                float p = t / 0.25f;
                float bounce = Mathf.Sin(p * Mathf.PI); // curva de rebote
                puzzleContainer.anchoredPosition =
                    Vector2.Lerp(upPos, startPos, p) + Vector2.up * bounce * 10f;
                t += Time.deltaTime;
                yield return null;
            }

            // Vuelve a la posición inicial para el siguiente rebote
            puzzleContainer.anchoredPosition = startPos;

            // Opcional: reducir ligeramente la altura de cada rebote para efecto de amortiguación
            jumpHeight *= 0.7f;
        }

        // Asegura que al final quede en su posición original exacta
        puzzleContainer.anchoredPosition = startPos;
    }
}
