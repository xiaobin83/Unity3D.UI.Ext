using UnityEngine;
using System.Collections;

public class Test : MonoBehaviour {

	public TestTreeView treeView;

	void Awake()
	{
		var n = treeView.Add("test01", new TestItem() { text = "Test 01" });
		treeView.Add("test02", new TestItem() { text = "Test 02" }, n);
		treeView.Add("test03", new TestItem() { text = "Test 03" }, n);
		var n04 = treeView.Add("test04", new TestItem() { text = "Test 04" }, n);
		treeView.Add("test05", new TestItem() { text = "Test 05" }, n04);
		treeView.Add("test05", new TestItem() { text = "Test 06" }, n04);
	}


}
