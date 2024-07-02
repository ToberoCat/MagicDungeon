using UnityEngine;

namespace Dungeon
{
    public class DungeonExit : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player"))
                return;

            foreach (var obj in GameObject.FindGameObjectsWithTag("Enemy"))
                Destroy(obj);
            foreach (var obj in GameObject.FindGameObjectsWithTag("Casts"))
                Destroy(obj);
            foreach (var obj in GameObject.FindGameObjectsWithTag("Dungeon"))
                Destroy(obj);

            DungeonGenerator.Instance.GenerateDungeon();
            other.transform.position = Vector3.zero;
        }
    }
}