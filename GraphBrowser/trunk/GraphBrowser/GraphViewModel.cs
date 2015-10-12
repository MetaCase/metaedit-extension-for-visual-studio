using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace MetaCase.GraphBrowser
{
    class GraphViewModel
    {
        private Graph graph;
		private GraphViewModel parent;
        public bool IsNodeExpanded { get; set; }
        public bool IsSelected { get; set; }
        public string Name
        {
            get { return this.ToString(); }
        }
		private readonly ICollection<GraphViewModel> _children = new ObservableCollection<GraphViewModel>();
        public ICollection<GraphViewModel> children
        {
            get { return _children; }
        }
		
		public GraphViewModel(Graph _graph)
        {
			this.graph = _graph;
			this._children = new List<GraphViewModel>();
		}

		public GraphViewModel()
        {
			this.graph = null;
			this._children = new List<GraphViewModel>();
		}

		public void setParent(GraphViewModel parent)
        {
			this.parent = parent;
		}

		public GraphViewModel getParent()
        {
			return parent;
		}

        public override string ToString()
        {
            string line = this.graph.Name;
            if (GraphBrowser.ShowGraphType) line += ": " + this.graph.TypeName;
            return line;
        }

		public Graph getGraph()
        {
			return this.graph;
		}

        public void addChild(GraphViewModel child)
        {
			_children.Add(child);
			child.setParent(this);
		}

		public void removeChild(GraphViewModel child)
        {
			_children.Remove(child);
			child.setParent(null);
		}

		public GraphViewModel [] getChildren()
        {
            return _children.ToArray();
		}

		public Boolean hasChildren() {
			return _children.Count > 0;
		}
		
		public void populate(Graph[] graphs, List<Graph> stack)
        {
			if (!stack.Contains(this.getGraph())) {
				stack.Add(this.getGraph());
				foreach (Graph g in graphs) {
					GraphViewModel gvm = new GraphViewModel(g);
					this.addChild(gvm);
                    Graph[] children = g.GetChildren();
					if (children != null && children.Count() > 0) {
                        Array.Sort(children);
						gvm.populate(children, stack);
					}
				}
                stack.RemoveAt(stack.Count() - 1);
			}
		}
    }
}
