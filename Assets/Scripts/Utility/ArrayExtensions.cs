﻿using System.Collections;
using System;

public static class ArrayExtensions {
	public static T[] SubArray<T>(this T[] source, int start, int length)
	{
		T[] subArr = new T[length];
		Array.Copy (source, start, subArr, 0, length);

		return subArr;
	}

	public static T[] Append<T>(this T[] first, T[] second) {
		T[] combined = new T[first.Length + second.Length];

		Array.Copy (first, combined, first.Length);
		Array.Copy (second, 0, combined, first.Length, second.Length);

		return combined;
	}
}