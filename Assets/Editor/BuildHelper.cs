using UnityEditor;
using System.IO;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;

class BuildHelper
{
	static void Build()
	{
		string[] levels = { "Assets/main.unity" };
		BuildPipeline.BuildPlayer(levels, "output", BuildTarget.iOS, BuildOptions.None);
	}
}
