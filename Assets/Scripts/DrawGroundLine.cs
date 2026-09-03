using UnityEngine;

public class DrawGroundLine : MonoBehaviour
{
    // Altura da linha em relação ao chão
    public float lineHeight = 0.1f;

    // Cor da linha
    public Color lineColor = Color.red;

    // Tamanho da linha
    public float lineWidth = 2.0f;

    // Distância máxima para desenhar a linha
    public float maxDistance = 10.0f;

    private void OnDrawGizmos()
    {
        // Define a cor do gizmo
        Gizmos.color = lineColor;

        // Raycast para baixo a partir da posição do personagem
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, maxDistance))
        {
            // Desenha a linha do ponto de colisão para baixo
            Vector3 startPoint = transform.position;
            Vector3 endPoint = hit.point + Vector3.up * lineHeight;

            // Desenha a linha
            Gizmos.DrawLine(startPoint, endPoint);

            // Desenha uma esfera no ponto de colisão
            Gizmos.DrawSphere(hit.point, 0.1f);
        }
        else
        {
            // Se não houver colisão, desenha uma linha até a altura máxima definida
            Vector3 startPoint = transform.position;
            Vector3 endPoint = transform.position - Vector3.up * lineHeight;
            Gizmos.DrawLine(startPoint, endPoint);
        }
    }
}