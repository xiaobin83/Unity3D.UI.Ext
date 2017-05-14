using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;


namespace ui
{
	public class TreeView<T> : UIBehaviour
	{
		[SerializeField]
		RectTransform m_Content;

		[SerializeField]
		int m_Indent = 5;

		[SerializeField]
		GameObject m_PrefabEntry;

		[SerializeField]
		bool m_EnableDragAndDrop = false;

		[SerializeField]
		Transform m_DraggingCanvas;



		Transform cachedCanvasInParent_;
		Transform draggingCanvas
		{
			get
			{
				if (m_DraggingCanvas == null)
				{
					if (cachedCanvasInParent_ == null)
						cachedCanvasInParent_ = FindCanvasInParent(content);
					return cachedCanvasInParent_;
				}
				else
				{
					return m_DraggingCanvas;
				}
			}
		}

		public class Node
		{
			static int staticEntryId = 0;
			public int id {get; private set; }
			public Node parent;
			public GameObject gameObject;
			public List<int> children;
			public string name;
			public int indent;
			public bool collapsed = false;
			public CanvasGroup canvasGroup;

			public T item;

			public bool hasChild
			{
				get
				{
					return children != null && children.Count > 0;
				}
			}

			public Node()
			{
				id = staticEntryId;
				++staticEntryId;
			}
		}

		RectTransform cachedContent_;
		RectTransform content
		{
			get
			{
				if (m_Content == null)
				{
					if (cachedContent_ == null)
						cachedContent_ = GetComponent<RectTransform>();
					return cachedContent_;
				}
				return m_Content;
			}
		}

		Node root_;
		Node root
		{
			get
			{
				if (root_ == null)
				{
					root_ = new Node()
					{
						parent = null,
						gameObject = content.gameObject,
						children = new List<int>(),
						indent = -m_Indent, // tricky
					};
				}
				return root_;
			}
		}
		Dictionary<int, Node> allNodes = new Dictionary<int, Node>();

		public Node selected { get; private set; }
				
		public virtual Node Add(string name, T item, Node parent = null)
		{
			if (parent == null) parent = root;
			if (parent.children == null)
			{
				parent.children = new List<int>();
			}
			var entryGameObject = GameObject.Instantiate(m_PrefabEntry);

			var n = new Node()
			{
				parent = parent,
				gameObject = entryGameObject,
				children = null,
				name = name,
				indent = parent.indent + m_Indent,
				item = item,
				canvasGroup = entryGameObject.AddComponent<CanvasGroup>()
			};
			allNodes.Add(n.id, n);

			var evtHandler = entryGameObject.AddComponent<NodeEventHandler>();
			evtHandler.onBeginDrag = (evt) => OnBeginDrag(evt, n);
			evtHandler.onDrag = (evt) => OnDrag(evt, n);
			evtHandler.onEndDrag = (evt) => OnEndDrag(evt, n);
			evtHandler.onPointerClick = (evt) => OnPointerClick(evt, n);
			evtHandler.onPointerEnter = (evt) => OnPointerEnter(evt, n);
			evtHandler.onPointerExit = (evt) => OnPointerExit(evt, n);



			parent.children.Add(n.id);
			entryGameObject.transform.SetParent(root.gameObject.transform, false);
			entryGameObject.SendMessage("OnItemCreated", n, SendMessageOptions.DontRequireReceiver);
			return n;
		}

		public void RemoveSelected()
		{
			var n = selected;
			if (n != null)
			{
				Select(null);
				Remove(n);
			}
		}

		public virtual void Remove(Node target)
		{
			if (temp.Count > 0) return;

			CollectAllChildren(target);
			temp.Add(target);
			StartCoroutine(Remove());
		}

		IEnumerator Remove()
		{
			for (int i = 0; i < temp.Count; ++i)
			{
				var target = temp[i];
				allNodes.Remove(target.id);
				Destroy(target.gameObject);
				yield return null;
			}
			temp.Clear();
		}

		Transform FindCanvasInParent(Transform trans)
		{
			var comps = Pool<List<Component>>.instance.Alloc();
			while (trans != null)
			{
				trans.GetComponents(typeof(Canvas), comps);
				for (int i = 0; i < comps.Count; ++i)
				{
					var c = (Canvas)comps[i];
					if (c.isRootCanvas)
					{
						comps.Clear();
						Pool<List<Component>>.instance.Free(comps);
						return c.transform;
					}
				}
				trans = trans.transform.parent;
				comps.Clear();
			}
			comps.Clear();
			Pool<List<Component>>.instance.Free(comps);
			return trans;
		}

		Node dragging_;
		protected virtual void OnBeginDrag(PointerEventData evtData, Node target)
		{
			if (!m_EnableDragAndDrop) return;

			if (dragging_ != null)
			{
				OnEndDrag(evtData, dragging_);
			}
			dragging_ = target;
			// collapse	it and attach to dragging canvas
			Collapse(dragging_);
			dragging_.gameObject.transform.SetParent(draggingCanvas);
			dragging_.canvasGroup.blocksRaycasts = false;
		}

		protected virtual void OnDrag(PointerEventData evtData, Node target)
		{
			if (!m_EnableDragAndDrop) return;

			if (dragging_ == target)
			{
				dragging_.gameObject.transform.position = evtData.position;
			}
		}

		protected virtual void OnEndDrag(PointerEventData evtData, Node target)
		{
			if (!m_EnableDragAndDrop) return;
			if (dragging_ != null)
			{
				if (hitting_ != null)
				{
					// TODO:
				}
				dragging_.canvasGroup.blocksRaycasts = true;
				dragging_ = null;
			}
		}

		protected virtual void OnPointerClick(PointerEventData evtData, Node target)
		{
			if (target == selected)
				CollapseOrExpand(target);
			else
				Select(target);
		}

		Node hitting_;
		protected virtual void OnPointerEnter(PointerEventData evtData, Node target)
		{
			hitting_ = target;
			Debug.Log(target.name);
		}

		protected virtual void OnPointerExit(PointerEventData evtData, Node target)
		{
			if (hitting_ == target)
			{
				hitting_ = null;
			}
		}


		public void Select(Node target)
		{
			if (selected != null)
			{
				selected.gameObject.SendMessage("OnItemDeselected", SendMessageOptions.DontRequireReceiver);
			}
			selected = target;
			if (selected != null)
			{
				selected.gameObject.SendMessage("OnItemSelected", SendMessageOptions.DontRequireReceiver);
			}
		}

		List<Node> temp = new List<Node>();
		public void CollapseOrExpand(Node target)
		{
			if (temp.Count > 0) return; // doing expanding or collapsing
			if (target.hasChild)
			{
				if (target.collapsed)
				{
					Expand(target);
					target.gameObject.SendMessage("OnItemExpanded", SendMessageOptions.DontRequireReceiver);
				}
				else
				{
					Collapse(target);
					target.gameObject.SendMessage("OnItemCollapsed", SendMessageOptions.DontRequireReceiver);
				}
			}
		}

		void CollectExpand(Node target)
		{
			if (target.children != null && target.children.Count > 0)
			{
				for (int i = 0; i < target.children.Count; ++i)
				{
					var id = target.children[i];
					var n = allNodes[id];
					temp.Add(n);
					if (!n.collapsed)
					{
						CollectExpand(n);
					}
				}
			}

		}

		IEnumerator ExpandOrCollapseAll(bool expandOrCollapse)
		{
			for (int i = 0; i < temp.Count; ++i)
			{
				var target = temp[i];
				target.gameObject.SetActive(expandOrCollapse);
				yield return null;
			}
			temp.Clear();

		}

		public void Expand(Node target)
		{
			temp.Clear();
			target.collapsed = false;
			CollectExpand(target);
			StartCoroutine(ExpandOrCollapseAll(true));
		}

		void CollectAllChildren(Node target)
		{
			if (target.children != null && target.children.Count > 0)
			{
				for (int i = 0; i < target.children.Count; ++i)
				{
					var id = target.children[i];
					var n = allNodes[id];
					CollectAllChildren(n);
					temp.Add(n);
				}
			}
		}

		public void Collapse(Node target)
		{
			temp.Clear();
			target.collapsed = true;
			CollectAllChildren(target);
			StartCoroutine(ExpandOrCollapseAll(false));
		}


	}
}
