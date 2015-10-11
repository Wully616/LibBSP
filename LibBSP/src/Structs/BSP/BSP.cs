#if (UNITY_2_6 || UNITY_2_6_1 || UNITY_3_0 || UNITY_3_0_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5 || UNITY_4_0 || UNITY_4_0_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_5 || UNITY_4_6 || UNITY_5)
#define UNITY
#endif

using System;
using System.IO;
using System.Collections.Generic;
#if UNITY
using UnityEngine;
#endif

namespace LibBSP {
#if !UNITY
	using Vector3 = Vector3d;
#endif
	public enum MapType : int {
		Undefined = 0,
		Quake = 29,
		// TYPE_GOLDSRC = 30, // Uses same algorithm and structures as Quake
		Nightfire = 42,
		Vindictus = 346131372,
		STEF2 = 556942937,
		MOHAA = 892416069,
		// TYPE_MOHBT = 1095516506, // Similar enough to MOHAA to use the same structures and algorithm
		Doom = 1145132868, // "DWAD"
		Hexen = 1145132872, // "HWAD"
		STEF2Demo = 1263223129,
		FAKK = 1263223152,
		TacticalInterventionEncrypted = 1268885814,
		CoD2 = 1347633741, // Uses same algorithm and structures as COD1. Read differently.
		SiN = 1347633747, // The headers for SiN and Jedi Outcast are exactly the same
		Raven = 1347633748,
		CoD4 = 1347633759, // Uses same algorithm and structures as COD1. Read differently.
		Source17 = 1347633767,
		Source18 = 1347633768,
		Source19 = 1347633769,
		Source20 = 1347633770,
		Source21 = 1347633771,
		Source22 = 1347633772,
		Source23 = 1347633773,
		L4D2 = 1347633774,
		Quake2 = 1347633775,
		Source27 = 1347633777,
		Daikatana = 1347633778,
		SoF = 1347633782, // Uses the same header as Q3.
		Quake3 = 1347633783,
		// TYPE_RTCW = 1347633784, // Uses same algorithm and structures as Quake 3
		CoD = 1347633796,
		DMoMaM = 1347895914,
	}

	/// <summary>
	/// Holds data for any and all BSP formats. Any unused lumps in a given format
	/// will be left as null. Then it will be fed into a universal decompile method
	/// which should be able to perform its job based on what data is stored.
	/// </summary>
	public class BSP {

		private MapType _version;

		private BSPReader reader;

		// Map structures
		// Quake 1/GoldSrc
		private Entities _entities;
		private List<Plane> _planes;
		private Textures _textures;
		private List<UIVertex> _vertices;
		private List<Node> _nodes;
		private List<TexInfo> _texInfo;
		private List<Face> _faces;
		private List<Leaf> _leaves;
		private NumList _markSurfaces;
		private List<Edge> _edges;
		private NumList _surfEdges;
		private List<Model> _models;
		// public byte[] pvs;
		// Quake 2
		private List<Brush> _brushes;
		private List<BrushSide> _brushSides;
		private NumList _markBrushes;
		// Nightfire
		private Textures _materials;
		private NumList _indices;
		// Source
		private List<Face> _originalFaces;
		private NumList _texTable;
		private List<SourceTexData> _texDatas;
		private List<SourceDispInfo> _dispInfos;
		private SourceDispVertices _dispVerts;
		private NumList _displacementTriangles;
		// public SourceOverlays overlays;
		private List<SourceCubemap> _cubemaps;
		private GameLump _gameLump;
		private SourceStaticProps _staticProps;

		/// <summary>
		/// An XOr encryption key for encrypted map formats. Must be read and set.
		/// </summary>
		public byte[] key = new byte[0];

		/// <summary>
		/// The version of this BSP. DO NOT CHANGE THIS unless you want to force reading a BSP as a certain format.
		/// </summary>
		public MapType version {
			get {
				if (_version == MapType.Undefined) {
					_version = reader.GetVersion();
				}
				return _version;
			}
			set {
				_version = value;
			}
		}

		public bool bigEndian { get { return reader.bigEndian; } }

		public Entities entities {
			get {
				if (_entities == null) {
					_entities = Entity.LumpFactory(reader.ReadLumpNum(Entity.GetIndexForLump(version), version), version);
				}
				return _entities;
			}
		}

		public List<Plane> planes {
			get {
				if (_planes == null) {
					_planes = PlaneExtensions.LumpFactory(reader.ReadLumpNum(PlaneExtensions.GetIndexForLump(version), version), version);
				}
				return _planes;
			}
		}

		public Textures textures {
			get {
				if (_textures == null) {
					_textures = Texture.LumpFactory(reader.ReadLumpNum(Texture.GetIndexForLump(version), version), version);
				}
				return _textures;
			}
		}

		public List<UIVertex> vertices {
			get {
				if (_vertices == null) {
					_vertices = UIVertexExtensions.LumpFactory(reader.ReadLumpNum(UIVertexExtensions.GetIndexForLump(version), version), version);
				}
				return _vertices;
			}
		}

		public List<Node> nodes {
			get {
				if (_nodes == null) {
					_nodes = Node.LumpFactory(reader.ReadLumpNum(Node.GetIndexForLump(version), version), version);
				}
				return _nodes;
			}
		}

		public List<TexInfo> texInfo {
			get {
				if (_texInfo == null) {
					_texInfo = TexInfo.LumpFactory(reader.ReadLumpNum(TexInfo.GetIndexForLump(version), version), version);
				}
				return _texInfo;
			}
		}

		public List<Face> faces {
			get {
				if (_faces == null) {
					_faces = Face.LumpFactory(reader.ReadLumpNum(Face.GetIndexForLump(version), version), version);
				}
				return _faces;
			}
		}

		public List<Leaf> leaves {
			get {
				if (_leaves == null) {
					_leaves = Leaf.LumpFactory(reader.ReadLumpNum(Leaf.GetIndexForLump(version), version), version);
				}
				return _leaves;
			}
		}

		public List<Edge> edges {
			get {
				if (_edges == null) {
					_edges = Edge.LumpFactory(reader.ReadLumpNum(Edge.GetIndexForLump(version), version), version);
				}
				return _edges;
			}
		}

		public List<Model> models {
			get {
				if (_models == null) {
					_models = Model.LumpFactory(reader.ReadLumpNum(Model.GetIndexForLump(version), version), version);
				}
				return _models;
			}
		}

		public List<Brush> brushes {
			get {
				if (_brushes == null) {
					_brushes = Brush.LumpFactory(reader.ReadLumpNum(Brush.GetIndexForLump(version), version), version);
				}
				return _brushes;
			}
		}

		public List<BrushSide> brushSides {
			get {
				if (_brushSides == null) {
					_brushSides = BrushSide.LumpFactory(reader.ReadLumpNum(BrushSide.GetIndexForLump(version), version), version);
				}
				return _brushSides;
			}
		}

		public Textures materials {
			get {
				if (_materials == null) {
					_materials = Texture.LumpFactory(reader.ReadLumpNum(Texture.GetIndexForMaterialLump(version), version), version);
				}
				return _materials;
			}
		}

		public List<Face> originalFaces {
			get {
				if (_originalFaces == null) {
					_originalFaces = Face.LumpFactory(reader.ReadLumpNum(Face.GetIndexForOriginalFacesLump(version), version), version);
				}
				return _originalFaces;
			}
		}

		public List<SourceTexData> texDatas {
			get {
				if (_texDatas == null) {
					_texDatas = SourceTexData.LumpFactory(reader.ReadLumpNum(SourceTexData.GetIndexForLump(version), version), version);
				}
				return _texDatas;
			}
		}

		public List<SourceDispInfo> dispInfos {
			get {
				if (_dispInfos == null) {
					_dispInfos = SourceDispInfo.LumpFactory(reader.ReadLumpNum(SourceDispInfo.GetIndexForLump(version), version), version);
				}
				return _dispInfos;
			}
		}

		public SourceDispVertices dispVerts {
			get {
				if (_dispVerts == null) {
					_dispVerts = SourceDispVertex.LumpFactory(reader.ReadLumpNum(SourceDispVertex.GetIndexForLump(version), version), version);
				}
				return _dispVerts;
			}
		}

		public List<SourceCubemap> cubemaps {
			get {
				if (_cubemaps == null) {
					_cubemaps = SourceCubemap.LumpFactory(reader.ReadLumpNum(SourceCubemap.GetIndexForLump(version), version), version);
				}
				return _cubemaps;
			}
		}

		public NumList markSurfaces {
			get {
				if (_markSurfaces == null) {
					NumList.DataType type;
					_markSurfaces = NumList.LumpFactory(reader.ReadLumpNum(NumList.GetIndexForMarkSurfacesLump(version, out type), version), type);
				}
				return _markSurfaces;
			}
		}

		public NumList surfEdges {
			get {
				if (_surfEdges == null) {
					NumList.DataType type;
					_surfEdges = NumList.LumpFactory(reader.ReadLumpNum(NumList.GetIndexForSurfEdgesLump(version, out type), version), type);
				}
				return _surfEdges;
			}
		}

		public NumList markBrushes {
			get {
				if (_markBrushes == null) {
					NumList.DataType type;
					_markBrushes = NumList.LumpFactory(reader.ReadLumpNum(NumList.GetIndexForMarkBrushesLump(version, out type), version), type);
				}
				return _markBrushes;
			}
		}

		public NumList indices {
			get {
				if (_indices == null) {
					NumList.DataType type;
					_indices = NumList.LumpFactory(reader.ReadLumpNum(NumList.GetIndexForIndicesLump(version, out type), version), type);
				}
				return _indices;
			}
		}

		public NumList texTable {
			get {
				if (_texTable == null) {
					NumList.DataType type;
					_texTable = NumList.LumpFactory(reader.ReadLumpNum(NumList.GetIndexForTexTableLump(version, out type), version), type);
				}
				return _texTable;
			}
		}

		public NumList displacementTriangles {
			get {
				if (_displacementTriangles == null) {
					NumList.DataType type;
					_displacementTriangles = NumList.LumpFactory(reader.ReadLumpNum(NumList.GetIndexForDisplacementTrianglesLump(version, out type), version), type);
				}
				return _displacementTriangles;
			}
		}

		public GameLump gameLump {
			get {
				if (_gameLump == null) {
					_gameLump = GameLump.LumpFactory(reader.ReadLumpNum(GameLump.GetIndexForLump(version), version), version);
				}
				return _gameLump;
			}
		}

		public SourceStaticProps staticProps {
			get {
				if (_staticProps == null) {
					GameLump.GameLumpInfo info = gameLump[GameLumpType.sprp];
					byte[] thisLump = new byte[info.length];
					Array.Copy(gameLump.rawData, info.offset - gameLump.gameLumpOffset, thisLump, 0, info.length);
					_staticProps = SourceStaticProp.LumpFactory(thisLump, version, info.version);
				}
				return _staticProps;
			}
		}

		/// <summary>
		/// Gets the path to this BSP file.
		/// </summary>
		public string filePath { get; private set; }

		/// <summary>
		/// Gets the file name of this map.
		/// </summary>
		public string MapName {
			get {
				int i;
				for (i = 0; i < filePath.Length; ++i) {
					if (filePath[filePath.Length - 1 - i] == '\\') {
						break;
					}
					if (filePath[filePath.Length - 1 - i] == '/') {
						break;
					}
				}
				return filePath.Substring(filePath.Length - i, (filePath.Length) - (filePath.Length - i));
			}
		}

		/// <summary>
		/// Gets the file name of this map without the ".BSP" extension.
		/// </summary>
		public string MapNameNoExtension {
			get {
				string name = MapName;
				int i;
				for (i = 0; i < name.Length; ++i) {
					if (name[name.Length - 1 - i] == '.') {
						break;
					}
				}
				return name.Substring(0, (name.Length - 1 - i) - (0));
			}
		}

		/// <summary>
		/// Gets the folder path where this map is located.
		/// </summary>
		public string Folder {
			get {
				int i;
				for (i = 0; i < filePath.Length; ++i) {
					if (filePath[filePath.Length - 1 - i] == '\\') {
						break;
					}
					if (filePath[filePath.Length - 1 - i] == '/') {
						break;
					}
				}
				return filePath.Substring(0, (filePath.Length - i) - (0));
			}
		}

		/// <summary>
		/// Creates a new <c>BSP</c> instance pointing to the file at <paramref name="filePath"/>. The
		/// <c>List</c>s in this class will be read and populated when accessed through their properties.
		/// </summary>
		/// <param name="filePath">The path to the .BSP file</param>
		public BSP(string filePath) {
			reader = new BSPReader(new FileInfo(filePath));
			this.filePath = filePath;
		}

		/// <summary>
		/// Creates a new <c>BSP</c> instance using the file referenced by <paramref name="file"/>. The
		/// <c>List</c>s in this class will be read and populated when accessed through their properties.
		/// </summary>
		/// <param name="file">A reference to the .BSP file</param>
		public BSP(FileInfo file) {
			reader = new BSPReader(file);
			this.filePath = file.FullName;
		}
	}
}
