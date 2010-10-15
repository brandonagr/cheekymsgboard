using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace MessageBoardLib
{
	/// <summary>
	/// Class to easily profile execution time
	/// </summary>
	public static class ProfileTimer
	{
		#region Declarations

		private static readonly Stopwatch sw = new Stopwatch();
		private static readonly bool _includeStackData = false; //should be false unless runing this locally in debug mode
		private static readonly bool _mergeLeaves = true;
		private static readonly Stack<ProfileDataNode> _data = new Stack<ProfileDataNode>();

		public static bool EchoPush = false;

		#endregion

		#region Instantiation & Setup

		static ProfileTimer()
		{
			sw.Start();
		}

		#endregion

		#region Public Methods

		public static void Push(string desc)
		{
			if (EchoPush)
				Console.WriteLine(desc + "...");

			if (_data.Count > 100)
				throw new ApplicationException("You are infinitely pushing!");

			ProfileDataNode data = new ProfileDataNode();

			if (_includeStackData)
			{
				StackTrace st = new StackTrace(true);
				StackFrame sf = st.GetFrame(1);

				string location = sf.GetFileName();
				if (!string.IsNullOrEmpty(location))
				{
					location = location.Substring(location.LastIndexOf('\\') + 1);

					data.Location = " [ " + sf.GetMethod().Name + " " + location + ":" + sf.GetFileLineNumber() + " ]";
				}
			}

			data.Desc = desc;
			data.Depth = _data.Count;

			_data.Push(data);
			_data.Peek().StartTicks = sw.ElapsedTicks; //make this the absolute last thing it does so the time only takes into account what it's measuring and not the additional time to push stuff onto the stack
		}

		public static void Pop(string desc)
		{
			if (_data.Count == 0)
				return; //throw new ApplicationException("You popped too many times!");				

			ProfileDataNode data = _data.Pop();
			data.TotalTicks = sw.ElapsedTicks - data.StartTicks;

			if (_data.Count > 0)
			{
				data.Parent = _data.Peek();

				//Combine multiple calls to the same leaf node into just one output line
				if (_mergeLeaves)
				{
					ProfileDataNode previous = data.Parent.PreviousChild();
					if (previous != null)
					{
						if (data.Children.Count == 0 && previous.Desc == data.Desc)
						{
							previous.CombineWith(data);
							data.Parent.RemoveLastChild();
						}
					}
				}
			}
			else
			{
				Console.WriteLine("");
				Output(data);
				Console.WriteLine("");
			}
		}

		public static void PopPush(string pop, string push)
		{
			Pop(pop);
			Push(push);
		}

		public static void EndAll()
		{
			while (_data.Count > 0)
				Pop(string.Empty);
		}

		/// <summary>
		/// Output Total time in this node, percent of parent, percentage of total time not including children
		/// </summary>
		/// <param name="cur"></param>
		private static void Output(ProfileDataNode cur)
		{
			double percentOfParent = 100.0;
			if (cur.Parent != null)
				percentOfParent = (cur.TotalTicks / (double)cur.Parent.TotalTicks) * 100.0;

			ProfileDataNode root = cur;
			while (root.Parent != null)
				root = root.Parent;

			long totalAccountedForInChildren = 0;
			foreach (ProfileDataNode child in cur.Children)
				totalAccountedForInChildren += child.TotalTicks;

			double percentOfTotal = ((cur.TotalTicks - totalAccountedForInChildren) / (double)root.TotalTicks) * 100.0;

			for (int i = 0; i < cur.Depth; i++)
				Console.Write("|\t");

			string seconds = (cur.TotalTicks / (double)Stopwatch.Frequency).ToString("0.000");

			Console.WriteLine(seconds + " (" + percentOfParent.ToString("0.0") + "% | " + percentOfTotal.ToString("0.0") + "%) " +
								(cur.Occurances > 0 ? ((cur.Occurances + 1) + "x ") : ("")) +
								"'" + cur.Desc + "' " +
								cur.Location);

			foreach (ProfileDataNode child in cur.Children)
				Output(child);
		}

		#endregion
	}

	/// <summary>
	/// Used for stack to record
	/// </summary>
	class ProfileDataNode
	{
		public long StartTicks { get; set; }
		public string Location { get; set; }
		public string Desc { get; set; }
		public int Occurances { get; set; }

		//Stuff used after popped off stack seconds
		private readonly List<ProfileDataNode> _children = new List<ProfileDataNode>();
		private ProfileDataNode _parent;

		public long TotalTicks { get; set; }
		public int Depth { get; set; }
		public ProfileDataNode Parent
		{
			get { return _parent; }
			set
			{
				_parent = value;
				_parent.Children.Add(this);
			}
		}
		public List<ProfileDataNode> Children
		{
			get { return _children; }
		}

		public ProfileDataNode PreviousChild()
		{
			if (_children.Count < 2)
				return null;

			return _children[_children.Count - 2];
		}

		public void CombineWith(ProfileDataNode data)
		{
			Occurances++;
			TotalTicks += data.TotalTicks;
		}

		public void RemoveLastChild()
		{
			_children.RemoveAt(_children.Count - 1);
		}
	}
}
