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
		Start = 6,
		End = 7,
		All = 8
	}

	/**
	 * Class representing CSS spacing (padding, margin, and borders). This is mostly necessary to
	 * properly implement interactions and updates for properties like margin, marginLeft, and
	 * marginHorizontal.
	 */

	public sealed class Spacing
	{
		/**
		 * Spacing type that represents the left direction. E.g. {@code marginLeft}.
		 */
		internal static readonly int LEFT = (int) SpacingType.Left;
		/**
		 * Spacing type that represents the top direction. E.g. {@code marginTop}.
		 */
		internal static readonly int TOP = (int) SpacingType.Top;
		/**
		 * Spacing type that represents the right direction. E.g. {@code marginRight}.
		 */
		internal static readonly int RIGHT = (int) SpacingType.Right;
		/**
		 * Spacing type that represents the bottom direction. E.g. {@code marginBottom}.
		 */
		internal static readonly int BOTTOM = (int) SpacingType.Bottom;
		/**
		 * Spacing type that represents vertical direction (top and bottom). E.g. {@code marginVertical}.
		 */
		internal static readonly int VERTICAL = (int) SpacingType.Vertical;
		/**
	     * Spacing type that represents horizontal direction (left and right). E.g.
		 * {@code marginHorizontal}.
		*/
		internal static readonly int HORIZONTAL = (int) SpacingType.Horizontal;
		/**
		 * Spacing type that represents start direction e.g. left in left-to-right, right in right-to-left.
		 */
		internal static readonly int START = (int)SpacingType.Start;
		/**
		 * Spacing type that represents end direction e.g. right in left-to-right, left in right-to-left.
		 */
		internal static readonly int END = (int)SpacingType.End;
		/**
		 * Spacing type that represents all directions (left, top, right, bottom). E.g. {@code margin}.
		 */
		internal static readonly int ALL = (int)SpacingType.All;

		/**
		 * @return an instance of an array that can be used with {@link #updateSpacing}. Stores
		 * the value for each spacing type or NaN if it hasn't been explicitly set.
		 */

		float[] mSpacing = newFullSpacingArray();
		[Nullable] float[] mDefaultSpacing = null;
		float[] mSpacingResult = newSpacingResultArray();

		/**
		 * Set a spacing value.
		 *
		 * @param spacingType one of {@link #LEFT}, {@link #TOP}, {@link #RIGHT}, {@link #BOTTOM},
		 *        {@link #VERTICAL}, {@link #HORIZONTAL}, {@link #ALL}
		 * @param value the value for this direction
		 * @return {@code true} if the spacing has changed, or {@code false} if the same value was already
		 *         set
		 */

		internal bool set(int spacingType, float value)
		{
			if (!FloatUtil.floatsEqual(mSpacing[spacingType], value))
			{
				mSpacing[spacingType] = value;
				return true;
			}
			return false;
		}

		/**
		 * Set a default spacing value. This is used as a fallback when no spacing has been set for a
		 * particular direction.
		 *
		 * @param spacingType one of {@link #LEFT}, {@link #TOP}, {@link #RIGHT}, {@link #BOTTOM}
		 * @param value the default value for this direction
		 * @return
		 */

		internal bool setDefault(int spacingType, float value)
		{
			if (mDefaultSpacing == null)
				mDefaultSpacing = newSpacingResultArray();

			if (!FloatUtil.floatsEqual(mDefaultSpacing[spacingType], value))
			{
				mDefaultSpacing[spacingType] = value;
				return true;
			}
			return false;
		}

		/**
		 * Get the spacing for a direction. This takes into account any default values that have been set.
		 *
		 * @param spacingType one of {@link #LEFT}, {@link #TOP}, {@link #RIGHT}, {@link #BOTTOM}
		 */

		internal float get(int spacingType)
		{
			int secondType = spacingType == TOP || spacingType == BOTTOM ? VERTICAL : HORIZONTAL;
			float defaultValue = spacingType == START || spacingType == END ? CSSConstants.UNDEFINED : 0;
			return
				!CSSConstants.isUndefined(mSpacing[spacingType])
					? mSpacing[spacingType]
					: !CSSConstants.isUndefined(mSpacing[secondType])
						? mSpacing[secondType]
						: !CSSConstants.isUndefined(mSpacing[ALL])
							? mSpacing[ALL]
							: mDefaultSpacing != null
								? mDefaultSpacing[spacingType]
								: defaultValue;
		}

		public float Get(SpacingType spacingType)
		{
			return get((int) spacingType);
		}

		/**
		 * Get the raw value (that was set using {@link #set(int, float)}), without taking into account
		 * any default values.
		 *
		 * @param spacingType one of {@link #LEFT}, {@link #TOP}, {@link #RIGHT}, {@link #BOTTOM},
		 *        {@link #VERTICAL}, {@link #HORIZONTAL}, {@link #ALL}
		 */

		internal float getRaw(int spacingType)
		{
			return mSpacing[spacingType];
		}

		public float GetRaw(SpacingType spacingType)
		{
			return getRaw((int) spacingType);
		}

		static float[] newFullSpacingArray()
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
				CSSConstants.Undefined,
			};
		}

		static float[] newSpacingResultArray()
		{
			return newSpacingResultArray(0);
		}

		static float[] newSpacingResultArray(float defaultValue)
		{
			return new[]
			{
				defaultValue,
				defaultValue,
				defaultValue,
				defaultValue,
				defaultValue,
				defaultValue,
				CSSConstants.UNDEFINED,
				CSSConstants.UNDEFINED,
				defaultValue
			};
		}
	}
}
