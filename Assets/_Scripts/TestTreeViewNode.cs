using UnityEngine;
using UnityEngine.UI;

public class TestTreeViewNode : MonoBehaviour
{
	public Image icon;
	public Text text;
	public Image highlight;

	TestItem item;

	HorizontalLayoutGroup layout;

	void Awake()
	{
		layout = GetComponent<HorizontalLayoutGroup>();
	}

	void OnItemCreated(ui.TreeView<TestItem>.Node node)
	{
		item = node.item;
		text.text = item.text;
		icon.sprite = item.icon;
		layout.padding.left = node.indent;
	}

	void OnItemSelected()
	{
		highlight.enabled = true;
	}

	void OnItemDeselected()
	{
		highlight.enabled = false;
	}

}

