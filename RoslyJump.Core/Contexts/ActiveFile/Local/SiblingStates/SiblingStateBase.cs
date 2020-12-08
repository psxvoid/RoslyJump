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

        public virtual void Next()
        {
            if (!this.HasTargets)
            {
                return;
            }

            this.ActiveIndex++;

            if (this.ActiveIndex >= this.Targets.Length)
            {
                this.ActiveIndex = 0;
            }
        }

        public virtual void Prev()
        {
            if (!this.HasTargets)
            {
                return;
            }

            this.ActiveIndex++;

            if (this.ActiveIndex >= this.Targets.Length)
            {
                this.ActiveIndex = 0;
            }
        }
    }
}
