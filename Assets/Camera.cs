using UnityEngine;

public class Camera : MonoBehaviour
{
    public Transform target; // O objeto que a câmera vai seguir
    public Vector3 offset;   // A posição de deslocamento da câmera em relação ao alvo
    public float smoothSpeed = 0.125f; // Velocidade de suavização do movimento

    private void LateUpdate()
    {
        if (target != null)
        {
            // Calcula a posição desejada
            Vector3 desiredPosition = target.position + offset;

            // Suaviza o movimento da câmera
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

            // Aplica a posição suavizada
            transform.position = new Vector3(target.position.x, target.position.y, -10);
        }
    }
}
