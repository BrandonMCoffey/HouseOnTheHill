using CoffeyUtils;

[System.Serializable]
public class DoorPosition
{
	public int X;
	public int Z;
	
	public bool AlongZ;
	
	[ShowIf("AlongZ"/*, Reverse = true*/)] public int X2;
	[ShowIf("AlongZ")] public int Z2;
}
