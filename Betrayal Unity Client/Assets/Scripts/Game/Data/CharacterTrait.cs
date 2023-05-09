[System.Serializable]
public class CharacterTrait
{
	public const int TraitValueCount = 8;
	
	public int Index = -1;
	public int Value1;
	public int Value2;
	public int Value3;
	public int Value4;
	public int Value5;
	public int Value6;
	public int Value7;
	public int Value8;
	
	public int GetValue(int index)
	{
		return index switch
		{
			1 => Value1,
				2 => Value2,
				3 => Value3,
				4 => Value4,
				5 => Value5,
				6 => Value6,
				7 => Value7,
				8 => Value8,
			_ => -1
		};
	}
}
