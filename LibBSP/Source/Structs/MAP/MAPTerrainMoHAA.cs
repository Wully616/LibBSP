#if (UNITY_2_6 || UNITY_2_6_1 || UNITY_3_0 || UNITY_3_0_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5 || UNITY_4_0 || UNITY_4_0_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_5 || UNITY_4_6 || UNITY_5 || UNITY_5_3_OR_NEWER)
#define UNITY
#endif
using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

namespace LibBSP {
#if !UNITY
	using Vector2 = Vector2d;
	using Vector3 = Vector3d;
#endif
	/// <summary>
	/// Class containing all data necessary to render a Terrain from MoHAA.
	/// </summary>
	[Serializable] public class MAPTerrainMoHAA {

		private static IFormatProvider _format = CultureInfo.CreateSpecificCulture("en-US");

		public Vector2 size;
		public int flags;
		public Vector3 origin;
		public List<Partition> partitions;
		public List<Vertex> vertices;

		/// <summary>
		/// Creates a new empty <see cref="MAPTerrainMoHAA"/> object. Internal data will have to be set manually.
		/// </summary>
		public MAPTerrainMoHAA() {
			partitions = new List<Partition>(4);
			vertices = new List<Vertex>(81);
		}
		
		/// <summary>
		/// Constructs a new <see cref="MAPTerrainMoHAA"/> object using the supplied string array as data.
		/// </summary>
		/// <param name="lines">Data to parse.</param>
		public MAPTerrainMoHAA(string[] lines) {
			// TODO: Constructor to parse text
		}

		[Serializable] public class Partition {
			public int unknown1;
			public int unknown2;
			public string shader;
			public int[] textureShift;
			public double rotation;
			public int unknown3;
			public double[] textureScale;
			public int unknown4;
			public int flags;
			public int unknown5;
			public string properties;

			public Partition() {
				unknown1 = 0;
				unknown2 = 0;
				shader = "";
				textureShift = new int[2];
				rotation = 0;
				unknown3 = 0;
				textureScale = new double[] { 1, 1 };
				unknown4 = 0;
				flags = 0;
				unknown5 = 0;
				properties = "";
			}
		}

		[Serializable] public class Vertex {
			public int height;
			public string unknown1;
			public string unknown2;

			public Vertex() {
				height = 0;
				unknown1 = "";
				unknown2 = "";
			}
		}
	}
}