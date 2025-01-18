using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class ArrayExtension
{
	public static T GetRandom<T>(this IEnumerable<T> array)
	{
		T[] arr = array as T[] ?? array.ToArray();
		return arr.Length == 0 
			? default 
			: arr[Random.Range(0, arr.Length)];
	}
}