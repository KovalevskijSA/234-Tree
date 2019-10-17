using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _24tree
{
	class Program
	{
		static void Main(string[] args)
		{
		
			Tree234 tree = new Tree234(); 
			int n = 0;
			Random rnd = new Random((int)(DateTime.Now.ToBinary()));
			for (int i = 0; i < n; ++i)
			{
				tree.Insert(rnd.Next(0, 100));
			}

			tree.DisplayTree();
			Console.WriteLine();
			tree.root.Print();
			while (true)
			{
				string str = Console.ReadLine();
				if (str == "e") return;
				int len = str.IndexOf("(");
				int k;
				int end = str.IndexOf(")") - len - 1;
				if (end < 0) continue;
				int.TryParse(str.Substring(len + 1, end), out k);
				switch (str.Substring(0, len))
				{
					case "add":
						tree.Insert(k);
						break;
					case "del":
						tree.Remove(k);
						break;
					case "rand":
						tree = new Tree234();
						for (int i = 0; i < k; ++i)
							tree.Insert(rnd.Next(0, 100));
						break;
					case "pred":
						tree.Predecessor(k);
						break;
					case "suc":
						tree.Successor(k);
						break;
				}
				tree.DisplayTree();
				tree.root.Print(textFormat: "(0)", spacing: 2);
			}
		}
	}


	public class DataItem
	{

		public int dData;

		public DataItem(int dd)
		{    
			dData = dd;
		}
		public void DisplayItem()
		{
			Console.Write("(" + dData.ToString()+")");
		}
	}

	public class Node
	{
		public int numItems;
		public Node parent;
		public Node[] childArray = new Node[4];
		public DataItem[] itemArray = new DataItem[3];

		public void RefreshNumItem()
		{ 
			for (numItems = 0;numItems < 3 && itemArray[numItems] != null; numItems++);
		}
		public void ConnectChild(int childNum, Node child)
		{
			childArray[childNum] = child;
			if (child != null)
				child.parent = this;
		}

		public Node DisconnectChild(int childNum)
		{
			Node tempNode = childArray[childNum];
			childArray[childNum] = null;

			return tempNode;
		}

		public Node getChild(int childNum)
		{
			return childArray[childNum];
		}
		public Node getLastChild()
		{
			if (childArray[3] != null) return childArray[3];
			if (childArray[2] != null) return childArray[2];
			if (childArray[1] != null) return childArray[1];
			return childArray[0];
		}

		public Node getParent()
		{
			return parent;
		}

		public bool isLeaf()
		{
			return (childArray[0] == null) ? true : false;
		}

		public int getNumItems()
		{
			return numItems;
		}

		public DataItem getItem(int index)
		{
			return itemArray[index];
		}

		public bool isFull()
		{
			return (numItems == 3) ? true : false;
		}

		public int FindItem(long key)
		{
			for (int j = 0; j < 3; j++)
			{
				if (itemArray[j] == null)
					break;
				else if (itemArray[j].dData == key)
					return j;
			}
			return -1;
		}

		public int InsertItem(DataItem newItem)
		{
			//if (this.findItem(newItem.dData) != -1) return 0;
			numItems++;
			long newKey = newItem.dData;

			for (int j = 2; j >= 0; j--)
			{
				if (itemArray[j] == null)
					continue;
				else
				{
					long itsKey = itemArray[j].dData;

					if (newKey < itsKey)
						itemArray[j + 1] = itemArray[j];
					else
					{
						itemArray[j + 1] = newItem;
						return j + 1;
					}
				}
			}
			itemArray[0] = newItem;
			return 0;
		}

		public DataItem RemoveItem()
		{
			DataItem temp = itemArray[numItems - 1];
			itemArray[numItems - 1] = null;
			numItems--;
			return temp;
		}
		public DataItem RemoveItem(int key)
		{
			DataItem temp = null;
			int i;
			for (i = 0; i < numItems; i++)
				if (itemArray[i].dData == key)
				{
					temp = itemArray[i];
					break;
				}
			for (; i < numItems - 1; i++)
				itemArray[i] = itemArray[i + 1];
			itemArray[i] = null;
			numItems--;
			return temp;

		}

		public void DisplayNode()
		{ 
			for (int j = 0;j < 3 && itemArray[j] != null; j++)
				itemArray[j].DisplayItem();
			Console.WriteLine("/");
		}
		public string ItemArrayToString()
		{
			string s = "";
			for (int i = 0; i < numItems; i++)
			{
				s += "(" + itemArray[i].dData.ToString() +")";

			}
			return s;
		}
	}

	class Tree234
	{

		public Node root = new Node();

		public int Find(long key)
		{
			Node curNode = root;
			int childNumber;

			while (true)
			{
				if ((childNumber = curNode.FindItem(key)) != -1)
					return childNumber;
				else if (curNode.isLeaf())
					return -1;
				else
					curNode = getNextChild(curNode, key);
			}
		}

		public Node FindNode(int key)
		{
			Node curNode = root;
			int childNumber;

			while (true)
			{
				if ((childNumber = curNode.FindItem(key)) != -1)
					return curNode;
				else if (curNode.isLeaf())
					return null;
				else
					curNode = getNextChild(curNode, key);
			}
		}

		public Node getNextChild(Node theNode, long theValue)
		{
			int j;
			int numItems = theNode.getNumItems();

			for (j = 0; j < numItems; j++)
			{
				if (theValue < theNode.getItem(j).dData)
					return theNode.getChild(j);
			}
			return theNode.getChild(j);
		}

		public void Insert(int dValue)
		{
			Node curNode = root;
			if (FindNode(dValue) != null) return;
			DataItem tempItem = new DataItem(dValue);

			while (true)
			{
				if (curNode.isFull())
				{
					Split(curNode);
					curNode = curNode.getParent();

					curNode = getNextChild(curNode, dValue);
				}
				else if (curNode.isLeaf())
					break;
				else
					curNode = getNextChild(curNode, dValue);
			}
			curNode.InsertItem(tempItem);
		}


		public void Split(Node thisNode)
		{
			DataItem itemB, itemC;

			Node parent, child2, child3;
			int itemIndex;

			itemC = thisNode.RemoveItem();
			itemB = thisNode.RemoveItem();
			child2 = thisNode.childArray[2];
			child3 = thisNode.childArray[3];
		
			thisNode.childArray[2] = null;
			thisNode.childArray[3] = null;
			Node newRight = new Node();
			if (thisNode == root)
			{
				root = new Node();
				parent = root;
				root.childArray[0] = thisNode;
				thisNode.parent = root;
			}
			else
				parent = thisNode.getParent();
			itemIndex = parent.InsertItem(itemB);
			int n = parent.getNumItems();

			for (int j = n - 1; j > itemIndex; j--)
			{
				parent.childArray[j + 1] = parent.childArray[j];
				parent.childArray[j + 1].parent = parent;
			}
			parent.childArray[itemIndex + 1] = newRight;
			newRight.parent = parent;

			newRight.InsertItem(itemC); 
			newRight.ConnectChild(0, child2);
			newRight.ConnectChild(1, child3);
			parent.RefreshNumItem();
			newRight.RefreshNumItem();

		}

		public void Remove(int k)
		{
			Node p = FindNode(k);
			if (p == null) return;
			if (p.isLeaf())
			{
				p.RemoveItem(k);
			}
			else
			{
				int posItem = p.FindItem(k);
				Node temp = FindMin(p.childArray[posItem + 1]);
				p.itemArray[posItem] = temp.itemArray[0];
				temp.RemoveItem(p.itemArray[posItem].dData);
				p = temp;
			}

			if (p.itemArray[0] == null)
			{
				HandleUnderflow(p, null);
			}
		}
		public Node FindMin(Node cur)
		{
			if (cur == null) return null;
			while (cur.getChild(0) != null)
				cur = cur.getChild(0);
			return cur;
		}
		public Node FindMax(Node cur)
		{
			while (cur.getLastChild() != null)
				cur = cur.getLastChild();
			return cur;
		}

		public void HandleUnderflow(Node p, Node Z)
		{
			if (p == root)
			{
				root = Z;    
				return;
			}

			Node parent;
			int pos;

			parent = p.parent;

			for (pos = 0; pos < 4; pos++)
			{
				if (parent.getChild(pos) == p)
					break;
			}
			if ((pos <= 2 && parent.getChild(pos + 1) != null) 
				 && parent.getChild(pos + 1).itemArray[1] != null)
			{
				Node R_sibling = parent.childArray[pos + 1];

				p.childArray[0] = Z; 

				if (Z != null) Z.parent = p;

				p.itemArray[0] = parent.itemArray[pos];

				p.childArray[1] = R_sibling.childArray[0];  
				if (p.childArray[1] != null)    
					p.childArray[1].parent = p;

				parent.itemArray[pos] = R_sibling.itemArray[0]; 
				for (int i = 0; i < 2; i++)
				{
					R_sibling.itemArray[i] = R_sibling.itemArray[i + 1];
					R_sibling.childArray[i] = R_sibling.childArray[i + 1];
				}
				R_sibling.childArray[2] = R_sibling.childArray[3];
				R_sibling.itemArray[2] = null;
				R_sibling.childArray[3] = null;
				R_sibling.RefreshNumItem();
				p.RefreshNumItem();

				return;
			}
			else if (pos > 0  && parent.childArray[pos - 1].itemArray[1] != null)
			{
				Node L_sibling = parent.childArray[pos - 1];
				int last = 1;
				if (L_sibling.itemArray[2] != null) last = 2;
				p.childArray[0] = L_sibling.childArray[last + 1]; 
				if (p.childArray[0] != null)    
					p.childArray[0].parent = p;

				p.itemArray[0] = parent.itemArray[pos - 1];            

				p.childArray[1] = Z;       
				if (Z != null) Z.parent = p;

				parent.itemArray[pos - 1] = L_sibling.itemArray[last];  

				L_sibling.itemArray[last] = null;
				L_sibling.childArray[last + 1] = null;
				L_sibling.numItems--;

				return; 
			}
			else if (pos != 3  && parent.childArray[pos + 1] != null )
			{
				Node R_sibling = parent.childArray[pos + 1];  

				p.childArray[0] = Z;        
				if (Z != null) Z.parent = p;

				p.itemArray[0] = parent.itemArray[pos];              

				p.childArray[1] = R_sibling.childArray[0];      
				if (p.childArray[1] != null)    
					p.childArray[1].parent = p;

				p.itemArray[1] = R_sibling.itemArray[0];       

				p.childArray[2] = R_sibling.childArray[1];      
				if (p.childArray[2] != null)     
					p.childArray[2].parent = p;

				for (int i = pos; i < 2; i++)
				{
					parent.itemArray[i] = parent.itemArray[i + 1];
					parent.childArray[i + 1] = parent.childArray[i + 2];
				}
				parent.itemArray[2] = null;
				parent.childArray[3] = null;
				parent.RefreshNumItem();
				p.RefreshNumItem();
				if (parent.itemArray[0] == null)
					HandleUnderflow(parent, p);
			}
			else 
			{
				Node L_sibling = parent.childArray[pos - 1]; 

				L_sibling.itemArray[1] = parent.itemArray[pos - 1];   
				L_sibling.childArray[2] = Z; 
				if (Z != null)
					Z.parent = L_sibling;
				for (int i = pos - 1; i < 2; i++)
				{
					parent.itemArray[i] = parent.itemArray[i + 1];
					parent.childArray[i + 1] = parent.childArray[i + 2];
				}
				parent.itemArray[2] = null;
				parent.childArray[3] = null;
				parent.RefreshNumItem();
				p.RefreshNumItem();

				if (parent.itemArray[0] == null)
					HandleUnderflow(parent, L_sibling);
			}
		}


		public void Successor(int k)
		{
			Node temp = FindNode(k);
			if (temp == null) Console.WriteLine("ключа {0} нет", k);
			int pos;
			temp = SuccessorNode(temp, k, out pos);
			if (temp == null) Console.WriteLine("Нет приемника");
			else Console.WriteLine(temp.itemArray[pos].dData);
		}
		public Node SuccessorNode(Node cur, int k, out int pos)
		{
			pos = -1;
			if (cur == null) return null;
			
			int posKey = cur.FindItem(k);
			if (cur.isLeaf())
			{
				//if (pos < 3 && cur.itemArray[pos + 1] != null) { pos = 1; return cur; }
				while(cur != null)
				{
					int i;
					for(i = 0; i < cur.numItems; i++)
						if (k < cur.itemArray[i].dData)
						{
							pos = i;
							return cur;
						}
					cur = cur.parent;
				}
				pos = -1;
				return null;
			}
			else
			{
				pos = 0;
				Node t = FindMin(cur.childArray[posKey + 1]);
				if (t.itemArray[0].dData == k) return SuccessorNode(t, k, out pos);
				else return t;
			}
		}

		public void Predecessor(int k)
		{
			Node temp = FindNode(k);
			if (temp == null) Console.WriteLine("ключа {0} нет", k);
			int pos;
			temp = PredecessorNode(temp, k, out pos);
			if (temp == null) Console.WriteLine("Нет предшественника");
			else Console.WriteLine(temp.itemArray[pos].dData);
		}

		public Node PredecessorNode(Node cur, int k, out int pos)
		{
			pos = -1;
			if (cur == null) return null;
			int posKey = cur.FindItem(k);
			if (cur.isLeaf())
			{
				while (cur != null)
				{
					int i;
					for (i = cur.numItems - 1; i >= 0; i--)
						if (k > cur.itemArray[i].dData)
						{
							pos = i;
							return cur;
						}
					cur = cur.parent;
				}
				pos = -1;
				return null;
			}
			else
			{
				Node t = FindMax(cur.childArray[posKey]);
				pos = t.numItems - 1;
				if (t.itemArray[0].dData == k) return PredecessorNode(t, k, out pos);
				else return t;
			}
		}

		public void DisplayTree()
		{
			RecDisplayTree(root, 0, 0);
		}
		
		private void RecDisplayTree(Node thisNode, int level, int childNumber)
		{
			Console.Write("level=" + level + " child=" + childNumber + " ");
			if (thisNode == null) return;
			thisNode.DisplayNode();

			thisNode.RefreshNumItem();
			int numItems = thisNode.getNumItems();

			for (int j = 0;j < numItems + 1; j++)
			{
				Node nextNode = thisNode.getChild(j);

				if (nextNode != null)
					RecDisplayTree(nextNode, level + 1, j);
				else
					return;
			}
		}

        private void Padding(String s, int n)
		{
			int i;

			for (i = 0; i < n; i++)
				Console.Write(s);
		}


        private int MaxLevel = 0;
        private void PrintSub(Node p, int id, int level)
		{
			if (level > MaxLevel)
				MaxLevel = level;

			int i;

			if (p == null)
				return;


			if (p.childArray[3] != null)
			{
				PrintSub(p.childArray[3], 3, level + 1);
			}

			if (p.childArray[2] != null)
			{
				PrintSub(p.childArray[2], 2, level + 1);
			}

			if (p.childArray[2] != null)
			{
				Padding("             ", level);
				Console.WriteLine("" + id + ":" + p);

				if (id == 0 && level == MaxLevel)
					Console.WriteLine();

				if (p.childArray[1] != null)
				{
					PrintSub(p.childArray[1], 1, level + 1);
				}
			}
			else
			{
				if (p.childArray[1] != null)
				{
					PrintSub(p.childArray[1], 1, level + 1);
				}

				Padding("             ", level);
				Console.WriteLine("" + id + ":" + p);

				if (id == 0 && level == MaxLevel)
					Console.WriteLine();
			}

			if (p.childArray[0] != null)
			{
				PrintSub(p.childArray[0], 0, level + 1);
				//        System.out.println();
			}
		}

		public void PrintTree()
		{
			MaxLevel = 0;
			Console.WriteLine();
			PrintSub(root, 0, 0);
		}
	}
	
	public static class TreePrinter
	{
		class NodeInfo
		{
			public Node node;
			public string Text;
			public int StartPos;
			public int Size { get { return Text.Length; } }
			public int EndPos { get { return StartPos + Size; } set { StartPos = value - Size; } }
			public NodeInfo Parent, Left, LMiddle, RMiddle, Right;
		}


		public static void Print(this Node root, string textFormat = "0", int spacing = 1, int topMargin = 1, int leftMargin = 1)
		{
			if (root == null) return;
			int rootTop = Console.CursorTop + topMargin;
			var last = new System.Collections.Generic.List<NodeInfo>();
			var next = root;
			for (int level = 0; next != null; level++)
			{
			
				var item = new NodeInfo { node = next, Text = next.ItemArrayToString() };
				if (level < last.Count)
				{
					item.StartPos = last[level].EndPos + spacing;
					last[level] = item;
				}
				else
				{
					item.StartPos = leftMargin;
					last.Add(item);
				}
				if (level > 0)
				{
					item.Parent = last[level - 1];
					if (next == item.Parent.node.childArray[0])
					{
						item.Parent.Left = item;
					}
					if (next == item.Parent.node.childArray[1])
					{
						item.Parent.LMiddle = item;
											}
					if (next == item.Parent.node.childArray[2])
					{
						item.Parent.RMiddle = item;
					}
					if (next == item.Parent.node.childArray[3])
					{
						item.Parent.Right = item;
					}
				}

				next = next.childArray[0];
				for (; next == null; item = item.Parent)
				{
					int top = rootTop + 2 * level;
					item.node.RefreshNumItem();
					if (item.Left != null)
					{
						Print("/", top + 1, item.Left.EndPos - 1);
					}
					if (item.LMiddle != null)
					{
						if (item.node.getNumItems() == 1)
						{
							Print("\\", top + 1, item.LMiddle.StartPos);
							
						}
						else Print("|", top + 1, (item.LMiddle.StartPos + item.LMiddle.EndPos)/2);
						Print("_", top, item.Left.EndPos, item.LMiddle.StartPos);

					}
					if (item.RMiddle != null)
					{
						if (item.node.getNumItems() == 2)
						{
							Print("\\", top + 1, item.RMiddle.StartPos);
						}
						else
						{
							Print("|", top + 1, (item.RMiddle.StartPos + item.RMiddle.EndPos) / 2);
						}
						Print("_", top, item.LMiddle.StartPos, item.RMiddle.StartPos);
					}
					if (item.Right != null)
					{
						Print("\\", top + 1, item.Right.StartPos);
						Print("_", top, item.RMiddle.StartPos, item.Right.StartPos);

					}
					Print(item.Text, top, item.StartPos);

					if (--level < 0) break;
					item.Parent.StartPos = (item.StartPos + item.Parent.Left.EndPos - item.Parent.Size) / 2;

					if (item == item.Parent.Left)
					{
						next = item.Parent.node.childArray[1];
					}
					else if (item == item.Parent.LMiddle)
					{
						next = item.Parent.node.childArray[2];
					}
					else if (item == item.Parent.RMiddle)
					{
						next = item.Parent.node.childArray[3];
					}
				}
			}
			Console.SetCursorPosition(0, rootTop + 2 * last.Count - 1);
		}

		private static void Print(string s, int top, int left, int right = -1)
		{
			Console.SetCursorPosition(left, top);
			if (right < 0) right = left + s.Length;
			while (Console.CursorLeft < right) Console.Write(s);
		}
	}
}
