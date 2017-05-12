using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ui
{

	public class NodeEventHandler : MonoBehaviour,
		IPointerClickHandler,
		IBeginDragHandler, IDragHandler, IEndDragHandler,
		IPointerEnterHandler, IPointerExitHandler
	{
		public System.Action<PointerEventData> onBeginDrag;
		public System.Action<PointerEventData> onDrag;
		public System.Action<PointerEventData> onEndDrag;
		public System.Action<PointerEventData> onPointerClick;
		public System.Action<PointerEventData> onPointerEnter;
		public System.Action<PointerEventData> onPointerExit;

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

		public void OnPointerEnter(PointerEventData eventData)
		{
			if (onPointerEnter != null)
				onPointerEnter(eventData);
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			if (onPointerExit != null)
				onPointerExit(eventData);
		}
	}
}