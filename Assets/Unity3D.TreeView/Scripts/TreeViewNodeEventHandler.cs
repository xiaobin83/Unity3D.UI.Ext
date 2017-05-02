using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;

namespace ui
{
	public class TreeViewNodeEventHandler : MonoBehaviour,
		IPointerClickHandler,
		IBeginDragHandler, IDragHandler, IEndDragHandler
	{
		public System.Action<PointerEventData> onBeginDrag;
		public System.Action<PointerEventData> onDrag;
		public System.Action<PointerEventData> onEndDrag;
		public System.Action<PointerEventData> onPointerClick;

		public void OnBeginDrag(PointerEventData eventData)
		{
			if (onBeginDrag != null)
				onBeginDrag(eventData);
		}

		public void OnDrag(PointerEventData eventData)
		{
			if (onDrag != null)
				onDrag(eventData);
		}

		public void OnEndDrag(PointerEventData eventData)
		{
			if (onEndDrag != null)
				onEndDrag(eventData);
		}

		public void OnPointerClick(PointerEventData eventData)
		{
			if (onPointerClick != null)
				onPointerClick(eventData);
		}
	}
}