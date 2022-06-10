﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrchidMod.Utilities
{
	public static class MathUtils
	{
		public static T MultiLerp<T>(Func<T, T, float, T> method, float percent, params T[] values)
		{
			if (method == null) throw new ArgumentNullException(nameof(method));
			if (percent >= 1) return values.Last();

			percent = Math.Max(percent, 0);
			float num = 1f / (values.Length - 1);
			int index = Math.Max(0, (int)(percent / num));

			return method.Invoke(values[index], values[index + 1], (percent - num * index) / num);
		}
	}
}