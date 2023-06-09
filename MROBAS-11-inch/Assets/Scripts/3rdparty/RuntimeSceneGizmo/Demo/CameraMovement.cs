﻿using UnityEngine;

namespace RuntimeSceneGizmo
{
	public class CameraMovement : MonoBehaviour
	{
#pragma warning disable 0649
		[SerializeField]
		private float sensitivity = 0.5f;
#pragma warning restore 0649

		private Vector3 prevMousePos;
		private Transform mainCamParent;

		private void Awake()
		{
			mainCamParent = Camera.main.transform.parent;
		}

		public bool gizmoSelected = false;

		private void Update()
		{
			/*if( Input.GetMouseButtonDown( 1 ) )
				prevMousePos = Input.mousePosition;
			else if( Input.GetMouseButton( 1 ) )
			{
				Vector3 mousePos = Input.mousePosition;
				Vector2 deltaPos = ( mousePos - prevMousePos ) * sensitivity;

				Vector3 rot = mainCamParent.localEulerAngles;
				while( rot.x > 180f )
					rot.x -= 360f;
				while( rot.x < -180f )
					rot.x += 360f;

				rot.x = Mathf.Clamp( rot.x - deltaPos.y, -89.8f, 89.8f );
				rot.y += deltaPos.x;
				rot.z = 0f;

				mainCamParent.localEulerAngles = rot;
				prevMousePos = mousePos;
			}*/

			if (Input.touchCount == 1)
			{
				Touch touch = Input.GetTouch(0);

				Ray ray = Camera.main.ScreenPointToRay(touch.position);
				RaycastHit hit;
				if (!gizmoSelected)
				{
					//Vector2 touchPosition = touch.position;

					if (touch.phase == TouchPhase.Began)
					{
						prevMousePos = touch.position;// Input.mousePosition;
					}
					else if (touch.phase == TouchPhase.Moved)
					{
						Vector3 mousePos = Input.mousePosition;
						Vector2 deltaPos = (mousePos - prevMousePos) * sensitivity;

						Vector3 rot = mainCamParent.localEulerAngles;
						while (rot.x > 180f)
							rot.x -= 360f;
						while (rot.x < -180f)
							rot.x += 360f;

						rot.x = Mathf.Clamp(rot.x - deltaPos.y, -89.8f, 89.8f);
						rot.y += deltaPos.x;
						rot.z = 0f;

						mainCamParent.localEulerAngles = rot;
						prevMousePos = mousePos;
					}
				}
	
			}
		}
	}
}