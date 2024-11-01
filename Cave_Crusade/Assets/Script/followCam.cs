using JetBrains.Annotations;
using UnityEngine;

namespace Script
{
    public class followCam : Singleton<followCam>
    {
        public Transform target; // Đối tượng mà camera sẽ theo dõi
        public float smoothSpeed = 0.125f; // Tốc độ mượt mà của camera
        public Vector3 offset; // Khoảng cách giữa camera và đối tượng
        protected override void Awake()
        {
            if(target == null)
            {
                target = GameObject.FindWithTag("Player").transform;
            }
        }
       
        void LateUpdate()
        {
            if (target == null)
            {
                // Tìm lại đối tượng Player nếu target bị mất
                target = GameObject.FindWithTag("Player").transform;
            }
            Vector3 desiredPosition = target.position + offset; // Vị trí mong muốn của camera
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed); // Vị trí mượt mà
            transform.position = smoothedPosition; // Cập nhật vị trí của camera
        }
    }
}