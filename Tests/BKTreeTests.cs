using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BKTree;
using Xunit;

namespace UnitTests
{
    public class BKTreeTests
    {

        #region Derived BKTree node

        private class TestNode : BKTreeNode
        {
            public TestNode(ushort id, int[] values) : base()
            {
                Data = values;
                Id   = id;
            }

            public TestNode(int[] values)
                : base()
            {
                Data = values;
                Id = 0;
            }

            public ushort Id { get; private set; }
            public int[] Data { get; private set; }

            override protected int calculateDistance(BKTreeNode node)
            {
                return DistanceMetric.calculateLeeDistance(
                    this.Data,
                    ((TestNode)node).Data);
            }
        }

        #endregion

        #region Unit tests

        /* Tests use xunit 1.9.1
         * http://xunit.codeplex.com
         */

        [Fact]
        public void BKTree_should_CalculateVarietyOfDistances()
        {
            Assert.Equal( 10,
                DistanceMetric.calculateHammingDistance(
                    new byte[] { 0xEF, 0x35, 0x20 },
                    new byte[] { 0xAD, 0x13, 0x87 }));

            Assert.Equal( 101,
                DistanceMetric.calculateLeeDistance(
                    new int[] { 196, 105, 48 },
                    new int[] { 201, 12, 51 }));

            Assert.Equal( 3,
                DistanceMetric.calculateLevenshteinDistance(
                    "kitten",
                    "sitting"));

        }

        [Fact]
        public void BKTree_should_FindBestDistance()
        {
            BKTree<TestNode> tree = new BKTree<TestNode>();

            TestNode search = new TestNode(new int[] { 118, 223, 316 });
            TestNode best   = new TestNode(3, new int[] { 120, 220, 320 });

            tree.add(new TestNode(1, new int[] { 100, 200, 300 }));
            tree.add(new TestNode(2, new int[] { 110, 210, 310 }));
            tree.add(best);
            tree.add(new TestNode(4, new int[] { 130, 230, 330 }));
            tree.add(new TestNode(5, new int[] { 140, 240, 340 }));

            Assert.Equal(9, DistanceMetric.calculateLeeDistance(search.Data, best.Data));
            Assert.Equal(9, tree.findBestDistance(search));
        }

        [Fact]
        public void BKTree_should_FindBestNode()
        {
            BKTree<TestNode> tree = new BKTree<TestNode>();

            TestNode search = new TestNode(new int[] { 210, 175, 233 });
            TestNode best = new TestNode(2, new int[] { 200, 200, 200 });

            tree.add(new TestNode(1, new int[] { 100, 100, 100 }));
            tree.add(best);
            tree.add(new TestNode(3, new int[] { 300, 300, 300 }));
            tree.add(new TestNode(4, new int[] { 400, 400, 400 }));
            tree.add(new TestNode(5, new int[] { 500, 500, 500 }));

            TestNode found = tree.findBestNode(search);

            Assert.Equal(2, found.Id);
            Assert.Equal(best.Data, found.Data);
        }

        [Fact]
        public void BKTree_should_FindBestNodeWithDistance()
        {
            BKTree<TestNode> tree = new BKTree<TestNode>();

            TestNode search = new TestNode(new int[] { 365, 422, 399 });
            TestNode best = new TestNode(4, new int[] { 400, 400, 400 });

            tree.add(new TestNode(1, new int[] { 100, 100, 100 }));
            tree.add(new TestNode(2, new int[] { 200, 200, 200 }));
            tree.add(new TestNode(3, new int[] { 300, 300, 300 }));
            tree.add(best);
            tree.add(new TestNode(5, new int[] { 500, 500, 500 }));

            Dictionary<TestNode,Int32> result = tree.findBestNodeWithDistance(search);

            Assert.Equal(1, result.Count);
            Assert.Equal(58, DistanceMetric.calculateLeeDistance(search.Data, best.Data));
            Assert.Equal(58, result.Values.ElementAt(0));
            Assert.Equal(4, result.Keys.ElementAt(0).Id);
            Assert.Equal(best.Data, result.Keys.ElementAt(0).Data);
        }

        [Fact]
        public void BKTree_should_QueryBestMatchesBelowGivenThreshold()
        {
            BKTree<TestNode> tree = new BKTree<TestNode>();

            TestNode search = new TestNode(new int[] { 399, 400, 400 });

            TestNode best1 = new TestNode(41, new int[] { 400, 400, 400 });
            TestNode best2 = new TestNode(42, new int[] { 403, 403, 403 });
            TestNode best3 = new TestNode(43, new int[] { 406, 406, 406 });

            tree.add(new TestNode(1, new int[] { 100, 100, 100 }));
            tree.add(new TestNode(2, new int[] { 200, 200, 200 }));
            tree.add(new TestNode(3, new int[] { 300, 300, 300 }));
            tree.add(best1);
            tree.add(best2);
            tree.add(new TestNode(5, new int[] { 500, 500, 500 }));

            Dictionary<TestNode, Int32> results;

            // Query for match within distance of 1 (best1 is only expected result)
            results = tree.query(search, 1);

            Assert.Equal(1, results.Count);
            Assert.Equal(1, DistanceMetric.calculateLeeDistance(search.Data, best1.Data));
            Assert.Equal(1, results.Values.ElementAt(0));
            Assert.Equal(41, results.Keys.ElementAt(0).Id);
            Assert.Equal(best1.Data, results.Keys.ElementAt(0).Data);

            // Query for match within distance of 10 (best1 & best2 are expected results)
            tree.add(best3); // exercise adding another node after already queried
            results = tree.query(search, 10);

            Assert.Equal(2, results.Count);
            Assert.Equal(1, DistanceMetric.calculateLeeDistance(search.Data, best1.Data));
            Assert.Equal(10, DistanceMetric.calculateLeeDistance(search.Data, best2.Data));
            Assert.True(results.Contains(new KeyValuePair<TestNode, int>(best1, 1)));
            Assert.True(results.Contains(new KeyValuePair<TestNode, int>(best2, 10)));

            // Query for matches within distance of 20 (best1, best2 & best3 are expected results)
            results = tree.query(search, 20);

            Assert.Equal(3, results.Count);
            Assert.Equal(1, DistanceMetric.calculateLeeDistance(search.Data, best1.Data));
            Assert.Equal(10, DistanceMetric.calculateLeeDistance(search.Data, best2.Data));
            Assert.Equal(19, DistanceMetric.calculateLeeDistance(search.Data, best3.Data));
            Assert.True(results.Contains(new KeyValuePair<TestNode, int>(best1, 1)));
            Assert.True(results.Contains(new KeyValuePair<TestNode, int>(best2, 10)));
            Assert.True(results.Contains(new KeyValuePair<TestNode, int>(best3, 19)));
        }

        [Fact]
        public void BKTree_should_ThrowUponAddingNullNode()
        {
            BKTree<TestNode> tree = new BKTree<TestNode>();

            tree.add(new TestNode(1, new int[] { 100, 200, 300 }));
            tree.add(new TestNode(2, new int[] { 110, 210, 310 }));
            tree.add(new TestNode(3, new int[] { 130, 230, 330 }));
            tree.add(new TestNode(4, new int[] { 140, 240, 340 }));

            Assert.ThrowsDelegate boom = 
                delegate
                {
                    tree.add(null);
                };

            Assert.Throws<NullReferenceException>(boom);
        }

        #endregion
    }
}
