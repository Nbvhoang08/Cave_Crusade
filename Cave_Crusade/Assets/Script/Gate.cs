using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;
namespace Script
{
    public class Gate : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                StartCoroutine(LoadNextScene());
            }
        }

        private IEnumerator LoadNextScene()
        {
            // Thêm hiệu ứng chờ đợi hoặc hiệu ứng chuyển cảnh tại đây nếu cần
            yield return new WaitForSeconds(1f); // Chờ 1 giây trước khi chuyển cảnh

            // Chuyển sang cảnh tiếp theo
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}