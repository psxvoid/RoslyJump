using System;
using System.Collections.Generic;
using System.Linq;
using dngrep.core.VirtualNodes;
using Microsoft.CodeAnalysis.CSharp;

namespace RoslyJump.Core.Contexts.ActiveFile.Local.SiblingStates
{
    public abstract class SiblingStateBase
    {
        private class DefaultComparer : IEqualityComparer<CombinedSyntaxNode?>
        {
            public bool Equals(CombinedSyntaxNode? x, CombinedSyntaxNode? y)
            {
                return x?.MixedNode.GetType() == y?.MixedNode.GetType();
            }

            public int GetHashCode(CombinedSyntaxNode? obj)
            {
                return obj.GetHashCode();
            }
        }

        protected static readonly IEqualityComparer<CombinedSyntaxNode?> ComparerInstance =
            new DefaultComparer();

        public SiblingStateBase(CombinedSyntaxNode baseNode)
        {
            this.BaseNode = baseNode;
        }

        internal CombinedSyntaxNode BaseNode { get; private set; }

        public int ActiveIndex { get; private set; } = -1;

        protected CombinedSyntaxNode[] Targets { get; private set; }
            = Array.Empty<CombinedSyntaxNode>();

        protected virtual IEqualityComparer<CombinedSyntaxNode?> Comparer => ComparerInstance;

        protected abstract CombinedSyntaxNode[] QueryTargetsProtected(CombinedSyntaxNode root);

        public void QueryTargets()
        {
            this.Targets = this.QueryTargetsProtected(this.BaseNode);

            bool hasSiblingsOfTheSameKind = this.Targets
                .GroupBy(x => x.BaseNode.Kind())
                .Any(x => x.Count() > 1);

            if (hasSiblingsOfTheSameKind)
            {
                throw new InvalidOperationException(
                    "Only unique kinds are allowed for sibling states. " +
                    "Please, filter siblings first before providing them as query targets. " +
                    "This is required in order to correctly find next/prev sibling.");
            }
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
                    if (this.Comparer.Equals(this.Targets[i], node))
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
                    if (this.Targets[i].MixedNode.GetType() == node?.MixedNode.GetType())
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
