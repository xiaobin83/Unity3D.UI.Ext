using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;


namespace ui
{
	public class TreeView<T> : MonoBehaviour
	{
		[SerializeField]
		RectTransform m_Content;

		[SerializeField]
		int m_Indent = 5;

		[SerializeField]
		GameObject m_PrefabEntry;

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

			public T item;

			public Node()
			{
				id = staticEntryId;
				++staticEntryId;
			}
		}

		RectTransform content
		{
			get
			{
				if (m_Content == null)
				{
					return GetComponent<RectTransform>();
				}
				return m_Content;
			}
		}

		Node m_Root;
		Node root
		{
			get
			{
				if (m_Root == null)
				{
					m_Root = new Node()
					{
						parent = null,
						gameObject = content.gameObject,
						children = new List<int>(),
						indent = -m_Indent, // tricky
					};
				}
				return m_Root;
			}
		}
		Dictionary<int, Node> allNodes = new Dictionary<int, Node>();

		public Node selected { get; private set; }


		public Node Add(string name, T item, Node parent = null)
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
			};
			allNodes.Add(n.id, n);

			var evtHandler = entryGameObject.AddComponent<TreeViewNodeEventHandler>();
			evtHandler.onBeginDrag = (evt) => OnBeginDrag(evt, n);
			evtHandler.onDrag = (evt) => OnDrag(evt, n);
			evtHandler.onEndDrag = (evt) => OnEndDrag(evt, n);
			evtHandler.onPointerClick = (evt) => OnPointerClick(evt, n);

			parent.children.Add(n.id);
			entryGameObject.transform.SetParent(root.gameObject.transform, false);
			entryGameObject.SendMessage("OnNodeCreated", n, SendMessageOptions.DontRequireReceiver);


			return n;
		}

		void OnBeginDrag(PointerEventData evtData, Node target)
		{

		}

		void OnDrag(PointerEventData evtData, Node target)
		{

		}

		void OnEndDrag(PointerEventData evtData, Node target)
		{

		}

		protected virtual void OnPointerClick(PointerEventData evtData, Node target)
		{
			if (target == selected)
				CollapseOrExpand(target);
			else
				Select(target);
		}

		public void Select(Node target)
		{
			if (selected != null)
			{
				selected.gameObject.SendMessage("OnNodeDeselected", SendMessageOptions.DontRequireReceiver);
			}
			selected = target;
			if (selected != null)
			{
				selected.gameObject.SendMessage("OnNodeSelected", SendMessageOptions.DontRequireReceiver);
			}
		}

		List<Node> temp = new List<Node>();

		public void CollapseOrExpand(Node target)
		{
			if (temp.Count > 0) return; // doing expanding or collapsing

			if (target.collapsed)
			{
				Expand(target);
			}
			else
			{
				Collapse(target);
			}
		}




		void CollectExpand(Node target)
		{
			if (target != selected)
				temp.Add(target);
			target.collapsed = false;

			if (target.children != null && target.children.Count > 0)
			{
				for (int i = 0; i < target.children.Count; ++i)
				{
					var id = target.children[i];
					var n = allNodes[id];
					CollectExpand(n);
				}

			}
		}

		IEnumerator ExpandOrCollapseAll(bool expandOrCollapse)
		{
			for (int i = 0; i < temp.Count; ++i)
			{
				var target = temp[i];
				target.collapsed = expandOrCollapse ? false : true;
				target.gameObject.SetActive(expandOrCollapse);
				yield return null;
			}
			temp.Clear();

		}

		public void Expand(Node target)
		{
			temp.Clear();
			CollectExpand(target);
			StartCoroutine(ExpandOrCollapseAll(true));
		}

		void CollectCollapse(Node target)
		{
			if (target.children != null && target.children.Count > 0)
			{
				for (int i = 0; i < target.children.Count; ++i)
				{
					var id = target.children[i];
					var n = allNodes[id];
					CollectCollapse(n);
				}
			}

			if (target != selected)
				temp.Add(target);
			target.collapsed = true;
		}

		public void Collapse(Node target)
		{
			temp.Clear();
			CollectCollapse(target);
			StartCoroutine(ExpandOrCollapseAll(false));
		}


	}
}
