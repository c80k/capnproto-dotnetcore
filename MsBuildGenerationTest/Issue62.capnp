@0x84b3d81482a93cd8;


interface Issue62
{
	listOfCaps @0 () -> (result :List(CapElement));

	interface CapElement
	{
		method @0 () -> ();
	}
}
