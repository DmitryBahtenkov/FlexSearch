using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Core.Storage.BinaryStorage;
using NUnit.Framework;

namespace GreatSearchEngineTests.BinaryStorageTests
{
    public class PersistentTreeTest
    {
        		[Test]
		public void TestSavingEmptyTree ()
		{
			// Create a tree with random elements,
			// presist it in a memory stream and then reconstruct it
			//BasicConfigurator.Configure ();

			var stream = new MemoryStream ();

			// Create the first tree
			var nodeManager = new TreeDiskNodeManager<int, long> (
				new TreeIntSerializer(),
				new TreeLongSerializer(),
				new RecordStorage(
					new BlockStorage(
						stream, 
						4096, 
						48
					)
				)
			); 
			var tree = new Tree<int, long>(nodeManager);

			// Init new tree
			stream.Position = 0;
			var nodeManager2 = new TreeDiskNodeManager<int, long> (
				new TreeIntSerializer(),
				new TreeLongSerializer(),
				new RecordStorage(
					new BlockStorage(
						stream, 
						4096, 
						48
					)
				)
			); 
			var tree2 = new Tree<int, long>(nodeManager2);
			var result = (from i in tree2.LargerThanOrEqualTo (0) select i).ToList();
			Assert.IsEmpty (result);
		}

		[Test]
		public void TestSavingFullTree ()
		{
			// Create a tree with random elements,
			// presist it in a memory stream and then reconstruct it
			//BasicConfigurator.Configure ();

			var stream = new MemoryStream ();
			var tree = new Tree<int, long>(
				new TreeDiskNodeManager<int, long> (
					new TreeIntSerializer(),
					new TreeLongSerializer(),
					new RecordStorage(
						new BlockStorage(
							stream, 
							4096, 
							48
						)
					)
				), 
				true
			);

			// Generate 10000 elements
			var sampleData = new List<Tuple<int, long>>();
			var rnd = new Random ();
			for (var i = 0; i < 1000; i++)
			{
				sampleData.Add (new Tuple<int, long>(
					rnd.Next(Int32.MinValue, Int32.MaxValue), 
					(long)rnd.Next(Int32.MinValue, Int32.MaxValue)
				));
			}

			// Insert these elements into the tree
			foreach (var d in sampleData) {
				tree.Insert (d.Item1, d.Item2);

				// Now create new tree see if changes were saved to the underlying stream
				var tree2 = new Tree<int, long>(
					new TreeDiskNodeManager<int, long> (
						new TreeIntSerializer(),
						new TreeLongSerializer(),
						new RecordStorage(
							new BlockStorage(
								stream, 
								4096, 
								48
							)
						)
					), 
					true
				);
				var actual = (from i in tree2.LargerThanOrEqualTo (Int32.MinValue) select i).ToList();
				var expected = (from i in tree.LargerThanOrEqualTo(Int32.MinValue) select i).ToList();
				Assert.IsTrue (actual.SequenceEqual (expected));
			}

			// Test deletion of some lemenets
			for (var i = 0; i < sampleData.Count; i++) {
				var deleteAt = rnd.Next (0, sampleData.Count);
				var deleteKey = sampleData[deleteAt];
				sampleData.RemoveAt (deleteAt);
				tree.Delete (deleteKey.Item1, deleteKey.Item2);

				// Now create new tree see if changes were saved to the underlying stream
				var tree2 = new Tree<int, long>(
					new TreeDiskNodeManager<int, long> (
						new TreeIntSerializer(),
						new TreeLongSerializer(),
						new RecordStorage(
							new BlockStorage(
								stream, 
								4096, 
								48
							)
						)
					), 
					true
				);
				var actual   = (from ii in tree2.LargerThanOrEqualTo (Int32.MinValue) select ii.Item1).ToList();
				var expected = (from ii in tree.LargerThanOrEqualTo(Int32.MinValue) select ii.Item1).ToList();
				Assert.IsTrue (actual.SequenceEqual (expected));
			}
		}
    }

    public class TreeIntSerializer : ISerializer<int>
    {
	    public byte[] Serialize (int value)
	    {
		    return LittleEndianByteOrder.GetBytes (value);
	    }

	    public int Deserialize (byte[] buffer, int offset, int length)
	    {
		    if (length != 4) {
			    throw new ArgumentException ("Invalid length: " + length);
		    }
			
		    return BufferHelper.ReadBufferInt32 (buffer, offset);
	    }

	    public bool IsFixedSize {
		    get {
			    return true;
		    }
	    }

	    public int Length {
		    get {
			    return 4;
		    }
	    }
    }

    public class TreeLongSerializer : ISerializer<long>
    {
	    public byte[] Serialize (long value)
	    {
		    return LittleEndianByteOrder.GetBytes (value);
	    }

	    public long Deserialize (byte[] buffer, int offset, int length)
	    {
		    if (length != 8) {
			    throw new ArgumentException ("Invalid length: " + length);
		    }
			
		    return BufferHelper.ReadBufferInt64 (buffer, offset);
	    }

	    public bool IsFixedSize {
		    get {
			    return true;
		    }
	    }

	    public int Length {
		    get {
			    return 8;
		    }
	    }
    }
}