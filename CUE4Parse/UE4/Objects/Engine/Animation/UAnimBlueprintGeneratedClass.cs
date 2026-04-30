using System;
using System.Collections.Generic;
using System.Linq;
using CUE4Parse.UE4.Assets.Exports;
using CUE4Parse.UE4.Assets.Exports.Animation;
using CUE4Parse.UE4.Assets.Objects;
using CUE4Parse.UE4.Assets.Objects.Properties;
using CUE4Parse.UE4.Assets.Readers;
using CUE4Parse.UE4.Assets.Utils;
using CUE4Parse.UE4.Objects.Core.Misc;
using CUE4Parse.UE4.Objects.UObject;
using Newtonsoft.Json;
using UObjectExport = CUE4Parse.UE4.Assets.Exports.UObject;

namespace CUE4Parse.UE4.Objects.Engine.Animation;

public class UAnimBlueprintGeneratedClass : UBlueprintGeneratedClass
{
	public FStructFallback[] BakedStateMachines = [];
	public FPackageIndex TargetSkeleton = new();
	public FAnimNotifyEvent[] AnimNotifies = [];
	public FName[] SyncGroupNames = [];
	public UScriptMap? OrderedSavedPoseIndicesMap;
	public UScriptMap? GraphAssetPlayerInformation;
	public UScriptMap? GraphBlendOptions;
	public FStructFallback[] AnimNodeData = [];
	public UScriptMap? NodeTypeMap;

	[JsonIgnore] private FAnimNodePropertyData[]? _animNodePropertyData;
	[JsonIgnore] private FAnimNodeData[]? _resolvedAnimNodeData;
	[JsonIgnore] private FAnimBlueprintFunction[]? _animBlueprintFunctions;
	[JsonIgnore] private Dictionary<string, int>? _orderedSavedPoseIndices;
	[JsonIgnore] private Dictionary<string, FStructFallback>? _orderedSavedPoseIndexData;
	[JsonIgnore] private Dictionary<string, FStructFallback>? _graphAssetPlayerInformationData;
	[JsonIgnore] private Dictionary<string, FStructFallback>? _graphBlendOptionsData;
	[JsonIgnore] private Dictionary<string, FAnimNodeStructData>? _nodeTypeData;
	[JsonIgnore] private Dictionary<string, FStructFallback>? _nodeTypeFallbackData;
	[JsonIgnore] private Dictionary<string, FAnimNodeStructData>? _nodeTypeDataAliases;
	[JsonIgnore] private Dictionary<string, FStructFallback>? _nodeTypeFallbackAliases;
	[JsonIgnore] private FPropertyTag[]? _constantNodeValueProperties;
	[JsonIgnore] private FPropertyTag[]? _mutableNodeValueProperties;
	[JsonIgnore] private FStructProperty? _mutableNodeDataProperty;

	[JsonIgnore]
	public FAnimNodePropertyData[] AnimNodePropertyData => _animNodePropertyData ??= BuildAnimNodePropertyData();

	[JsonIgnore]
	public FAnimNodeData[] ResolvedAnimNodeData => _resolvedAnimNodeData ??= BuildAnimNodeData();

	[JsonIgnore]
	public FStructProperty[] AnimNodeProperties => [.. AnimNodePropertyData.Select(data => data.Property)];

	[JsonIgnore]
	public FStructProperty[] LinkedAnimGraphNodeProperties =>
		[.. AnimNodePropertyData.Where(data => data.IsLinkedAnimGraphNode).Select(data => data.Property)];

	[JsonIgnore]
	public FStructProperty[] LinkedAnimLayerNodeProperties =>
		[.. AnimNodePropertyData.Where(data => data.IsLinkedAnimLayerNode).Select(data => data.Property)];

	[JsonIgnore]
	public FStructProperty[] StateMachineNodeProperties =>
		[.. AnimNodePropertyData.Where(data => data.IsStateMachineNode).Select(data => data.Property)];

	[JsonIgnore]
	public FAnimBlueprintFunction[] AnimBlueprintFunctions => _animBlueprintFunctions ??= GenerateAnimationBlueprintFunctions();

	[JsonIgnore]
	public IReadOnlyDictionary<string, int> OrderedSavedPoseNodeIndices =>
		_orderedSavedPoseIndices ??= BuildOrderedSavedPoseNodeIndices();

	[JsonIgnore]
	public IReadOnlyDictionary<string, FStructFallback> OrderedSavedPoseIndexData =>
		_orderedSavedPoseIndexData ??= BuildStructFallbackMap(OrderedSavedPoseIndicesMap);

	[JsonIgnore]
	public IReadOnlyDictionary<string, FStructFallback> GraphAssetPlayerInformationData =>
		_graphAssetPlayerInformationData ??= BuildStructFallbackMap(GraphAssetPlayerInformation);

	[JsonIgnore]
	public IReadOnlyDictionary<string, FStructFallback> GraphBlendOptionsData =>
		_graphBlendOptionsData ??= BuildStructFallbackMap(GraphBlendOptions);

	[JsonIgnore]
	public IReadOnlyDictionary<string, FAnimNodeStructData> NodeTypeData =>
		_nodeTypeData ??= BuildNodeTypeData();

	[JsonIgnore]
	public IReadOnlyDictionary<string, FStructFallback> NodeTypeFallbackData =>
		_nodeTypeFallbackData ??= BuildStructFallbackMap(NodeTypeMap);

	[JsonIgnore]
	private IReadOnlyDictionary<string, FAnimNodeStructData> NodeTypeDataAliases =>
		_nodeTypeDataAliases ??= BuildAliasMap(NodeTypeData);

	[JsonIgnore]
	private IReadOnlyDictionary<string, FStructFallback> NodeTypeFallbackAliases =>
		_nodeTypeFallbackAliases ??= BuildAliasMap(NodeTypeFallbackData);

	[JsonIgnore]
	public FStructProperty[] PreUpdateNodeProperties { get; private set; } = [];

	[JsonIgnore]
	public FStructProperty[] DynamicResetNodeProperties { get; private set; } = [];

	[JsonIgnore]
	public FStructProperty[] InitializationNodeProperties { get; private set; } = [];

	[JsonIgnore]
	public int RootAnimNodeIndex => ResolveRootAnimNodeIndex();

	[JsonIgnore]
	public FStructProperty? RootAnimNodeProperty =>
		RootAnimNodeIndex >= 0 && RootAnimNodeIndex < AnimNodeProperties.Length ? AnimNodeProperties[RootAnimNodeIndex] : null;

	[JsonIgnore]
	public IReadOnlyList<FPropertyTag> ConstantNodeValueProperties =>
		_constantNodeValueProperties ??= BuildConstantNodeValueProperties();

	[JsonIgnore]
	public IReadOnlyList<FPropertyTag> MutableNodeValueProperties =>
		_mutableNodeValueProperties ??= BuildMutableNodeValueProperties();

	[JsonIgnore]
	public FStructProperty? MutableNodeDataProperty => _mutableNodeDataProperty ??= ResolveMutableNodeDataProperty();

	public override void Deserialize(FAssetArchive Ar, long validPos)
	{
		base.Deserialize(Ar, validPos);

		BakedStateMachines = GetOrDefault(nameof(BakedStateMachines), Array.Empty<FStructFallback>());
		TargetSkeleton = GetOrDefault(nameof(TargetSkeleton), TargetSkeleton);
		AnimNotifies = GetOrDefault(nameof(AnimNotifies), Array.Empty<FAnimNotifyEvent>());
		SyncGroupNames = GetOrDefault(nameof(SyncGroupNames), Array.Empty<FName>());
		OrderedSavedPoseIndicesMap = GetOrDefault<UScriptMap?>(nameof(OrderedSavedPoseIndicesMap));
		GraphAssetPlayerInformation = GetOrDefault<UScriptMap?>(nameof(GraphAssetPlayerInformation));
		GraphBlendOptions = GetOrDefault<UScriptMap?>(nameof(GraphBlendOptions));
		AnimNodeData = GetOrDefault(nameof(AnimNodeData), Array.Empty<FStructFallback>());
		NodeTypeMap = GetOrDefault<UScriptMap?>(nameof(NodeTypeMap));

		InvalidateCaches();
	}

	public bool TryGetAnimBlueprintFunction(string functionName, out FAnimBlueprintFunction function)
	{
		var match = AnimBlueprintFunctions.FirstOrDefault(candidate =>
			candidate.Name.Equals(functionName, StringComparison.OrdinalIgnoreCase));
		if (match is null)
		{
			function = null!;
			return false;
		}

		function = match;
		return true;
	}

	public bool TryGetAnimNodePropertyData(string propertyName, out FAnimNodePropertyData propertyData)
	{
		var match = AnimNodePropertyData.FirstOrDefault(candidate =>
			candidate.Name.Equals(propertyName, StringComparison.Ordinal));
		if (match is null)
		{
			propertyData = null!;
			return false;
		}

		propertyData = match;
		return true;
	}

	public bool TryGetAnimNodeData(int animNodePropertyIndex, out FAnimNodeData nodeData)
	{
		if (animNodePropertyIndex >= 0 && animNodePropertyIndex < ResolvedAnimNodeData.Length)
		{
			nodeData = ResolvedAnimNodeData[animNodePropertyIndex];
			return true;
		}

		nodeData = null!;
		return false;
	}

	public bool TryGetAnimNodeData(string propertyName, out FAnimNodeData nodeData)
	{
		var match = ResolvedAnimNodeData.FirstOrDefault(candidate =>
			candidate.PropertyName.Equals(propertyName, StringComparison.Ordinal));
		if (match is null)
		{
			nodeData = null!;
			return false;
		}

		nodeData = match;
		return true;
	}

	public bool TryGetNodeTypeData(string nodeTypeName, out FAnimNodeStructData nodeTypeData)
	{
		foreach (var lookupName in GetNodeTypeLookupNames(nodeTypeName))
		{
			if (NodeTypeDataAliases.TryGetValue(lookupName, out nodeTypeData!))
				return true;
		}

		nodeTypeData = null!;
		return false;
	}

	public bool TryGetNodeTypeFallbackData(string nodeTypeName, out FStructFallback rawData)
	{
		foreach (var lookupName in GetNodeTypeLookupNames(nodeTypeName))
		{
			if (NodeTypeFallbackAliases.TryGetValue(lookupName, out rawData!))
				return true;
		}

		rawData = null!;
		return false;
	}

	public int GetAnimNodePropertyIndex(string nodeTypeName, string propertyName)
	{
		if (TryGetNodeTypeData(nodeTypeName, out var nodeTypeData))
			return nodeTypeData.GetPropertyIndex(propertyName);

		if (!TryLoadAnimNodeStruct(nodeTypeName, out var nodeStruct) || nodeStruct.ChildProperties is not { Length: > 0 } childProperties)
			return -1;

		for (var propertyIndex = 0; propertyIndex < childProperties.Length; propertyIndex++)
		{
			if (childProperties[propertyIndex].Name.Text.Equals(propertyName, StringComparison.Ordinal))
				return propertyIndex;
		}

		return -1;
	}

	public int GetAnimNodePropertyCount(string nodeTypeName)
	{
		if (TryGetNodeTypeData(nodeTypeName, out var nodeTypeData))
			return nodeTypeData.GetNumProperties();

		return TryLoadAnimNodeStruct(nodeTypeName, out var nodeStruct) ? nodeStruct.ChildProperties?.Length ?? 0 : 0;
	}

	public int GetSyncGroupIndex(FName syncGroupName) => GetSyncGroupIndex(syncGroupName.Text);

	public int GetSyncGroupIndex(string syncGroupName)
	{
		for (var index = 0; index < SyncGroupNames.Length; index++)
		{
			if (SyncGroupNames[index].Text.Equals(syncGroupName, StringComparison.OrdinalIgnoreCase))
				return index;
		}

		return -1;
	}

	public bool TryGetConstantNodeValueRaw(int entryIndex, out FPropertyTag propertyTag)
	{
		var properties = ConstantNodeValueProperties;
		if (entryIndex >= 0 && entryIndex < properties.Count)
		{
			propertyTag = properties[entryIndex];
			return true;
		}

		propertyTag = null!;
		return false;
	}

	public bool TryGetMutableNodeValueRaw(int entryIndex, out FPropertyTag propertyTag)
	{
		var properties = MutableNodeValueProperties;
		if (entryIndex >= 0 && entryIndex < properties.Count)
		{
			propertyTag = properties[entryIndex];
			return true;
		}

		propertyTag = null!;
		return false;
	}

	public bool TryGetNodeValueRaw(FAnimNodeData nodeData, int propertyIndex, out FPropertyTag propertyTag)
	{
		if (nodeData.IsInstanceDataEntry(propertyIndex, out var instanceEntryIndex) &&
			TryGetMutableNodeValueRaw(instanceEntryIndex, out propertyTag))
			return true;

		if (nodeData.IsConstantDataEntry(propertyIndex, out var constantEntryIndex) &&
			TryGetConstantNodeValueRaw(constantEntryIndex, out propertyTag))
			return true;

		propertyTag = null!;
		return false;
	}

	public bool TryGetNodeValueRaw(FAnimNodeData nodeData, string propertyName, out FPropertyTag propertyTag)
	{
		var propertyIndex = GetAnimNodePropertyIndex(nodeData.StructTypeName, propertyName);
		if (propertyIndex < 0)
		{
			propertyTag = null!;
			return false;
		}

		return TryGetNodeValueRaw(nodeData, propertyIndex, out propertyTag);
	}

	public bool TryGetNodeValue<T>(FAnimNodeData nodeData, int propertyIndex, out T value)
	{
		if (TryGetNodeValueRaw(nodeData, propertyIndex, out var propertyTag) && propertyTag.Tag?.GetValue(typeof(T)) is T typedValue)
		{
			value = typedValue;
			return true;
		}

		value = default!;
		return false;
	}

	public bool TryGetNodeValue<T>(FAnimNodeData nodeData, string propertyName, out T value)
	{
		if (TryGetNodeValueRaw(nodeData, propertyName, out var propertyTag) && propertyTag.Tag?.GetValue(typeof(T)) is T typedValue)
		{
			value = typedValue;
			return true;
		}

		value = default!;
		return false;
	}

	public bool TryGetRootNodeIndexForFunction(string functionName, out int outputPoseNodeIndex)
	{
		outputPoseNodeIndex = -1;
		return TryGetAnimBlueprintFunction(functionName, out var function) &&
			   function.OutputPoseNodeIndex >= 0 &&
			   (outputPoseNodeIndex = function.OutputPoseNodeIndex) >= 0;
	}

	public bool TryGetRootNodePropertyForFunction(string functionName, out FAnimNodePropertyData? propertyData)
	{
		propertyData = null;
		if (!TryGetRootNodeIndexForFunction(functionName, out var outputPoseNodeIndex))
			return false;

		propertyData = AnimNodePropertyData.FirstOrDefault(candidate => candidate.AnimNodePropertyIndex == outputPoseNodeIndex);
		return propertyData is not null;
	}

	public int GetNodeIndexFromGuid(FGuid guid)
	{
		for (var index = 0; index < AnimNodePropertyData.Length; index++)
		{
			if (TryGetNodeGuid(AnimNodePropertyData[index], out var nodeGuid) && nodeGuid == guid)
				return index;
		}

		return -1;
	}

	private void InvalidateCaches()
	{
		_animNodePropertyData = null;
		_resolvedAnimNodeData = null;
		_animBlueprintFunctions = null;
		_orderedSavedPoseIndices = null;
		_orderedSavedPoseIndexData = null;
		_graphAssetPlayerInformationData = null;
		_graphBlendOptionsData = null;
		_nodeTypeData = null;
		_nodeTypeFallbackData = null;
		_nodeTypeDataAliases = null;
		_nodeTypeFallbackAliases = null;
		_constantNodeValueProperties = null;
		_mutableNodeValueProperties = null;
		_mutableNodeDataProperty = null;
		PreUpdateNodeProperties = [];
		DynamicResetNodeProperties = [];
		InitializationNodeProperties = [];
	}

	private FAnimNodePropertyData[] BuildAnimNodePropertyData()
	{
		var result = new List<FAnimNodePropertyData>();
		var childProperties = ChildProperties ?? [];
		for (var childPropertyIndex = 0; childPropertyIndex < childProperties.Length; childPropertyIndex++)
		{
			if (childProperties[childPropertyIndex] is not FStructProperty structProperty)
				continue;

			if (!IsAnimNodeStruct(structProperty))
				continue;

			result.Add(new FAnimNodePropertyData(structProperty, result.Count, childPropertyIndex,
				ResolveDefaultValue(structProperty.Name.Text)));
		}

		return [.. result];
	}

	private FAnimNodeData[] BuildAnimNodeData()
	{
		var propertyData = AnimNodePropertyData;
		var count = Math.Max(propertyData.Length, AnimNodeData.Length);
		var result = new List<FAnimNodeData>(count);
		for (var index = 0; index < count; index++)
		{
			var property = index < propertyData.Length ? propertyData[index] : null;
			var rawData = index < AnimNodeData.Length ? AnimNodeData[index] : null;
			result.Add(new FAnimNodeData(index, property, rawData));
		}

		return [.. result];
	}

	private Dictionary<string, FAnimNodeStructData> BuildNodeTypeData()
	{
		var result = new Dictionary<string, FAnimNodeStructData>(StringComparer.OrdinalIgnoreCase);
		foreach (var (nodeTypeName, rawData) in NodeTypeFallbackData)
		{
			if (string.IsNullOrEmpty(nodeTypeName))
				continue;

			result[nodeTypeName] = new FAnimNodeStructData(nodeTypeName, rawData, TryLoadAnimNodeStruct(nodeTypeName, out var nodeStruct) ? nodeStruct : null);
		}

		foreach (var property in AnimNodePropertyData)
		{
			if (string.IsNullOrEmpty(property.StructName) || result.ContainsKey(property.StructName))
				continue;

			result[property.StructName] = new FAnimNodeStructData(property.StructName, null,
				property.Property.Struct.TryLoad<UStruct>(out var nodeStruct) ? nodeStruct : null);
		}

		return result;
	}

	private FPropertyTag[] BuildConstantNodeValueProperties()
	{
		if (!TryLoadClassDefaultObject(out var defaultObject))
			return [];

		return BuildFlattenedPropertyTagTable(defaultObject?.SerializedSparseClassData, defaultObject?.SerializedSparseClassDataStruct);
	}

	private FPropertyTag[] BuildMutableNodeValueProperties()
	{
		if (!TryLoadClassDefaultObject(out var defaultObject))
			return [];

		var mutableNodeProperty = MutableNodeDataProperty;
		if (defaultObject is null || mutableNodeProperty is null || !defaultObject.TryGetValue(out FStructFallback mutableNodeData, mutableNodeProperty.Name.Text))
			return [];

		return BuildFlattenedPropertyTagTable(mutableNodeData,
			mutableNodeProperty.Struct.TryLoad<UStruct>(out var mutableNodeStruct) ? mutableNodeStruct : null);
	}

	private FStructProperty? ResolveMutableNodeDataProperty()
	{
		foreach (var childProperty in ChildProperties ?? [])
		{
			if (childProperty is FStructProperty structProperty && IsStructOrDerivedFrom(structProperty, "AnimBlueprintMutableData"))
				return structProperty;
		}

		return null;
	}

	private static FPropertyTag[] BuildFlattenedPropertyTagTable(FStructFallback? structData, UStruct? structType)
	{
		if (structData?.Properties is not { Count: > 0 } serializedProperties)
			return [];

		if (structType?.ChildProperties is not { Length: > 0 } childProperties)
			return [.. serializedProperties.Where(static property => property.Tag is not null)];

		var result = new List<FPropertyTag>(serializedProperties.Count);
		var usedProperties = new HashSet<FPropertyTag>();

		foreach (var childProperty in childProperties)
		{
			var propertyTag = serializedProperties.FirstOrDefault(candidate =>
				candidate.Tag is not null &&
				candidate.ArrayIndex == 0 &&
				candidate.Name.Text.Equals(childProperty.Name.Text, StringComparison.Ordinal));
			if (propertyTag is null)
				continue;

			result.Add(propertyTag);
			usedProperties.Add(propertyTag);
		}

		foreach (var propertyTag in serializedProperties)
		{
			if (propertyTag.Tag is null || usedProperties.Contains(propertyTag))
				continue;

			result.Add(propertyTag);
		}

		return [.. result];
	}

	private FAnimBlueprintFunction[] GenerateAnimationBlueprintFunctions()
	{
		if (FuncMap is not { Count: > 0 })
			return [];

		var functions = new List<FAnimBlueprintFunction>(FuncMap.Count);
		foreach (var (name, packageIndex) in FuncMap)
		{
			if (!packageIndex.TryLoad<UFunction>(out var function))
				continue;

			if (!TryCreateAnimBlueprintFunction(name, function, out var animBlueprintFunction))
				continue;

			functions.Add(animBlueprintFunction);
		}

		functions.Sort(static (left, right) =>
		{
			var leftIsAnimGraph = left.Name.Equals("AnimGraph", StringComparison.OrdinalIgnoreCase);
			var rightIsAnimGraph = right.Name.Equals("AnimGraph", StringComparison.OrdinalIgnoreCase);
			if (leftIsAnimGraph != rightIsAnimGraph)
				return leftIsAnimGraph ? -1 : 1;
			return StringComparer.Ordinal.Compare(left.Name, right.Name);
		});

		LinkFunctionsToDefaultObjectNodes(functions);

		return [.. functions];
	}

	private bool TryCreateAnimBlueprintFunction(FName functionName, UFunction function, out FAnimBlueprintFunction animBlueprintFunction)
	{
		animBlueprintFunction = null!;

		var inputPoseNames = new List<string>();
		FStructProperty? outputPoseProperty = null;
		foreach (var childProperty in function.ChildProperties ?? [])
		{
			if (childProperty is not FStructProperty structProperty || !IsPoseLinkStruct(structProperty))
				continue;

			var isOutParm = structProperty.PropertyFlags.HasFlag(EPropertyFlags.OutParm) &&
							!structProperty.PropertyFlags.HasFlag(EPropertyFlags.ReturnParm);
			if (isOutParm && outputPoseProperty is null)
			{
				outputPoseProperty = structProperty;
				continue;
			}

			if (structProperty.PropertyFlags.HasFlag(EPropertyFlags.Parm))
				inputPoseNames.Add(structProperty.Name.Text);
		}

		if (outputPoseProperty is null && inputPoseNames.Count == 0 && !functionName.Text.Equals("AnimGraph", StringComparison.OrdinalIgnoreCase))
			return false;

		var outputLinkId = -1;
		var outputNodeIndex = -1;
		if (outputPoseProperty is not null)
		{
			if (TryResolvePoseLinkForFunction(functionName.Text, outputPoseProperty.Name.Text, out var poseLink))
			{
				outputLinkId = poseLink.LinkID;
				outputNodeIndex = ResolveAnimNodePropertyIndexFromLinkId(outputLinkId, true);
			}
		}

		var inputNodeIndices = new int[inputPoseNames.Count];
		Array.Fill(inputNodeIndices, -1);

		animBlueprintFunction = new FAnimBlueprintFunction(functionName.Text, function, outputPoseProperty?.Name.Text,
			inputPoseNames.ToArray(), inputNodeIndices, outputLinkId, outputNodeIndex);
		return true;
	}

	private void LinkFunctionsToDefaultObjectNodes(List<FAnimBlueprintFunction> functions)
	{
		if (functions.Count == 0 || AnimNodePropertyData.Length == 0)
			return;

		foreach (var propertyData in AnimNodePropertyData)
		{
			var defaultValue = propertyData.DefaultValue;
			if (defaultValue is null && !TryGetAnimNodeData(propertyData.AnimNodePropertyIndex, out _))
				continue;

			if (propertyData.IsRootNode)
			{
				var rootNodeName = ResolveRootNodeFunctionName(propertyData, defaultValue);
				if (string.IsNullOrEmpty(rootNodeName))
					continue;

				var function = functions.FirstOrDefault(candidate =>
					candidate.Name.Equals(rootNodeName, StringComparison.OrdinalIgnoreCase));
				if (function is null)
					continue;

				function.OutputPoseNodeIndex = propertyData.AnimNodePropertyIndex;
				if (defaultValue is not null && TryGetNestedPoseLink(defaultValue, "Result", out var poseLink))
					function.OutputPoseLinkID = poseLink.LinkID;
			}
			else if (propertyData.IsLinkedAnimGraphNode || propertyData.IsLinkedAnimLayerNode)
			{
				if (defaultValue is null)
					continue;

				var graphName = ResolveTextValue(defaultValue, "Graph");
				var inputPoseName = ResolveTextValue(defaultValue, "Name");
				if (string.IsNullOrEmpty(graphName) || string.IsNullOrEmpty(inputPoseName))
					continue;

				var function = functions.FirstOrDefault(candidate =>
					candidate.Name.Equals(graphName, StringComparison.OrdinalIgnoreCase));
				if (function is null)
					continue;

				for (var inputIndex = 0; inputIndex < function.InputPoseNames.Length; inputIndex++)
				{
					if (function.InputPoseNames[inputIndex].Equals(inputPoseName, StringComparison.OrdinalIgnoreCase))
						function.InputPoseNodeIndices[inputIndex] = propertyData.AnimNodePropertyIndex;
				}
			}
		}
	}

	private string ResolveRootNodeFunctionName(FAnimNodePropertyData propertyData, FStructFallback? defaultValue)
	{
		if (TryGetAnimNodeData(propertyData.AnimNodePropertyIndex, out var nodeData) &&
			TryResolveNodeDataTextValue(nodeData, out var rootNodeName, "Name", "NodeName", "GraphName"))
		{
			return rootNodeName;
		}

		return defaultValue is not null ? ResolveTextValue(defaultValue, "Name", "NodeName", "GraphName") : string.Empty;
	}

	private bool TryResolveNodeDataTextValue(FAnimNodeData nodeData, out string value, params string[] propertyNames)
	{
		foreach (var propertyName in propertyNames)
		{
			if (nodeData.TryGetValue(this, propertyName, out FName textName))
			{
				value = textName.Text;
				if (!string.IsNullOrEmpty(value))
					return true;
			}

			if (nodeData.TryGetValue(this, propertyName, out string textValue) && !string.IsNullOrEmpty(textValue))
			{
				value = textValue;
				return true;
			}
		}

		value = string.Empty;
		return false;
	}

	private Dictionary<string, int> BuildOrderedSavedPoseNodeIndices()
	{
		var result = new Dictionary<string, int>(StringComparer.Ordinal);
		foreach (var (keyName, value) in OrderedSavedPoseIndexData)
		{
			if (TryExtractSavedPoseNodeIndex(value, out var savedPoseIndex))
				result[keyName] = savedPoseIndex;
		}

		return result;
	}

	private static bool TryExtractSavedPoseNodeIndex(FStructFallback savedPoseIndexData, out int savedPoseIndex)
	{
		if (savedPoseIndexData.TryGetValue(out savedPoseIndex, "SavedPoseNodeIndex") ||
			savedPoseIndexData.TryGetValue(out savedPoseIndex, "PoseNodeIndex") ||
			savedPoseIndexData.TryGetValue(out savedPoseIndex, "CachePoseNodeIndex"))
			return true;

		if (savedPoseIndexData.TryGetValue(out int[] nodeIndices, "OrderedSavedPoseNodeIndices") && nodeIndices.Length > 0)
		{
			savedPoseIndex = nodeIndices[0];
			return true;
		}

		savedPoseIndex = -1;
		return false;
	}

	private static Dictionary<string, FStructFallback> BuildStructFallbackMap(UScriptMap? map)
	{
		var result = new Dictionary<string, FStructFallback>(StringComparer.Ordinal);
		if (map?.Properties is not { Count: > 0 })
			return result;

		foreach (var (key, value) in map.Properties)
		{
			var keyName = GetMapString(key);
			if (string.IsNullOrEmpty(keyName) || !TryGetStructFallbackValue(value, out var structValue))
				continue;

			result[keyName] = structValue;
		}

		return result;
	}

	private static bool TryGetStructFallbackValue(FPropertyTagType? value, out FStructFallback structValue)
	{
		if (value is null)
		{
			structValue = null!;
			return false;
		}

		if (value.GenericValue is FStructFallback fallback)
		{
			structValue = fallback;
			return true;
		}

		if (value.GenericValue is FScriptStruct { StructType: FStructFallback scriptStructFallback })
		{
			structValue = scriptStructFallback;
			return true;
		}

		if (value.GetValue(typeof(FScriptStruct)) is FScriptStruct { StructType: FStructFallback typedScriptStructFallback })
		{
			structValue = typedScriptStructFallback;
			return true;
		}

		structValue = null!;
		return false;
	}

	private int ResolveRootAnimNodeIndex()
	{
		if (TryGetRootNodeIndexForFunction("AnimGraph", out var animGraphRootNodeIndex))
			return animGraphRootNodeIndex;

		for (var index = 0; index < AnimNodePropertyData.Length; index++)
		{
			if (AnimNodePropertyData[index].IsRootNode)
				return index;
		}

		return -1;
	}

	private bool TryLoadAnimNodeStruct(string nodeTypeName, out UStruct nodeStruct)
	{
		nodeStruct = null!;
		var normalizedNodeTypeName = NormalizeNodeTypeName(nodeTypeName);

		foreach (var property in AnimNodeProperties)
		{
			if (!property.Struct.TryLoad<UStruct>(out var structType))
				continue;

			if (!structType.Name.Equals(nodeTypeName, StringComparison.OrdinalIgnoreCase) &&
				!structType.Name.Equals(normalizedNodeTypeName, StringComparison.OrdinalIgnoreCase))
				continue;

			nodeStruct = structType;
			return true;
		}

		foreach (var (key, _) in NodeTypeFallbackData)
		{
			if (!key.Equals(nodeTypeName, StringComparison.OrdinalIgnoreCase) &&
				!NormalizeNodeTypeName(key).Equals(normalizedNodeTypeName, StringComparison.OrdinalIgnoreCase))
				continue;

			var matchingProperty = AnimNodeProperties.FirstOrDefault(property =>
				property.Struct.ResolvedObject?.Name.Text.Equals(key, StringComparison.OrdinalIgnoreCase) == true);
			if (matchingProperty?.Struct.TryLoad<UStruct>(out var structType) == true)
			{
				nodeStruct = structType;
				return true;
			}
		}

		return false;
	}

	private bool TryResolvePoseLinkForFunction(string functionName, string outputPosePropertyName, out FPoseLinkDescription poseLink)
	{
		TryLoadClassDefaultObject(out var cdo);
		if (cdo is not null && TryResolvePoseLinkForHolder(cdo, functionName, outputPosePropertyName, out poseLink))
			return true;

		if (cdo?.SerializedSparseClassData is not null &&
			TryResolvePoseLinkForHolder(cdo.SerializedSparseClassData, functionName, outputPosePropertyName, out poseLink))
			return true;

		poseLink = FPoseLinkDescription.Invalid;
		return false;
	}

	private static bool TryResolvePoseLinkForHolder(IPropertyHolder holder, string functionName,
		string outputPosePropertyName, out FPoseLinkDescription poseLink)
	{
		if (holder.TryGetValue(out FStructFallback functionStruct, functionName))
		{
			if (TryResolvePoseLinkFromStruct(functionStruct, outputPosePropertyName, out poseLink))
				return true;
		}

		if (!outputPosePropertyName.Equals(functionName, StringComparison.OrdinalIgnoreCase) &&
			holder.TryGetValue(out FStructFallback outputPoseStruct, outputPosePropertyName) &&
			TryCreatePoseLink(outputPoseStruct, out poseLink))
			return true;

		poseLink = FPoseLinkDescription.Invalid;
		return false;
	}

	private static bool TryResolvePoseLinkFromStruct(FStructFallback functionStruct, string outputPosePropertyName,
		out FPoseLinkDescription poseLink)
	{
		if (functionStruct.TryGetValue(out FStructFallback outputPoseStruct, outputPosePropertyName) &&
			TryCreatePoseLink(outputPoseStruct, out poseLink))
			return true;

		if (TryCreatePoseLink(functionStruct, out poseLink))
			return true;

		poseLink = FPoseLinkDescription.Invalid;
		return false;
	}

	private static bool TryCreatePoseLink(FStructFallback poseLinkStruct, out FPoseLinkDescription poseLink)
	{
		if (!poseLinkStruct.TryGetValue(out int linkId, "LinkID"))
		{
			poseLink = FPoseLinkDescription.Invalid;
			return false;
		}

		var sourceLinkId = poseLinkStruct.GetOrDefault<int>("SourceLinkID", -1);
		var sourceProperty = poseLinkStruct.GetOrDefault<FName>("SourceProperty").Text;
		poseLink = new FPoseLinkDescription(linkId, sourceLinkId, sourceProperty);
		return true;
	}

	private static bool TryGetNestedPoseLink(FStructFallback holder, string propertyName, out FPoseLinkDescription poseLink)
	{
		if (holder.TryGetValue(out FStructFallback poseLinkStruct, propertyName) && TryCreatePoseLink(poseLinkStruct, out poseLink))
			return true;

		poseLink = FPoseLinkDescription.Invalid;
		return false;
	}

	private int ResolveAnimNodePropertyIndexFromLinkId(int linkId, bool preferRootNode)
	{
		if (linkId < 0 || AnimNodePropertyData.Length == 0)
			return -1;

		var candidates = new List<int>();
		AddCandidate(linkId);

		var childPropertyIndexMatch = Array.FindIndex(AnimNodePropertyData,
			data => data.ChildPropertyIndex == linkId);
		AddCandidate(childPropertyIndexMatch);

		AddCandidate(AnimNodePropertyData.Length - 1 - linkId);

		var reversedChildPropertyIndex = (ChildProperties?.Length ?? 0) - 1 - linkId;
		var reversedChildPropertyMatch = Array.FindIndex(AnimNodePropertyData,
			data => data.ChildPropertyIndex == reversedChildPropertyIndex);
		AddCandidate(reversedChildPropertyMatch);

		if (preferRootNode)
		{
			var rootCandidate = candidates.FirstOrDefault(index => index >= 0 && index < AnimNodePropertyData.Length &&
				AnimNodePropertyData[index].IsRootNode);
			if (rootCandidate >= 0)
				return rootCandidate;
		}

		return candidates.FirstOrDefault(index => index >= 0 && index < AnimNodePropertyData.Length, -1);

		void AddCandidate(int index)
		{
			if (index < 0 || candidates.Contains(index))
				return;
			candidates.Add(index);
		}
	}

	private bool IsAnimNodeStruct(FStructProperty structProperty)
	{
		if (IsKnownAnimNodeType(structProperty))
			return true;

		return IsStructOrDerivedFrom(structProperty, "AnimNode_Base") ||
			   structProperty.Name.Text.Contains("AnimGraphNode", StringComparison.OrdinalIgnoreCase);
	}

	private bool IsKnownAnimNodeType(FStructProperty structProperty)
	{
		foreach (var candidateName in GetStructTypeLookupNames(structProperty))
		{
			foreach (var lookupName in GetNodeTypeLookupNames(candidateName))
			{
				if (NodeTypeFallbackAliases.ContainsKey(lookupName))
					return true;
			}
		}

		return false;
	}

	private static IEnumerable<string> GetStructTypeLookupNames(FStructProperty structProperty)
	{
		var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

		var resolvedName = structProperty.Struct.ResolvedObject?.Name.Text;
		if (!string.IsNullOrWhiteSpace(resolvedName) && seen.Add(resolvedName))
			yield return resolvedName;

		if (structProperty.Struct.TryLoad<UStruct>(out var structType))
		{
			if (!string.IsNullOrWhiteSpace(structType.Name) && seen.Add(structType.Name))
				yield return structType.Name;
		}
	}

	private static bool IsPoseLinkStruct(FStructProperty structProperty) =>
		IsStructName(structProperty, "PoseLink") ||
		IsStructName(structProperty, "FPoseLink") ||
		IsStructName(structProperty, "ComponentSpacePoseLink") ||
		IsStructName(structProperty, "FComponentSpacePoseLink");

	private static bool IsStructOrDerivedFrom(FStructProperty structProperty, string baseStructName)
	{
		if (structProperty?.Struct is null || !structProperty.Struct.TryLoad<UStruct>(out var current) || current is null)
			return false;

		while (current is not null)
		{
			if (current.Name.Equals(baseStructName, StringComparison.OrdinalIgnoreCase))
				return true;

			if (current.SuperStruct is null || current.SuperStruct.IsNull || !current.SuperStruct.TryLoad<UStruct>(out current))
				break;
		}

		return false;
	}

	private static bool IsStructName(FStructProperty structProperty, string structName) =>
		structProperty.Struct.ResolvedObject?.Name.Text.Equals(structName, StringComparison.OrdinalIgnoreCase) == true;

	private static bool TryGetNodeGuid(FAnimNodePropertyData propertyData, out FGuid guid)
	{
		if (propertyData.DefaultValue is not null && propertyData.DefaultValue.TryGetValue(out guid, "NodeGuid"))
			return true;

		guid = default;
		return false;
	}

	private static string ResolveTextValue(FStructFallback fallback, params string[] propertyNames)
	{
		foreach (var propertyName in propertyNames)
		{
			if (fallback.TryGetValue(out FName name, propertyName))
				return name.Text;

			if (fallback.TryGetValue(out string text, propertyName))
				return text;
		}

		return string.Empty;
	}

	private static string GetMapString(FPropertyTagType property)
	{
		if (property.GetValue(typeof(FName)) is FName name)
			return name.Text;

		if (property.GetValue(typeof(string)) is string text)
			return text;

		return property.GenericValue?.ToString() ?? string.Empty;
	}

	private static Dictionary<string, TValue> BuildAliasMap<TValue>(IReadOnlyDictionary<string, TValue> source)
	{
		var aliases = new Dictionary<string, TValue>(StringComparer.OrdinalIgnoreCase);
		foreach (var (key, value) in source)
		{
			foreach (var alias in GetNodeTypeLookupNames(key))
			{
				aliases.TryAdd(alias, value);
			}
		}

		return aliases;
	}

	private static IEnumerable<string> GetNodeTypeLookupNames(string nodeTypeName)
	{
		if (string.IsNullOrWhiteSpace(nodeTypeName))
			yield break;

		var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

		foreach (var candidate in ExpandNodeTypeLookupNames(nodeTypeName))
		{
			if (!string.IsNullOrEmpty(candidate) && seen.Add(candidate))
				yield return candidate;
		}
	}

	private static IEnumerable<string> ExpandNodeTypeLookupNames(string nodeTypeName)
	{
		var trimmedName = nodeTypeName.Trim();
		if (trimmedName.Length == 0)
			yield break;

		yield return trimmedName;

		if (TryExtractQuotedObjectPath(trimmedName, out var quotedObjectPath))
			yield return quotedObjectPath;

		var normalizedName = NormalizeNodeTypeName(trimmedName);
		if (!string.IsNullOrEmpty(normalizedName) && !normalizedName.Equals(trimmedName, StringComparison.OrdinalIgnoreCase))
			yield return normalizedName;
	}

	private static string NormalizeNodeTypeName(string nodeTypeName)
	{
		if (string.IsNullOrWhiteSpace(nodeTypeName))
			return string.Empty;

		var normalizedName = nodeTypeName.Trim();
		if (TryExtractQuotedObjectPath(normalizedName, out var quotedObjectPath))
			normalizedName = quotedObjectPath;

		var lastDotIndex = normalizedName.LastIndexOf('.');
		if (lastDotIndex >= 0 && lastDotIndex + 1 < normalizedName.Length)
			normalizedName = normalizedName[(lastDotIndex + 1)..];
		else
		{
			var lastSlashIndex = normalizedName.LastIndexOf('/');
			if (lastSlashIndex >= 0 && lastSlashIndex + 1 < normalizedName.Length)
				normalizedName = normalizedName[(lastSlashIndex + 1)..];
		}

		return normalizedName.Trim();
	}

	private static bool TryExtractQuotedObjectPath(string value, out string objectPath)
	{
		var firstQuoteIndex = value.IndexOf('\'');
		var lastQuoteIndex = value.LastIndexOf('\'');
		if (firstQuoteIndex >= 0 && lastQuoteIndex > firstQuoteIndex)
		{
			objectPath = value.Substring(firstQuoteIndex + 1, lastQuoteIndex - firstQuoteIndex - 1);
			return true;
		}

		objectPath = string.Empty;
		return false;
	}

	private FStructFallback? ResolveDefaultValue(string propertyName)
	{
		TryLoadClassDefaultObject(out var defaultObject);
		if (defaultObject != null && defaultObject.TryGetValue(out FStructFallback value, propertyName))
			return value;

		if (defaultObject?.SerializedSparseClassData != null &&
			defaultObject.SerializedSparseClassData.TryGetValue(out value, propertyName))
			return value;

		return null;
	}

	private bool TryLoadClassDefaultObject(out UObjectExport? defaultObject) =>
		ClassDefaultObject.TryLoad<UObjectExport>(out defaultObject);
}

[StructFallback]
public class FAnimBlueprintFunction
{
	public string Name;
	[JsonIgnore] public UFunction Function;
	public string OutputPosePropertyName;
	public string[] InputPoseNames;
	public int[] InputPoseNodeIndices;
	public int OutputPoseLinkID;
	public int OutputPoseNodeIndex;

	public FAnimBlueprintFunction(string name, UFunction function, string? outputPosePropertyName,
		string[] inputPoseNames, int[] inputPoseNodeIndices, int outputPoseLinkId, int outputPoseNodeIndex)
	{
		Name = name;
		Function = function;
		OutputPosePropertyName = outputPosePropertyName ?? string.Empty;
		InputPoseNames = inputPoseNames;
		InputPoseNodeIndices = inputPoseNodeIndices;
		OutputPoseLinkID = outputPoseLinkId;
		OutputPoseNodeIndex = outputPoseNodeIndex;
	}

	public bool HasOutputPose => !string.IsNullOrEmpty(OutputPosePropertyName);
}

public class FAnimNodePropertyData
{
	public string Name { get; }
	public string StructName { get; }
	public FStructProperty Property { get; }
	public int AnimNodePropertyIndex { get; }
	public int ChildPropertyIndex { get; }
	public FStructFallback? DefaultValue { get; }

	public bool IsRootNode => StructName.EndsWith("_Root", StringComparison.OrdinalIgnoreCase) ||
							  StructName.Equals("AnimNode_Root", StringComparison.OrdinalIgnoreCase);

	public bool IsLinkedAnimLayerNode => StructName.Equals("AnimNode_LinkedAnimLayer", StringComparison.OrdinalIgnoreCase);

	public bool IsLinkedAnimGraphNode => StructName.Equals("AnimNode_LinkedInputPose", StringComparison.OrdinalIgnoreCase) ||
										 StructName.Equals("AnimNode_LinkedAnimGraph", StringComparison.OrdinalIgnoreCase);

	public bool IsStateMachineNode => StructName.Equals("AnimNode_StateMachine", StringComparison.OrdinalIgnoreCase);

	public FAnimNodePropertyData(FStructProperty property, int animNodePropertyIndex, int childPropertyIndex,
		FStructFallback? defaultValue)
	{
		Property = property;
		Name = property.Name.Text;
		StructName = property.Struct.ResolvedObject?.Name.Text ?? string.Empty;
		AnimNodePropertyIndex = animNodePropertyIndex;
		ChildPropertyIndex = childPropertyIndex;
		DefaultValue = defaultValue;
	}
}

[Flags]
public enum EAnimNodeDataFlags : uint
{
	None = 0x00000000,
	HasInitialUpdateFunction = 0x00000001,
	HasBecomeRelevantFunction = 0x00000002,
	HasUpdateFunction = 0x00000004
}

public class FAnimNodeData
{
	public const uint InvalidEntry = 0xFFFFFFFF;
	public const uint InstanceDataFlag = 0x80000000;
	public const uint InstanceDataMask = ~InstanceDataFlag;

	public int AnimNodePropertyIndex { get; }
	public string PropertyName { get; }
	public string StructTypeName { get; }
	public FAnimNodePropertyData? PropertyData { get; }
	public FStructFallback? RawData { get; }
	public FStructFallback? NodeData { get; }
	public FStructFallback? ConstantData { get; }
	public FStructFallback? MutableData { get; }
	public FGuid? NodeGuid { get; }
	public uint[] Entries { get; }
	public int NodeIndex { get; }
	public EAnimNodeDataFlags Flags { get; }

	public bool HasData => RawData is not null || NodeData is not null || ConstantData is not null || MutableData is not null;
	public bool HasEntries => Entries.Length > 0;

	public FAnimNodeData(int animNodePropertyIndex, FAnimNodePropertyData? propertyData, FStructFallback? rawData)
	{
		AnimNodePropertyIndex = animNodePropertyIndex;
		PropertyData = propertyData;
		RawData = rawData;
		PropertyName = propertyData?.Name ?? ResolveName(rawData, "PropertyName", "Property", "SourceProperty") ?? string.Empty;
		StructTypeName = propertyData?.StructName ?? ResolveName(rawData, "NodeType", "StructType", "ScriptStruct") ?? string.Empty;
		NodeData = ResolveNestedStruct(rawData, "NodeData", "Data");
		ConstantData = ResolveNestedStruct(rawData, "ConstantData", "Constants", "FoldedData");
		MutableData = ResolveNestedStruct(rawData, "MutableData", "Mutables", "InstanceData");
		NodeGuid = ResolveGuid(rawData) ?? ResolveGuid(propertyData?.DefaultValue);
		Entries = rawData?.GetOrDefault<uint[]>("Entries", []) ?? [];
		NodeIndex = rawData?.GetOrDefault("NodeIndex", animNodePropertyIndex) ?? animNodePropertyIndex;
		Flags = (EAnimNodeDataFlags) (rawData?.GetOrDefault<uint>("Flags", (uint) EAnimNodeDataFlags.None) ?? 0);
	}

	public bool HasNodeAnyFlags(EAnimNodeDataFlags flags) => (Flags & flags) != 0;

	public int GetResolvedEntryIndex(int propertyIndex)
	{
		if (propertyIndex < 0 || propertyIndex >= Entries.Length)
			return -1;

		var entry = Entries[propertyIndex];
		if (entry == InvalidEntry)
			return -1;

		return unchecked((int) (entry & InstanceDataMask));
	}

	public bool IsInstanceDataEntry(int propertyIndex, out int entryIndex)
	{
		entryIndex = GetResolvedEntryIndex(propertyIndex);
		return entryIndex >= 0 && propertyIndex >= 0 && propertyIndex < Entries.Length && (Entries[propertyIndex] & InstanceDataFlag) != 0;
	}

	public bool IsConstantDataEntry(int propertyIndex, out int entryIndex)
	{
		entryIndex = GetResolvedEntryIndex(propertyIndex);
		return entryIndex >= 0 && propertyIndex >= 0 && propertyIndex < Entries.Length && (Entries[propertyIndex] & InstanceDataFlag) == 0;
	}

	public bool TryGetRawValue(UAnimBlueprintGeneratedClass animBlueprintClass, int propertyIndex, out FPropertyTag propertyTag) =>
		animBlueprintClass.TryGetNodeValueRaw(this, propertyIndex, out propertyTag);

	public bool TryGetRawValue(UAnimBlueprintGeneratedClass animBlueprintClass, string propertyName, out FPropertyTag propertyTag) =>
		animBlueprintClass.TryGetNodeValueRaw(this, propertyName, out propertyTag);

	public bool TryGetValue<T>(UAnimBlueprintGeneratedClass animBlueprintClass, int propertyIndex, out T value) =>
		animBlueprintClass.TryGetNodeValue(this, propertyIndex, out value);

	public bool TryGetValue<T>(UAnimBlueprintGeneratedClass animBlueprintClass, string propertyName, out T value) =>
		animBlueprintClass.TryGetNodeValue(this, propertyName, out value);

	private static FStructFallback? ResolveNestedStruct(FStructFallback? rawData, params string[] names)
	{
		if (rawData is null)
			return null;

		foreach (var name in names)
		{
			if (rawData.TryGetValue(out FStructFallback nestedStruct, name))
				return nestedStruct;
		}

		return null;
	}

	private static FGuid? ResolveGuid(FStructFallback? rawData)
	{
		if (rawData is not null && rawData.TryGetValue(out FGuid guid, "NodeGuid"))
			return guid;

		return null;
	}

	private static string? ResolveName(FStructFallback? rawData, params string[] names)
	{
		if (rawData is null)
			return null;

		foreach (var name in names)
		{
			if (rawData.TryGetValue(out FName textName, name))
				return textName.Text;

			if (rawData.TryGetValue(out string text, name))
				return text;
		}

		return null;
	}
}

public class FAnimNodeStructData
{
	public string NodeTypeName { get; }
	public FStructFallback? RawData { get; }
	public IReadOnlyDictionary<string, int> NameToIndexMap => _nameToIndexMap;
	public int NumProperties { get; }

	private readonly Dictionary<string, int> _nameToIndexMap;

	public FAnimNodeStructData(string nodeTypeName, FStructFallback? rawData, UStruct? nodeStruct)
	{
		NodeTypeName = nodeTypeName;
		RawData = rawData;
		_nameToIndexMap = BuildNameToIndexMap(rawData, nodeStruct);
		NumProperties = rawData?.GetOrDefault("NumProperties", _nameToIndexMap.Count) ?? _nameToIndexMap.Count;
	}

	public int GetPropertyIndex(string propertyName) =>
		_nameToIndexMap.TryGetValue(propertyName, out var propertyIndex) ? propertyIndex : -1;

	public int GetPropertyIndex(FName propertyName) => GetPropertyIndex(propertyName.Text);

	public int GetNumProperties() => NumProperties;

	public bool DoesLayoutMatch(FAnimNodeStructData other)
	{
		if (other is null || NumProperties != other.NumProperties || _nameToIndexMap.Count != other._nameToIndexMap.Count)
			return false;

		foreach (var (name, propertyIndex) in _nameToIndexMap)
		{
			if (!other._nameToIndexMap.TryGetValue(name, out var otherIndex) || otherIndex != propertyIndex)
				return false;
		}

		return true;
	}

	private static Dictionary<string, int> BuildNameToIndexMap(FStructFallback? rawData, UStruct? nodeStruct)
	{
		if (TryBuildMapFromFallback(rawData, out var nameToIndexMap))
			return nameToIndexMap;

		nameToIndexMap = new Dictionary<string, int>(StringComparer.Ordinal);
		var childProperties = nodeStruct?.ChildProperties;
		if (childProperties is null)
			return nameToIndexMap;

		for (var propertyIndex = 0; propertyIndex < childProperties.Length; propertyIndex++)
			nameToIndexMap[childProperties[propertyIndex].Name.Text] = propertyIndex;

		return nameToIndexMap;
	}

	private static bool TryBuildMapFromFallback(FStructFallback? rawData, out Dictionary<string, int> nameToIndexMap)
	{
		nameToIndexMap = new Dictionary<string, int>(StringComparer.Ordinal);
		if (rawData is null || !rawData.TryGetValue(out UScriptMap rawMap, "NameToIndexMap") || rawMap.Properties.Count == 0)
			return false;

		foreach (var (key, value) in rawMap.Properties)
		{
			var propertyName = GetMapString(key);
			if (string.IsNullOrEmpty(propertyName) || value is null)
				continue;

			if (value.GetValue(typeof(int)) is int propertyIndex)
				nameToIndexMap[propertyName] = propertyIndex;
		}

		return nameToIndexMap.Count > 0;
	}

	private static string GetMapString(FPropertyTagType property)
	{
		if (property.GetValue(typeof(FName)) is FName name)
			return name.Text;

		if (property.GetValue(typeof(string)) is string text)
			return text;

		return property.GenericValue?.ToString() ?? string.Empty;
	}
}

public readonly record struct FPoseLinkDescription(int LinkID, int SourceLinkID, string SourceProperty)
{
	public static FPoseLinkDescription Invalid => new(-1, -1, string.Empty);
}
