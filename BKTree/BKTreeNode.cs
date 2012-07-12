using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BKTree
{
    public abstract class BKTreeNode
    {
        private readonly Dictionary<Int32, BKTreeNode> _children;

        public BKTreeNode()
        {
            _children = new Dictionary<Int32, BKTreeNode>();
        }

        public virtual void add(BKTreeNode node)
        {
            int distance = calculateDistance(node);

            if (_children.ContainsKey(distance))
            {
                _children[distance].add(node);
            }
            else
            {
                _children.Add(distance, node);
            }
        }

        public virtual int findBestMatch(BKTreeNode node, int bestDistance, out BKTreeNode bestNode)
        {
            int distanceAtNode = calculateDistance(node);

            bestNode = node;

            if(distanceAtNode < bestDistance)
            {
                bestDistance = distanceAtNode;
                bestNode = this;
            }
                        
            int possibleBest = bestDistance;

            foreach (Int32 distance in _children.Keys)
            {
                if (distance < distanceAtNode + bestDistance)
                {
                    possibleBest = _children[distance].findBestMatch(node, bestDistance, out bestNode);
                    if (possibleBest < bestDistance)
                    {
                        bestDistance = possibleBest;
                    }
                }
            }

            return bestDistance;
        }

        public virtual void query(BKTreeNode node, int threshold, Dictionary<BKTreeNode, Int32> collected)
        {
            int distanceAtNode = calculateDistance(node);

            if (distanceAtNode == threshold)
            {
                collected.Add(this, distanceAtNode);
                return;
            }

            if (distanceAtNode < threshold)
            {
                collected.Add(this, distanceAtNode);
            }

            for (int distance = (distanceAtNode - threshold); distance <= (threshold + distanceAtNode); distance++)
            {
                if (_children.ContainsKey(distance))
                {
                    _children[distance].query(node, threshold, collected);
                }
            }
        }

        protected abstract int calculateDistance(BKTreeNode node);
    }
}
