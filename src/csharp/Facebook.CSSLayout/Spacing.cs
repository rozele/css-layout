/**
 * Copyright (c) 2014, Facebook, Inc.
 * All rights reserved.
 *
 * This source code is licensed under the BSD-style license found in the
 * LICENSE file in the root directory of this source tree. An additional grant
 * of patent rights can be found in the PATENTS file in the same directory.
 */
namespace Facebook.CSSLayout
{

	public enum SpacingType
	{
		Left = 0,
		Top = 1,
		Right = 2,
		Bottom = 3,
		Vertical = 4,
		Horizontal = 5,
		All = 6
	}

	/**
	 * Utility class for handling CSS spacing (padding, margin, and borders). This is mostly necessary
	 * to properly implement interactions and updates for properties like margin, marginLeft, and
	 * marginHorizontal. This is not a great API and should probably be updated to use actual objects
	 * for type safety, defaults safety, and simplicity.
	 */

	static class Spacing
	{

		// Indices into FullSpacingArray and SpacingResultArray
		public static readonly int LEFT = (int) SpacingType.Left;
		public static readonly int TOP = (int) SpacingType.Top;
		public static readonly int RIGHT = (int) SpacingType.Right;
		public static readonly int BOTTOM = (int) SpacingType.Bottom;
		public static readonly int VERTICAL = (int) SpacingType.Vertical;
		public static readonly int HORIZONTAL = (int) SpacingType.Horizontal;
		public static readonly int ALL = (int) SpacingType.All;

		/**
	   * @return an instance of an array that can be used with {@link #updateSpacing}. Stores
	   * the value for each spacing type or NaN if it hasn't been explicitly set.
	   */

		public static float[] newFullSpacingArray()
		{
			return new[]
			{
				CSSConstants.Undefined,
				CSSConstants.Undefined,
				CSSConstants.Undefined,
				CSSConstants.Undefined,
				CSSConstants.Undefined,
				CSSConstants.Undefined,
				CSSConstants.Undefined,
				CSSConstants.Undefined,
			};
		}

		/**
	   * @return {@link #newSpacingResultArray} filled with zero.
	   */

		public static float[] newSpacingResultArray()
		{
			return newSpacingResultArray(0);
		}

		/**
	   * @return an instance of an array used to store the end result of the interactions between the
	   * values in a full spacing array. Use {@link #TOP}, etc to access result values.
	   */

		public static float[] newSpacingResultArray(float defaultValue)
		{
			return new[]
			{
				defaultValue,
				defaultValue,
				defaultValue,
				defaultValue,
			};
		}

		/**
	   * Given the fullSpacing from {@link #newFullSpacingArray()} and the spacingResult from
	   * {@link #newSpacingResultArray()} from a View, update them both to reflect a new value for the
	   * given spacingType (e.g. {@link #TOP}). defaultValue specifies the result value that should be
	   * used whenever a spacing property hasn't been set.
	   */

		public static void updateSpacing(
			float[] fullSpacing,
			float[] spacingResult,
			int spacingType,
			float value,
			float defaultValue)
		{
			fullSpacing[spacingType] = value;
			spacingResult[Spacing.TOP] =
				!CSSConstants.IsUndefined(fullSpacing[Spacing.TOP])
					? fullSpacing[Spacing.TOP]
					: !CSSConstants.IsUndefined(fullSpacing[Spacing.VERTICAL])
						? fullSpacing[Spacing.VERTICAL]
						: !CSSConstants.IsUndefined(fullSpacing[Spacing.ALL])
							? fullSpacing[Spacing.ALL]
							: defaultValue;
			spacingResult[Spacing.BOTTOM] =
				!CSSConstants.IsUndefined(fullSpacing[Spacing.BOTTOM])
					? fullSpacing[Spacing.BOTTOM]
					: !CSSConstants.IsUndefined(fullSpacing[Spacing.VERTICAL])
						? fullSpacing[Spacing.VERTICAL]
						: !CSSConstants.IsUndefined(fullSpacing[Spacing.ALL])
							? fullSpacing[Spacing.ALL]
							: defaultValue;
			spacingResult[Spacing.LEFT] =
				!CSSConstants.IsUndefined(fullSpacing[Spacing.LEFT])
					? fullSpacing[Spacing.LEFT]
					: !CSSConstants.IsUndefined(fullSpacing[Spacing.HORIZONTAL])
						? fullSpacing[Spacing.HORIZONTAL]
						: !CSSConstants.IsUndefined(fullSpacing[Spacing.ALL])
							? fullSpacing[Spacing.ALL]
							: defaultValue;
			spacingResult[Spacing.RIGHT] =
				!CSSConstants.IsUndefined(fullSpacing[Spacing.RIGHT])
					? fullSpacing[Spacing.RIGHT]
					: !CSSConstants.IsUndefined(fullSpacing[Spacing.HORIZONTAL])
						? fullSpacing[Spacing.HORIZONTAL]
						: !CSSConstants.IsUndefined(fullSpacing[Spacing.ALL])
							? fullSpacing[Spacing.ALL]
							: defaultValue;
		}
	}
}
