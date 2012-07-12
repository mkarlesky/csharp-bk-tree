using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BKTree;

namespace ExampleApplication
{
    class Program
    {
        public class ExampleNodeRecord : BKTreeNode
        {
            public ExampleNodeRecord(ushort id, int[] values)
                : base()
            {
                if (id == 0)
                {
                    throw new ArgumentException("0 is a reserved Id value");
                }

                Data = values;
                Id = id;
            }

            public ExampleNodeRecord(int[] values)
                : base()
            {
                Data = values;
                Id = 0;
            }

            public ushort Id { get; private set; }
            public int[] Data { get; private set; } // String of symbols

            // The only required method of abstract class BKTreeNode
            override protected int calculateDistance(BKTreeNode node)
            {
                return DistanceMetric.calculateLeeDistance(
                    this.Data,
                    ((ExampleNodeRecord)node).Data);
            }
        }

        /*
         * To use BKTree:
         * 1. Create a class dervied from BKTreeNode
         * 2. Add a member variable of your data to be sorted / retrieved
         * 3. Override the calculateDistance method to calculate the distance metric 
         *    between two nodes for the data to be sorted / retrieved.
         * 4. Instantiate a BKTree with the type name of the class created in (1).
         */

        static void Main(string[] args)
        {
            /*
             * NOTE: More comprehensive examples of BK-Tree methods in unit tests
             */

            // Exercise static distance metric methods -- just because
            Console.WriteLine(
                DistanceMetric.calculateHammingDistance(
                    new byte[] { 0xEF, 0x35, 0x20 },
                    new byte[] { 0xAD, 0x13, 0x87 }));

            Console.WriteLine(
                DistanceMetric.calculateLeeDistance(
                    new int[] { 196, 105, 48 },
                    new int[] { 201, 12, 51 }));

            Console.WriteLine(
                DistanceMetric.calculateLevenshteinDistance(
                    "kitten",
                    "sitting"));


            // Create BKTree with derived node class from top of file
            BKTree<ExampleNodeRecord> tree = new BKTree<ExampleNodeRecord>();

            // Add some nodes
            tree.add( new ExampleNodeRecord( 1, new int[] {100,200,300}) );
            tree.add( new ExampleNodeRecord( 2, new int[] {110,210,310}) );
            tree.add( new ExampleNodeRecord( 3, new int[] {120,220,320}) );
            tree.add( new ExampleNodeRecord( 4, new int[] {130,230,330}) );
            tree.add( new ExampleNodeRecord( 5, new int[] {140,240,340}) );

            // Get best node from our tree with best distance
            Dictionary<ExampleNodeRecord, Int32> results = 
                tree.findBestNodeWithDistance(
                    new ExampleNodeRecord( new int[] { 103, 215, 303 }) );

            // Get best nodes below threshold
            results = tree.query(
                new ExampleNodeRecord(new int[] { 103, 215, 303 }),
                10 ); // arbitrary threshold
        
            // Dictionaries don't print well; so invent your own handy print routine
        }
    }
}
