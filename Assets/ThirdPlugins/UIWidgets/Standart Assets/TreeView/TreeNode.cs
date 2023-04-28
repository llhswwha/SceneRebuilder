using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System;
using System.ComponentModel;
using Location.WCFServiceReferences.LocationServices;

namespace UIWidgets {
	/// <summary>
	/// Tree node.
	/// </summary>
	[Serializable]
	public class TreeNode<TItem> 
		: IDisposable
		, IObservable
	, INotifyPropertyChanged
	{
		public int Level = 0;

		/// <summary>
		/// Occurs when on change.
		/// </summary>
		public event OnChange OnChange;

		/// <summary>
		/// Occurs when a property value changes.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// The pause observation.
		/// </summary>
		public bool PauseObservation;

		[SerializeField]
		bool isVisible = true;

		/// <summary>
		/// Gets or sets a value indicating whether this instance is visible.
		/// </summary>
		/// <value><c>true</c> if this instance is visible; otherwise, <c>false</c>.</value>
		public bool IsVisible {
			get {
				return isVisible;
			}
			set {
				isVisible = value;
				Changed("IsVisible");
			}
		}

		public void SetIsVisible(bool v)
        {
			isVisible = v;

			//if (node.Parent != null)
			//{
			//	Debug.LogError($"AutoSetNodeVisible node:{Parent.Item.Name}->{Item.Name} showCount:{showCount} isVisible:{node.IsVisible}");
			//}
			//else
			//{
			//	Debug.LogError($"AutoSetNodeVisible node:NULL->{node.Item.Name} showCount:{showCount} isVisible:{node.IsVisible}");
			//}
		}

		public int VisibleCount = -1;

		public int GetVisibleCount()
		{
            //if (VisibleCount == -1)
            {
				VisibleCount = 0;
				if(Nodes!=null)
					foreach(var node in Nodes) 
					{
						if(node.IsVisible)
						{
							VisibleCount++;
						}
					}
            }
			return VisibleCount;
		}

		//public void SetParentVisible()
  //      {
		//	SetParentVisible(Parent);
		//}

		//public void SetParentVisible(TreeNode<TItem> parent)
		//{
		//	if (parent == null) return;
		//	int showCount = parent.GetVisibleCount();
		//	if (showCount > 0)
		//	{
		//		parent.IsVisible = true;
		//	}
		//	else
		//	{
		//		parent.IsVisible = false;
		//	}
		//	SetParentVisible(parent.Parent);
		//}

		public List<TreeNode<TItem>> GetVisibleNodes()
		{
			List<TreeNode<TItem>> visibleNodes = new List<TreeNode<TItem>>();
				foreach (var node in Nodes)
				{
					if (node.IsVisible)
					{
					visibleNodes.Add(node);
					}
				}
			return visibleNodes;
		}

		[SerializeField]
		bool isExpanded;

		/// <summary>
		/// Gets or sets a value indicating whether this instance is expanded.
		/// </summary>
		/// <value><c>true</c> if this instance is expanded; otherwise, <c>false</c>.</value>
		public bool IsExpanded {
			get {
				return isExpanded;
			}
			set {
				isExpanded = value;
				Changed("IsExpanded");
			}
		}

		public void SetIsExpanded(bool v)
        {
			isExpanded = v;
		}
		
		[SerializeField]
		TItem item;

		/// <summary>
		/// Gets or sets the item.
		/// </summary>
		/// <value>The item.</value>
		public TItem Item {
			get {
				return item;
			}
			set {
				item = value;
				Changed("Item");
			}
		}
		
		[SerializeField]
		IObservableList<TreeNode<TItem>> nodes;

		/// <summary>
		/// Gets or sets the nodes.
		/// </summary>
		/// <value>The nodes.</value>
		public IObservableList<TreeNode<TItem>> Nodes {
			get {
				return nodes;
			}
			set {
				if (nodes!=null)
				{
					nodes.OnChange -= Changed;
					nodes.OnCollectionChange -= CollectionChanged;
				}
				nodes = value;
				if (nodes!=null)
				{
					nodes.OnChange += Changed;
					nodes.OnCollectionChange += CollectionChanged;
					CollectionChanged();
				}
				Changed("Nodes");
			}
		}

		/// <summary>
		/// Gets the total nodes count.
		/// </summary>
		/// <value>The total nodes count.</value>
		public int TotalNodesCount {
			get {
				if (nodes==null)
				{
					return 1;
				}
				return nodes.Sum(x => x.TotalNodesCount) + 1;
			}
		}

		public int NodesCount
		{
			get
			{
				if (nodes == null)
				{
					return 0;
				}
				return nodes.Count;
			}
		}

		/// <summary>
		/// The used nodes count.
		/// </summary>
		public int UsedNodesCount;

		/// <summary>
		/// Gets all used nodes count.
		/// </summary>
		/// <value>All used nodes count.</value>
		public int AllUsedNodesCount {
			get {
				if (!isVisible)
				{
					return 0;
				}
				if (!isExpanded)
				{
					return 0 + UsedNodesCount;
				}
				if (nodes==null)
				{
					return 0 + UsedNodesCount;
				}
				return nodes.Sum(x => x.AllUsedNodesCount) + UsedNodesCount;
			}
		}

		WeakReference parent;

		/// <summary>
		/// Gets or sets the parent.
		/// </summary>
		/// <value>The parent.</value>
		public TreeNode<TItem> Parent {
			get {
				if ((parent!=null) && (parent.IsAlive))
				{
					return parent.Target as TreeNode<TItem>;
				}
				return null;
			}
			set {
				SetParentValue(value);
			}
		}

		public List<TreeNode<TItem>> Path {
			get {
				var result = new List<TreeNode<TItem>>();
				var current_parent = Parent;

				while (current_parent!=null)
				{
					result.Add(current_parent);
					current_parent = current_parent.Parent;
				}
				var last = result.Count - 1;
				if ((last>=0) && (result[last].Item==null))
				{
					result.RemoveAt(last);
				}
				return result;
			}
		}

		/// <summary>
		/// Determines whether this instance is parent of node the specified node.
		/// </summary>
		/// <returns><c>true</c> if this instance is parent of node the specified node; otherwise, <c>false</c>.</returns>
		/// <param name="node">Node.</param>
		public bool IsParentOfNode(TreeNode<TItem> node)
		{
			var nodeParent = node.Parent;
			while (nodeParent != null)
			{
				if (nodeParent==this)
				{
					return true;
				}
				nodeParent = nodeParent.Parent;
			}

			return false;
		}

		/// <summary>
		/// Determines whether this instance can be child of the specified newParent.
		/// </summary>
		/// <returns><c>true</c> if this instance can be child of the specified newParent; otherwise, <c>false</c>.</returns>
		/// <param name="newParent">New parent.</param>
		public bool CanBeParent(TreeNode<TItem> newParent)
		{
			if (this==newParent)
			{
				return false;
			}
			return !IsParentOfNode(newParent);
		}

		void SetParentValue(TreeNode<TItem> newParent)
		{
			var oldParent = ((parent!=null) && (parent.IsAlive)) ? parent.Target as TreeNode<TItem> : null;

			if (oldParent==newParent)
			{
				return ;
			}

			if (newParent!=null)
			{
				if (newParent==this)
				{
					throw new ArgumentException("Node cannot be own parent.");
				}
				if (IsParentOfNode(newParent))
				{
					throw new ArgumentException("Own child node cannot be parent node.");
				}
			}

			if (oldParent!=null)
			{
				oldParent.nodes.OnCollectionChange -= oldParent.CollectionChanged;
				oldParent.nodes.Remove(this);
				oldParent.nodes.OnCollectionChange += oldParent.CollectionChanged;
			}

			parent = new WeakReference(newParent);

			if (newParent!=null)
			{
				if (newParent.nodes==null)
				{
					newParent.nodes = new ObservableList<TreeNode<TItem>>();

					newParent.nodes.OnChange += newParent.Changed;
					newParent.nodes.OnCollectionChange += newParent.CollectionChanged;
				}

				newParent.nodes.OnCollectionChange -= newParent.CollectionChanged;
				newParent.nodes.Add(this);
				newParent.nodes.OnCollectionChange += newParent.CollectionChanged;
			}
			//Changed();
		}

		/// <summary>
		/// Initializes a new instance of the class.
		/// </summary>
		/// <param name="nodeItem">Node item.</param>
		/// <param name="nodeNodes">Node nodes.</param>
		/// <param name="nodeIsExpanded">If set to <c>true</c> node is expanded.</param>
		/// <param name="nodeIsVisible">If set to <c>true</c> node is visible.</param>
		public TreeNode(TItem nodeItem,
		                IObservableList<TreeNode<TItem>> nodeNodes = null,
		                bool nodeIsExpanded = false,
		                bool nodeIsVisible = true)
		{
			item = nodeItem;
			nodes = nodeNodes;

			isExpanded = nodeIsExpanded;
			isVisible = nodeIsVisible;

			if (nodes!=null)
			{
				nodes.OnChange += Changed;
				nodes.OnCollectionChange += CollectionChanged;
				CollectionChanged();
			}
		}

		void CollectionChanged()
		{
			if (nodes==null)
			{
				return ;
			}
			nodes.ForEach(SetParent);
		}

		void SetParent(TreeNode<TItem> node)
		{
			if ((node.Parent!=null) && (node.Parent!=this))
			{
				node.Parent.nodes.Remove(node);
			}
			node.parent = new WeakReference(this);
		}

		void Changed()
		{
			Changed("Nodes");
		}

		void Changed(string propertyName)
		{
			if (PauseObservation)
			{
				return ;
			}
			if (OnChange!=null)
			{
				OnChange();
			}
			if (PropertyChanged!=null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		/// <summary>
		/// Determines whether the specified object is equal to the current object.
		/// </summary>
		/// <param name="obj">The object to compare with the current object.</param>
		/// <returns><c>true</c> if the specified object is equal to the current object; otherwise, <c>false</c>.</returns>
		public override bool Equals(System.Object obj)
		{
			//bool r1 = this == obj;
			//if (r1) return r1;
			var nodeObj = obj as TreeNode<TItem>; 
			if (nodeObj==null)
			{
				return this==null;
			}
			if (this==null)
			{
				return false;
			}
            if (item == null)
            {
				//Debug.LogError($"TreeNode.Equals item == null nodeObj:{nodeObj}");
				return false;
            }
			if (nodeObj == null)
			{
				//Debug.LogError($"TreeNode.Equals nodeObj == null item:{item}");
				return false;
			}
			return item.Equals(nodeObj.item);
		}

		/// <summary>
		/// Serves as a hash function for a particular type.
		/// </summary>
		/// <returns>A hash code for this instance that is suitable for use in hashing algorithms and data structures such as a hash table.</returns>
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		/// <summary>
		/// Returns true if the nodes items are equal, false otherwise.
		/// </summary>
		/// <param name="a">The alpha component.</param>
		/// <param name="b">The blue component.</param>
		public static bool operator ==(TreeNode<TItem> a, TreeNode<TItem> b)
		{
			var a_null = object.ReferenceEquals(null, a);
			var b_null = object.ReferenceEquals(null, b);
			if (a_null && b_null)
			{
				return true;
			}
			if (a_null!=b_null)
			{
				return false;
			}

			var a_item_null = object.ReferenceEquals(null, a.item);
			var b_item_null = object.ReferenceEquals(null, b.item);
			if (a_item_null && b_item_null)
			{
				return true;
			}
			if (a_item_null!=b_item_null)
			{
				return false;
			}

			return a.item.Equals(b.item);
		}

		/// <summary>
		/// Returns true if the nodes items are not equal, false otherwise.
		/// </summary>
		/// <param name="a">The alpha component.</param>
		/// <param name="b">The blue component.</param>
		public static bool operator !=(TreeNode<TItem> a, TreeNode<TItem> b)
		{
			var a_null = object.ReferenceEquals(null, a);
			var b_null = object.ReferenceEquals(null, b);
			if (a_null && b_null)
			{
				return false;
			}
			if (a_null!=b_null)
			{
				return true;
			}

			var a_item_null = object.ReferenceEquals(null, a.item);
			var b_item_null = object.ReferenceEquals(null, b.item);
			if (a_item_null && b_item_null)
			{
				return false;
			}
			if (a_item_null!=b_item_null)
			{
				return true;
			}

			return !a.item.Equals(b.item);
		}

		private bool disposed = false;
		
		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		/// <remarks>Call <see cref="Dispose"/> when you are finished using the <see cref="UIWidgets.ObservableList`1"/>. The <see cref="Dispose"/> method leaves the <see cref="UIWidgets.ObservableList`1"/> in an unusable state. After calling <see cref="Dispose"/>, you must release all references to the <see cref="UIWidgets.ObservableList`1"/> so the garbage collector can reclaim the memory that the <see cref="UIWidgets.ObservableList`1"/> was occupying.</remarks>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		void DisposeItem(TreeNode<TItem> node)
		{
			node.OnChange -= Changed;
			node.Dispose();
		}

		/// <summary>
		/// Dispose.
		/// </summary>
		/// <param name="disposing">Free other state (managed objects).</param>
		protected virtual void Dispose(bool disposing)
		{
			if (!disposed)
			{
				if (disposing)
				{
					// Free other state (managed objects).
				}
				if (Nodes!=null)
				{
					Nodes.BeginUpdate();
					Nodes.ForEach(DisposeItem);
					Nodes.EndUpdate();
					Nodes = null;
				}
				
				// Free your own state (unmanaged objects).
				// Set large fields to null.
				disposed = true;
			}
		}

		// Use C# destructor syntax for finalization code.
		~TreeNode()
		{
			// Simply call Dispose(false).
			Dispose(false);
		}
	}

	public static class TreeNodeHelper
    {
		public static string GetTreeNodePath(TreeNode<TreeViewItem> node)
        {
            if (node.Parent != null)
            {
				return GetTreeNodePath(node.Parent) + ">>" + GetTreeNodeName(node);
            }
            else
            {
				return GetTreeNodeName(node);
            }
        }
		public static void SetIsVisibleEx(this TreeNode<TreeViewItem> node,bool isVisible, TreeNode<TreeViewItem> root=null, string path="")
        {
			bool v1 = node.IsVisible;
			node.SetIsVisible(isVisible);

			string nodeName = GetTreeNodeName(node);

            if (nodeName.Contains("100-Undefined-0191-1.6A11SO"))
            {
				//PhysicalTopology topo = node.Item.Tag as PhysicalTopology;
				//var detail = BimNodeHelper_PhysicalTopology.GetLeafKKSDetails(topo);
				Debug.LogError($"SetIsVisibleEx isVisible:{v1}->{node.IsVisible} node:{GetTreeNodePath(node)} root:{GetTreeNodeNameVisible(root)} path:{path}");
            }

            if (node.Parent != null)
            {
                if (node.Parent.Item == null)
                {
                    //Debug.LogError($"SetIsVisibleEx node.Parent.Item==null node:{node.Item.Name}");
                }
                else
                {
					//if(node.Parent.Item.Name.Contains("阀门"))
					//	Debug.LogError($"SetIsVisibleEx isVisible:{v1}->{node.IsVisible} node:{GetTreeNodePath(node)} root:{GetTreeNodeNameVisible(root)} path:{path}");
					
                }
            }
            else
            {
                if (node.Item == null)
                {
                    //Debug.LogError($"SetIsVisibleEx node.Item == null isVisible:{v1}->{node.IsVisible}");
                    if (node.Parent != null)
                    {
                        if (node.Parent.Item == null)
                        {
                            //Debug.LogError($"SetIsVisibleEx node.Parent.Item==null node.Item == null node:{node.Item.Name}");
                        }
                        else
                        {
							//if (node.Parent.Item.Name.Contains("JQ-2-0GMM-11"))
							//	Debug.LogError($"SetIsVisibleEx node:{node.Parent.Item.Name}->NULL isVisible:{v1}->{node.IsVisible}");
						}
                    }
                }
                else
                {
                    //Debug.Log($"SetIsVisibleEx node:NULL->{node.Item.Name} isVisible:{v1}->{node.IsVisible}");
                }
            }
        }

		public static void AutoSetNodeVisible(TreeNode<TreeViewItem> node,TreeNode<TreeViewItem> root, string path)
		{
			if (node == null) return;
			int showCount = node.GetVisibleCount();
			if (showCount > 0)
			{
				SetIsVisibleEx(node, true,root,path);
			}
			else
			{
				//parent.IsVisible = false;
				//node.SetIsVisible(false);
				SetIsVisibleEx(node, false,root,path);
			}
   //         if (node.Parent != null)
   //         {
			//	Debug.LogError($"AutoSetNodeVisible node:{node.Parent.Item.Name}->{node.Item.Name} showCount:{showCount} isVisible:{node.IsVisible}");
			//}
   //         else
   //         {
			//	Debug.LogError($"AutoSetNodeVisible node:NULL->{node.Item.Name} showCount:{showCount} isVisible:{node.IsVisible}");
			//}
		}

		public static string GetTreeNodeName(TreeNode<TreeViewItem> node)
        {
			if (node == null) return "NULL";
            if (node.Item != null)
            {
				return node.Item.Name;
            }
			return node + "";
        }

		public static string GetTreeNodeNameVisible(TreeNode<TreeViewItem> node)
		{
			if (node == null) return "NULL";
			if (node.Item != null)
			{
				if(node.Item.Tag is PhysicalTopology)
                {
					PhysicalTopology topo = node.Item.Tag as PhysicalTopology;
					return $"{node.Item.Name}|{node.IsVisible}|{node.Level}|{node.VisibleCount}/{node.NodesCount}|{topo.Type}";
				}
				else
                {
return $"[{node.Item.Name}|{node.IsVisible}|{node.Level}|{node.VisibleCount}/{node.NodesCount}]";
                }
				
			}
			return $"[{node}|{node.IsVisible}|{node.Level}|{node.VisibleCount}/{node.NodesCount}]";
		}

		public static void AutoSetParentNodeVisible(TreeNode<TreeViewItem> parent)
		{
			if (parent == null) return;
			string path = GetTreeNodeNameVisible(parent);
			AutoSetNodeVisible(parent, parent, path);
			AutoSetParentNodeVisible(parent.Parent, parent, path+"->"+ GetTreeNodeNameVisible(parent.Parent));
		}

		public static void AutoSetParentNodeVisibleFormType(TreeNode<TreeViewItem> parent, TreeNode<TreeViewItem> root,string typeName)
		{
			if (parent == null) return;
			string path = $"【{typeName}】"+GetTreeNodeNameVisible(parent);
			AutoSetNodeVisible(parent, parent, path);
			AutoSetParentNodeVisible(parent.Parent, root, path + "->" + GetTreeNodeNameVisible(parent.Parent));
		}

		public static void AutoSetParentNodeVisible(TreeNode<TreeViewItem> parent,TreeNode<TreeViewItem> root,string path)
        {
            if (parent == null) return;
			AutoSetNodeVisible(parent,root,path);
			AutoSetParentNodeVisible(parent.Parent,root,path+">>"+GetTreeNodeNameVisible(parent.Parent));
        }

        public static void SetParentNodeVisible(TreeNode<TreeViewItem> parent,bool isVisible)
		{
			if (parent == null) return;
            if (isVisible == true)
            {
				parent.SetIsVisibleEx(true);
			}
            else
            {
				int showCount = parent.GetVisibleCount();
				if (showCount > 0)
				{
					//parent.IsVisible = true;
					parent.SetIsVisibleEx(true);
				}
				else
				{
					//parent.IsVisible = false;
					parent.SetIsVisibleEx(false);
				}
			}

			SetParentNodeVisible(parent.Parent, isVisible);
		}
	}
}