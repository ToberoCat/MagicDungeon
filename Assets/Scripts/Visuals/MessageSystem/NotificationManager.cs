using UnityEngine;

namespace Visuals.MessageSystem
{
    public class NotificationManager : MonoBehaviour
    {
        public static NotificationManager Instance { get; private set; }
        [SerializeField] private GameObject notificationPrefab;
        [SerializeField] private Transform player;

        private void Awake()
        {
            Instance = this;
        }

        public void ShowPlayerNotification(string message)
        {
            ShowNotification(player.position, message, Color.white);
        }

        public void ShowNotification(Vector3 origin, string message, Color color)
        {
            var floatingMessage = Instantiate(notificationPrefab, origin, Quaternion.identity)
                .GetComponent<FloatingMessage>();
            floatingMessage.SetText(message);
            floatingMessage.SetColor(color);
        }
    }
}