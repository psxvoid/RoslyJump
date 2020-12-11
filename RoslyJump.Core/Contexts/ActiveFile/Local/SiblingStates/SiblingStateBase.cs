using System;
using dngrep.core.VirtualNodes;

namespace RoslyJump.Core.Contexts.ActiveFile.Local.SiblingStates
{
    public abstract class SiblingStateBase
    {
        public SiblingStateBase(CombinedSyntaxNode baseNode)
        {
            this.BaseNode = baseNode;
        }

        private CombinedSyntaxNode BaseNode { get; set; }

        public int ActiveIndex { get; private set; } = -1;

        protected CombinedSyntaxNode[] Targets { get; private set; }
            = Array.Empty<CombinedSyntaxNode>();

        protected abstract CombinedSyntaxNode[] QueryTargetsProtected(CombinedSyntaxNode root);

        public void QueryTargets()
        {
            this.Targets = this.QueryTargetsProtected(this.BaseNode);
        }

        public virtual CombinedSyntaxNode Target
        {
            get
            {
                if (this.ActiveIndex == -1)
                {
                    throw new InvalidOperationException(
                        "The sibling target is not set." +
                        " Please, ensure to call Next or Prev methods before accessing the target." +
                        " Also, be sure the sibling state has non-empty query targets.");
                }

                return this.Targets[this.ActiveIndex];
            }
        }

        public bool HasTargets => this.Targets.Length > 0;

        public virtual void Next(CombinedSyntaxNode? node)
        {
            if (!this.HasTargets)
            {
                return;
            }

            if (node != null && node?.BaseNode != null)
            {
                int activeIndex = -1;

                for (int i = 0; i < this.Targets.Length; i++)
                {
                    if (this.Targets[i].BaseNode.GetType() == node?.BaseNode.GetType())
                    {
                        activeIndex = i;
                        break;
                    }
                }

                this.ActiveIndex = activeIndex + 1;
            }
            else
            {
                this.ActiveIndex++;
            }

            if (this.ActiveIndex >= this.Targets.Length)
            {
                this.ActiveIndex = 0;
            }
        }

        public virtual void Prev(CombinedSyntaxNode? node)
        {
            if (!this.HasTargets)
            {
                return;
            }

            if (node != null && node?.BaseNode != null)
            {
                int activeIndex = -1;

                for (int i = 0; i < this.Targets.Length; i++)
                {
                    if (this.Targets[i].BaseNode.GetType() == node?.BaseNode.GetType())
                    {
                        activeIndex = i;
                        break;
                    }
                }

                this.ActiveIndex = activeIndex - 1;
            }
            else
            {
                this.ActiveIndex--;
            }

            if (this.ActiveIndex < 0)
            {
                this.ActiveIndex = this.Targets.Length - 1;
            }
        }

        public virtual bool HasSibling(CombinedSyntaxNode node)
        {
            return node.BaseNode.Parent == this.BaseNode.BaseNode;
        }
    }
}
