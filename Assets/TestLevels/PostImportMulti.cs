#if TOOLS
using Godot;
using System;

[Tool]
public partial class PostImportMulti : EditorScenePostImport
{
	public override GodotObject _PostImport(Node scene)
	{

		Iterate(scene);
		return scene; // Return the modified root node when you're done.
	}

	private void Iterate(Node node)
	{
		string name = node.Name.ToString();
		if (name.StartsWith("MultiMesh"))
		{
			HandleMultiMesh(node);
			return;
		}
		
		
		foreach (Node child in node.GetChildren())
		{
			Iterate(child);
		}
	}

	void HandleMultiMesh(Node node)
	{
		Node3D container = node as Node3D;
		if (container == null)
			return;

		int numChildren = container.GetChildCount();
		if (numChildren < 1)
			return;

		MeshInstance3D meshInstance = container.GetChild(0) as MeshInstance3D;
		if (meshInstance == null || meshInstance.Mesh == null)
			return;

		MultiMesh multimesh = new MultiMesh
		{
			TransformFormat = MultiMesh.TransformFormatEnum.Transform3D,
			Mesh = meshInstance.Mesh,
			InstanceCount = numChildren
		};

		for (int i = 0; i < numChildren; i++)
		{
			Node3D child = container.GetChild(i) as Node3D;
			if (child == null)
				continue;

			multimesh.SetInstanceTransform(i, child.Transform);
			child.QueueFree();
		}

		MultiMeshInstance3D mmInstance = new MultiMeshInstance3D
		{
			Multimesh = multimesh,
			Transform = container.Transform
		};

		container.GetParent().AddChild(mmInstance);
		mmInstance.Owner = container.Owner;
		
		container.QueueFree();
		mmInstance.Name = container.Name;
	}

	
}
#endif
